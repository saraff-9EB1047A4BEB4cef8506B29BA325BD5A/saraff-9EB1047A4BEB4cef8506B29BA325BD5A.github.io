---
layout: default
title: Saraff.TwainX — Troubleshooting
---
[All products](../) / [Saraff.TwainX](./index.md)
# Troubleshooting

## The package cannot be restored in a netstandard2.0 project

`Saraff.TwainX` targets `netstandard2.0`. Make sure you reference the `Saraff.TwainX` package, not
the Windows-only `Saraff.Twain` package.

## DSM or source is not found

`Saraff.TwainX` requires:

* a TWAIN 2.x Data Source Manager (`TWAINDSM.dll` / `libtwaindsm`);
* a TWAIN 2.x/DS2 data source.

Use `DsmPath` if the DSM is installed in a non-default location:

```csharp
using (var twain = new TwainX()) {
    twain.DsmPath = "/usr/local/lib/libtwaindsm.so";
}
```

## TWAIN 1.x source is not supported

`Saraff.TwainX` requires DS2. For TWAIN 1.x sources use the Windows `Saraff.Twain` product
(`Twain32`).

## `InvalidOperationException` about threads

A `TwainX` instance is bound to the first TWAIN thread. Do not share one instance between the UI
thread and a worker thread.

* Use `Acquire()` on the TWAIN/message-loop thread.
* Or use `TwainXAcquisitionSession.AcquireAsync()` and let it own the thread.

## `AcquireAsync` never completes

Check:

| Cause | Fix |
|---|---|
| `ShowUI = false` + `DisableAfterAcquire = false` | Set `MaxImages` or `DisableAfterAcquire = true` |
| Source waits for user input | Set `ShowUI = true` and close the source UI |
| Source error | Subscribe to diagnostics or check the task exception |
| Cancellation ignored | TWAIN cancellation is best effort; close the UI/DSM |
| Wrong platform pump | Test the async path on Windows first |

The async runner completes on:

* `MaxImages`;
* `DisableAfterAcquire`;
* `AcquireCompleted` when `ShowUI = false`;
* DS disabled/closed;
* `AcquireError`;
* cancellation.

## Streams are empty or disposed

`EndXferEventArgs.Stream` is owned by the event args. If you keep it after the handler returns, copy
it into a `MemoryStream`.

`TwainXAcquisitionResult.Streams` already contains copies. Do not dispose a result before you finish
reading its streams.

## Parent window is ignored

`Parent` is applied to `EnableDS` only. It does not change `OpenDSM`, `CloseDSM` or `DisableDS` in
this product. Verify that the handle is valid and belongs to the calling UI thread.

## Progress is not on the UI thread

`TwainXAcquireOptions.Progress` is invoked on the session thread. Use `Progress<T>` created on the
UI thread or marshal manually.

## Async cancellation does not interrupt a modal source UI

TWAIN has no universal cancel operation. The runner requests DSM closure, but the source may keep its
modal UI open until the user closes it. Design the UX around this limitation.

## Deadlock when blocking on the task

Avoid:

```csharp
var result = session.AcquireAsync().Result;
```

Prefer `await`. If you must block, do not block the UI thread that receives progress callbacks.

## Platform notes

| Platform | Async acquisition |
|---|---|
| Windows | STA + Win32 message pump |
| Linux | Dedicated thread; no Win32 pump |
| macOS | Dedicated thread; no Win32 pump |

Test with the exact DSM and source used in production.

## Next

* [Async Acquisition](./AsyncAcquisition.md)
* [Threading and UI](./Threading.md)
* [Migration](./Migration.md)
