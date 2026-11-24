[All products](../) / [Saraff.ThreadingModel.NET](./index.md)
# Threading and COM

The library exists because some objects must live on a specific thread. COM components, TWAIN data
sources, UI controls and many vendor SDKs expect their calls to arrive on an STA thread that also
processes Windows messages. Everything else in the application can stay multi-threaded.

## Apartments in one picture

```mermaid
flowchart LR
    subgraph MTA["MTA threads"]
        W1["Worker"]
        W2["Thread pool"]
    end
    subgraph STA["STA apartment"]
        T["Dispatcher thread<br/>message pump"]
        O["COM object / TWAIN source"]
    end

    W1 -->|"proxy"| T
    W2 -->|"proxy"| T
    T --> O
```

* An **STA** apartment is single-threaded: objects created in it must be used from that thread.
* A **message pump** is required for cross-apartment COM calls: the caller's RPC requests are
  delivered as Windows messages, and the STA thread must dispatch them.
* Without a pump, an incoming call can wait forever even though the STA thread is "alive".

Saraff.ThreadingModel starts the thread with `ApartmentState.STA` and pumps messages with
`MsgWaitForMultipleObjects` (`UseMessagePump = true` by default).

## What the pump does

```mermaid
flowchart TB
    W["MsgWaitForMultipleObjects"] --> A{"Queue event?"}
    A -->|"yes"| B["Execute queued work items"]
    A -->|"no"| C{"Win32 message?"}
    C -->|"yes"| D["PeekMessage / Translate / Dispatch"]
    C -->|"no"| W
    B --> W
    D --> W
```

* The thread waits efficiently for either internal work or incoming Win32 messages.
* COM calls are dispatched while the thread is idle.
* Messages are also pumped while a queue handler awaits (`HandlerTimeout` loop), so an `await` in a
  handler does not freeze COM.

## Verification

```csharp
using (var proxy = new SingleThreadedProxy(typeof(IScanner), typeof(Scanner))) {
    var scanner = proxy.GetTransparentProxy<IScanner>();
    Console.WriteLine(scanner.Apartment);            // STA
    Console.WriteLine(scanner.ThreadId);             // stable across calls
}
```

```csharp
var dispatcher = new SingleThreadedDispatcher();
dispatcher.Send(() => {
    Console.WriteLine(Thread.CurrentThread.GetApartmentState());   // STA
    Console.WriteLine(SingleThreadedDispatcher.Current == dispatcher);   // True
});
```

## A TWAIN-like scenario

```mermaid
sequenceDiagram
    participant App as Application thread
    participant Scanner as IScanner proxy
    participant Dispatcher as STA dispatcher
    participant DSM as TWAIN DSM / device

    App->>Scanner: Open()
    Scanner->>Dispatcher: Send
    Dispatcher->>DSM: DSM_Entry on STA
    DSM-->>Dispatcher: TWRC_SUCCESS
    Dispatcher-->>App: result

    App->>Scanner: Acquire()
    Scanner->>Dispatcher: Send
    Dispatcher->>DSM: DSM_Entry + message loop
    DSM-->>Dispatcher: transfer messages pumped
    DSM-->>Dispatcher: TWRC_XFERDONE
    Dispatcher-->>App: result

    App->>Scanner: Close()
    Scanner->>Dispatcher: Send
    Dispatcher->>DSM: DSM_Entry
```

The caller is blocked for the duration of `Acquire`; the STA thread pumps messages while the device
transfers data, so the COM/TWAIN protocol can complete.

## UI threads

A UI thread is usually STA with its own `SynchronizationContext`. Two rules keep the application
responsive:

1. Do not block the UI thread with `Send`/`Invoke` for long operations.
2. Let events and completions flow back through the UI context.

```csharp
// UI event handler
private async void OnScanClick(object sender, EventArgs e) {
    var result = await dispatcher.InvokeAsync(() => scanner.Scan(10));
    label.Text = $"scanned {result}";
}
```

```mermaid
sequenceDiagram
    participant UI as UI thread
    participant Dispatcher as STA dispatcher
    participant Scanner

    UI->>Dispatcher: InvokeAsync(Scan)
    Note over UI: UI thread is free
    Dispatcher->>Scanner: Scan
    Scanner-->>Dispatcher: result
    Dispatcher-->>UI: task completes on captured context
    UI->>UI: update controls
```

## Common deadlock patterns

### 1. UI thread blocks on the dispatcher

```csharp
var value = scanner.SlowOperation();   // called from a UI event handler
```

Not a deadlock by itself, but the UI freezes until the STA finishes. Use `InvokeAsync`/`PostAsync`.

### 2. Dispatcher sends an event to the blocked UI thread

```mermaid
sequenceDiagram
    participant UI as UI thread
    participant Dispatcher as STA dispatcher

    UI->>Dispatcher: Send(Scan) and waits
    Dispatcher->>Dispatcher: Scan raises Completed
    Dispatcher->>UI: context.Send(handler)
    Note over UI,Dispatcher: UI cannot process the handler<br/>because it is waiting for Send
    Note over UI,Dispatcher: deadlock
```

The event adapter captures the UI `SynchronizationContext` at subscription time and calls
`context.Send`, which requires the UI thread. The UI thread is blocked in `Send` to the dispatcher.

**Fix:** use `InvokeAsync`/`PostAsync` on the UI thread, or unsubscribe/raise events without a UI
context on the dispatcher thread.

### 3. Handler waits for a thread that calls the same dispatcher

```mermaid
sequenceDiagram
    participant STA as STA handler
    participant Worker

    STA->>STA: HandleAsync started
    STA->>Worker: wait for signal
    Worker->>STA: Send(...)
    Note over STA,Worker: STA is inside the handler and cannot execute Send
    Note over STA,Worker: deadlock
```

**Fix:** handlers must not synchronously wait for callbacks into the same dispatcher. Use `Post`
(fire and forget) or redesign the flow.

### 4. `Task.Result` or `.Wait()` on the UI thread

```csharp
var task = dispatcher.InvokeAsync(() => scanner.Scan(1));
var result = task.Result;     // blocks the UI context; continuations cannot run
```

**Fix:** `await` instead of blocking.

## Guidelines

| Situation | Do | Avoid |
|---|---|---|
| UI event | `await dispatcher.InvokeAsync(...)` | `Send`/`Invoke` on the UI thread |
| Queue handler | call the proxy directly | waiting for another thread that calls back |
| Event subscriber | quick handler; marshal long work out | blocking the dispatcher in a handler |
| Background worker | `Send` if the result is needed, `Post` otherwise | taking locks that the STA also takes |
| Shutdown | `Dispose` the owner (container/proxy/dispatcher) | leaving the dispatcher running in a loop |

## Notes

* .NET initializes COM on the thread automatically; the library does not call `CoInitializeEx` for
  you because the thread is managed by the CLR.
* `ApartmentState.MTA` is allowed but disables the COM affinity guarantees described here.
* `UseMessagePump = false` is useful for tests and pure CPU work; do not use it for COM objects.
* A dispatcher thread is a background thread: it never prevents process exit.
