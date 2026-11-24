[All products](../) / [Saraff.ThreadingModel.NET](./index.md)
# Architecture

The library has three building blocks: a **dispatcher** (the thread), a **proxy** (the interception
layer) and a **queue** (the message pipeline). Events are a projection of the proxy layer.

```mermaid
flowchart TB
    subgraph App["Application threads"]
        A1["Worker"]
        A2["UI"]
        A3["Pool"]
    end

    subgraph ProxyLayer["Saraff.ThreadingModel"]
        SP["SingleThreadedProxy"]
        EV["EventAdapter<br/>Expression trees"]
        DISP["SingleThreadedDispatcher"]
        SC["StaSynchronizationContext"]
        PUMP["StaMessagePump<br/>MsgWaitForMultipleObjects"]
        QMD["QueueMessageDispatcher"]
        REG["QueueMessageHandlerRegistry"]
        INV["MessageHandlerInvoker<br/>cached delegates"]
    end

    subgraph Container["Saraff.IoC"]
        C["ServiceContainer"]
        S["IServiceScope per message"]
        H["IQueueMessageHandler&lt;T&gt;"]
    end

    A1 --> SP
    A2 --> SP
    A3 --> SP
    SP -->|Send / Invoke / Post| DISP
    DISP --> SC
    DISP --> PUMP
    DISP --> QMD
    QMD --> REG
    QMD --> S
    S --> H
    H --> INV
    H --> SP
    SP --> EV
    EV -->|SynchronizationContext| App
    C -->|provider| DISP
    C --> S
```

## Responsibilities

| Component | Responsibility |
|---|---|
| `SingleThreadedProxy` | Intercepts every interface call and event accessor, forwards work to the dispatcher, rethrows original exceptions |
| `SingleThreadedDispatcher` | Owns the thread, serializes work, provides `Send`/`Invoke`/`InvokeAsync`/`Post`/`PostAsync`, pumps Win32 messages, handles shutdown |
| `StaSynchronizationContext` | `Post`/`Send` for async continuations; executes inline when already on the dispatcher thread |
| `StaMessagePump` | Keeps the STA thread responsive to COM/Win32 messages while waiting for work |
| `QueueMessageDispatcher` | Turns posted messages into handler invocations with a per-message scope and error policy |
| `QueueMessageHandlerRegistry` | Maps message types to registered service types (exact type, base type, `object` catch-all) |
| `MessageHandlerInvoker` | Compiles and caches `(handler, message, ct) => Task` delegates |
| `EventAdapter` | Holds the subscriber list, builds the proxy delegate with expression trees, delivers events |

Public types live in `Saraff.ThreadingModel`; internals are in `Saraff.ThreadingModel.Core` and
`Saraff.ThreadingModel.Messaging`.

## Synchronous call path

```mermaid
sequenceDiagram
    participant Caller
    participant Proxy as SingleThreadedProxy
    participant Dispatcher as SingleThreadedDispatcher
    participant STA as STA thread
    participant Target

    Caller->>Proxy: method(args)
    Proxy->>Dispatcher: Send(closure)
    Note over Proxy,Dispatcher: blocks the caller
    Dispatcher->>Dispatcher: enqueue WorkItem
    Dispatcher->>STA: signal queue event
    STA->>Target: method(args)
    Target-->>STA: result or exception
    STA-->>Dispatcher: TaskCompletionSource
    Dispatcher-->>Caller: result or exception
    Caller->>Proxy: receives value
```

If the caller is already on the dispatcher thread (for example, inside a handler or an event
subscriber), the call is executed **inline** instead of being queued, so a handler does not deadlock
on itself.

## Asynchronous call path

```mermaid
sequenceDiagram
    participant Caller
    participant Dispatcher as SingleThreadedDispatcher
    participant STA as STA thread

    Caller->>Dispatcher: InvokeAsync(func)
    Dispatcher-->>Caller: Task of T (pending)
    Dispatcher->>STA: enqueue
    STA->>STA: func()
    STA-->>Dispatcher: result
    Dispatcher-->>Caller: Task completes
```

`PostAsync` has the same shape, but the task completes after the message handlers (and their scope)
finish. `Post` returns immediately and reports errors through `UnhandledException`.

## Message queue path

```mermaid
flowchart LR
    P["Post(message)"] --> Q["Dispatcher queue"]
    Q --> S{"IServiceProvider?"}
    S -->|"yes"| SC["CreateScope()"]
    S -->|"no"| E["InvalidOperationException"]
    SC --> R["Resolve handlers<br/>by message type"]
    R --> H1["Handler A"]
    R --> H2["Handler B"]
    H1 --> X{"Exception?"}
    H2 --> X
    X -->|"yes"| EH["IQueueMessageErrorHandler chain"]
    X -->|"no"| D["Dispose scope"]
    EH --> D
    D --> F["PostAsync completion"]
```

* Handlers run **on the dispatcher thread**.
* If a handler returns a pending `Task`, the dispatcher keeps pumping messages and continuations
  until the task completes (or `HandlerTimeout` fires).
* Handlers are resolved from a fresh scope; scoped dependencies are disposed after the message.
* Base-type handlers and `object` catch-all handlers are included (registration order).

## Event path

```mermaid
sequenceDiagram
    participant Subscriber
    participant Proxy as SingleThreadedProxy
    participant Adapter as EventAdapter
    participant STA as STA thread
    participant Target

    Subscriber->>Proxy: target.Completed += handler
    Proxy->>Adapter: create with captured SynchronizationContext
    Proxy->>Target: add_Completed(proxyDelegate)
    Note over Target: target keeps ONE proxy delegate
    Target->>Adapter: proxyDelegate(sender, args)
    alt subscriber has a foreign context
        Adapter->>Subscriber: context.Send(handler)
    else no context or same dispatcher
        Adapter->>Subscriber: handler(...) on STA thread
    end
```

One proxy delegate is attached to the target regardless of how many subscribers exist. `add` and
`remove` maintain the subscriber list inside the adapter; the last `remove` detaches the proxy
delegate from the target. See [Events](./Events.md) for details.

## Thread and lifetime model

```mermaid
stateDiagram-v2
    [*] --> Started: constructor
    Started --> Running: thread signals startup
    Running --> Running: execute work / pump messages
    Running --> Stopping: Dispose / DisposeAsync
    Stopping --> Stopped: queue drained
    Stopped --> [*]

    Running --> Running: Send / Invoke / Post
    Stopping --> Stopping: queued work is still executed
    Stopped --> Stopped: ObjectDisposedException for new calls
```

* The dispatcher thread is a background thread, so it never keeps the process alive.
* `Dispose` waits up to `DisposeTimeout`; `Dispose` called from the dispatcher thread does not wait.
* A proxy created without an explicit dispatcher owns it and disposes it.
* A proxy created with a shared dispatcher does not own it.
* A dispatcher created through `RegisterSingleThreadedDispatcher` is owned by the container.

## Error propagation

| Layer | Behavior |
|---|---|
| Target method | Original exception is captured and rethrown with its stack (`ExceptionDispatchInfo`) |
| Proxy call | The caller receives the original exception type, not `TargetInvocationException` |
| Event handler | The exception propagates through the proxy call that raised the event |
| `Post` work item | Reported to `UnhandledException` |
| Queue handler | Sent to all `IQueueMessageErrorHandler`; the queue continues or stops according to `ContinueOnHandlerError` |
| Dispatcher thread | Exceptions never kill the thread; they are reported |

## Guarantees

* A target instance is touched by exactly one thread unless the application itself leaks it.
* Calls are serialized in FIFO order with respect to the moment they are enqueued.
* `Send`/`Invoke` either return a result, throw the original exception, or throw `TimeoutException`.
* After `Dispose`, new calls throw `ObjectDisposedException`.
* Messages are processed in FIFO order; each message sees a fresh DI scope.
* Win32/COM messages are pumped while idle **and** while a handler awaits asynchronous work.
