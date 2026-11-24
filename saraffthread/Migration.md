[All products](../) / [Saraff.ThreadingModel.NET](./index.md)
# Migration from 1.x

Version 2.0 keeps the public `SingleThreadedProxy` shape but replaces the internals, the target
frameworks and the event pipeline. This page lists everything that can affect existing code.

| | 1.x | 2.0 |
|---|---|---|
| Target framework | .NET Framework 2.0 | `net462`, `net8.0-windows` |
| Packaging | old-style `.csproj`, no solution/tests/CI | SDK-style, solution, tests, CI, NuGet package |
| Strong name | signed with committed `sign.snk` | `net8.0-windows` public-signed (`54f373c993f693dc`); `net462` unsigned |
| Dispatcher | private helper thread per proxy | `SingleThreadedDispatcher`, shareable and disposable |
| Serialization | `Monitor.Enter` on the proxy | FIFO dispatcher queue |
| Event pipeline | runtime IL generation via `AssemblyBuilder` | expression trees, cached shapes |
| Message pump | none | Win32 pump (`MsgWaitForMultipleObjects`) |
| Lifecycle | infinite background loop, no `Dispose` | `IDisposable`/`IAsyncDisposable`, timeouts |
| Queue | none | message handlers resolved from Saraff.IoC |
| .NET Core / .NET 5+ | not supported | `net8.0-windows` via `DispatchProxy` |

## Package and namespaces

```powershell
dotnet add package Saraff.ThreadingModel
```

```xml
<ItemGroup>
  <PackageReference Include="Saraff.ThreadingModel" Version="2.0.0" />
</ItemGroup>
```

The root namespace is still `Saraff.ThreadingModel`. New namespaces:

* `Saraff.ThreadingModel.Messaging` — internal queue plumbing (no public types required by consumers).
* `Saraff.ThreadingModel.DependencyInjection` — the Saraff.IoC registration helpers.

> The 1.x package stays on NuGet for projects that must remain on .NET Framework 2.0. 2.0 does not
> support .NET 2.0.

## Preserved API

```csharp
// 1.x — still compiles and works in 2.0
var scanner = new SingleThreadedProxy(typeof(IScanner)).GetTransparentProxy() as IScanner;
var scanner = new SingleThreadedProxy(typeof(Scanner), "arg1", 42).GetTransparentProxy() as IScanner;
var scanner = new SingleThreadedProxy(new Scanner()).GetTransparentProxy() as IScanner;
```

* All three constructors are still present.
* `GetTransparentProxy()` is unchanged.
* Added `GetTransparentProxy<T>()` — typed helper.
* Added overloads that accept a shared `SingleThreadedDispatcher`.
* Added the explicit `(serviceType, implementationType, dispatcher, args)` constructor.

## Removed types

| Type | Status | Replacement |
|---|---|---|
| `Saraff.ThreadingModel.Core.EventHandlerHelper` | removed (was public) | internal `EventAdapter`, used automatically |
| `Saraff.ThreadingModel.Core.SingleThreadedHelper` | removed (was internal) | `SingleThreadedDispatcher` |

These types were public in 1.x but served as internals. If you referenced them directly, switch to
the public proxy and dispatcher.

## Behavior changes

| Area | 1.x | 2.0 |
|---|---|---|
| Call serialization | `lock (proxy)` around every call | dispatcher queue; calls are still serialized |
| Constructor failure | could hang the first call forever | exception is returned to the caller |
| Exception shape | often wrapped in `TargetInvocationException` | original exception with its stack |
| Event delivery | executed on the caller's stack through a hand-off trampoline | subscriber `SynchronizationContext` (`Send`) or dispatcher thread |
| Event with non-`void` delegate | invalid IL | supported |
| Event with `null` argument | `NullReferenceException` | accepted |
| Event with `ref`/`out` delegate | broken IL | explicit `NotSupportedException` |
| Event leak | dynamic types per subscription | adapters are released when the last subscriber leaves |
| Long-running call | no timeout | `InvokeTimeout` / `TimeoutException` |
| Queue | absent | `Post`/`PostAsync` + handlers |
| Shutdown | background loop forever | `Dispose`/`DisposeAsync`, `ObjectDisposedException` |

### Serialization semantics

1.x used `Monitor.Enter(this)` inside `Invoke`, which serialized callers **and** made the proxy object
a lock target. 2.0 serializes work in the dispatcher queue. Public behavior (one call at a time on
the target thread) is the same, but:

* external code can no longer deadlock by locking the proxy object;
* `Send`/`Invoke` no longer hold a monitor while the target runs;
* calls made from the dispatcher thread execute inline.

### Event semantics

```csharp
// 1.x: event handler ran on the thread that called the method
// 2.0: handler runs through the subscriber SynchronizationContext if one was captured
scanner.Completed += (sender, args) => UpdateUi(args);
```

If you subscribed from a UI thread, the handler now arrives on the UI thread through
`SynchronizationContext.Send`. If you subscribed from a thread without a context, the handler runs on
the dispatcher thread (same as the old "caller thread" in most cases).

A slow event handler still blocks the operation that raised the event; the difference is *which*
thread runs it.

## Strong naming

The committed `sign.snk` was removed because its private key was public. Version 2.0 uses
**public signing** for `net8.0-windows` with `eng/strongname.public.snk`; no private key is stored in
the repository. `net462` stays unsigned because .NET Framework verifies strong names and rejects
public-signed/delay-signed assemblies (`HRESULT 0x80131045`).

| Target | Strong name | Public key token |
|---|---|---|
| `net8.0-windows` | `PublicSign` | `54f373c993f693dc` |
| `net462` | none | — |

Update binding redirects and `assemblyIdentity` entries:

```xml
<!-- 1.x -->
<assemblyIdentity name="Saraff.ThreadingModel"
                  publicKeyToken="..." culture="neutral" />

<!-- 2.0, net8.0-windows -->
<assemblyIdentity name="Saraff.ThreadingModel"
                  publicKeyToken="54f373c993f693dc" culture="neutral" />

<!-- 2.0, net462 (unsigned) -->
<assemblyIdentity name="Saraff.ThreadingModel" culture="neutral" />
```

If your `net462` application requires a strong name (GAC, verification enabled), full-sign it with
a private key kept outside the repository. See [Strong Name](./StrongName.md).

## Migration checklist

1. Retarget the project to `net462` or `net8.0-windows`.
2. Add the `Saraff.ThreadingModel` package reference (and `Saraff.IoC` if you want DI helpers).
3. Replace direct `Core.EventHandlerHelper` usage with `SingleThreadedProxy` /
   `SingleThreadedDispatcher` (the old helper type was public but internal by design).
4. If you lock the proxy object yourself, remove that lock — it is no longer used by the library.
5. Review event handlers that assumed they run on the calling thread; UI subscriptions now require
   marshaling discipline (see [Events](./Events.md)).
6. Set `InvokeTimeout`/`HandlerTimeout` where a hang was possible before.
7. Dispose proxies, dispatchers and containers. Prefer `using` blocks.
8. For new code, register a dispatcher and proxies through Saraff.IoC (see
   [Dependency Injection](./DependencyInjection.md)).
9. Update binding redirects/`assemblyIdentity` for the new public key token (see
   [Strong Name](./StrongName.md)).

## Code before and after

```csharp
// 1.x
var proxy = new SingleThreadedProxy(typeof(IScanner));
var scanner = (IScanner)proxy.GetTransparentProxy();
var result = scanner.Scan(1);
// no way to stop the worker thread
```

```csharp
// 2.0
using (var dispatcher = new SingleThreadedDispatcher()) {
    using (var proxy = new SingleThreadedProxy(typeof(IScanner), typeof(Scanner), dispatcher)) {
        proxy.InvokeTimeout = TimeSpan.FromSeconds(10);
        var scanner = proxy.GetTransparentProxy<IScanner>();
        var result = scanner.Scan(1);
    }
}
```

```csharp
// 2.0 with DI and a message queue
using (var container = new ServiceContainer()) {
    container.RegisterSingleThreadedDispatcher("scanner");
    container.RegisterSingleThreadedProxy<IScanner, Scanner>("scanner");
    container.RegisterQueueMessageHandler<ScanRequest, ScanRequestHandler>();

    var dispatcher = container.ResolveNamed<SingleThreadedDispatcher>("scanner");
    await dispatcher.PostAsync(new ScanRequest(1, "invoice"));
}
```
