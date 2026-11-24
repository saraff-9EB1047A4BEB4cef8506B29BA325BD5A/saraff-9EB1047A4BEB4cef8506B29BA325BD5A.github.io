---
layout: default
title: Saraff.Twain.NET — Troubleshooting
---
[All products](../) / [Saraff.Twain.NET](./index.md)
# Troubleshooting

## No data sources are visible

| Check | What to do |
|---|---|
| Bitness | A 64-bit process cannot load a 32-bit TWAIN source. Use a 64-bit source/DSM or run the app as x86. |
| DSM | Install the TWAIN Data Source Manager or set `DsmPath`. |
| Mode | On x86, TWAIN 1.x uses `twain_32.dll`; TWAIN 2.x needs `TWAINDSM.dll`. |
| Source | Confirm that the scanner driver is installed and visible in another TWAIN application. |

```csharp
twain.IsTwain2Enable = true;              // x64 is already true by default
twain.DsmPath = @"C:\Windows\System32\TWAINDSM.dll";
```

## `InvalidOperationException` about threads

The component binds to the first TWAIN thread:

```text
TWAIN operations must be performed on the same thread.
Операции TWAIN должны выполняться в одном потоке.
```

Create, use and dispose `Twain32` on the UI/STA thread. Do not call `Acquire()` from a thread-pool
thread.

## Source UI appears behind the application

Set `Parent` before the first TWAIN operation:

```csharp
twain.Parent = this;   // WinForms Form
```

For WPF, pass an `IWin32Window` adapter around the main window handle.

## `Acquire()` returns but no images appear

`Acquire()` does not always block until the scan is complete. Use events:

```csharp
twain.AcquireCompleted += (sender, e) => { /* a page is ready */ };
twain.AcquireError += (sender, e) => { /* show error */ };
twain.Acquire();
```

Check `ImageCount` after the event or after the TWAIN UI closes. If you disable the automatic
disable, close the session explicitly:

```csharp
twain.DisableAfterAcquire = false;
// ... user scans several pages ...
twain.CloseDSM();
```

## Images are empty or corrupted

* Check `Capabilities.XferMech` and the selected transfer mode.
* For `TwSX.File`, inspect `FileXferEvent`/`ImageFileXfer.FileName`.
* For `TwSX.Memory`, read `MemXferEvent` chunks in order.
* Ensure that the native bitmap handler matches the platform (`DibToImage`, `Tiff`, `Pict`).
* Do not dispose an image while the collection still owns it.

## Scanner settings are ignored

Capabilities are source-specific. Check support first:

```csharp
var supported = twain.IsCapSupported(TwCap.Resolution);
if ((supported & TwQC.Set) != 0) {
    twain.Capabilities.XResolution.Set(300f);
}
```

If a capability is not supported, the source silently uses its default or throws
`TwainException`.

## `Dispose()` does not release the DSM

`Dispose()` must run on the TWAIN thread. When it is called from another thread, the component
writes a `Trace` warning and skips native DSM cleanup to avoid an unsafe call.

```csharp
protected override void Dispose(bool disposing) {
    if (disposing) {
        this._twain.Dispose();
    }
    base.Dispose(disposing);
}
```

## WPF image handling

`EndXferEventArgs.ImageSource` is created on the TWAIN/UI thread. Do not pass a non-frozen WPF
`ImageSource` to another thread. If you need cross-thread use, copy the pixels or create a frozen
`BitmapImage` on the UI thread.

## Diagnostics

* Enable `Trace` listeners to see TWAIN and disposal warnings.
* Check `TwainException` properties for the TWAIN return code and status.
* Test with the same bitness as the target application.
* Try `ShowUI = true` first, then move to a headless/custom UI scenario.

## Next

* [Scanning](./Scanning.md)
* [Architecture](./Architecture.md)
* [Migration](./Migration.md)
