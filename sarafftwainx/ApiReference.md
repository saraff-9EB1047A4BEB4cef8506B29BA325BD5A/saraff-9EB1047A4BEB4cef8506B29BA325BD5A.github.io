---
layout: default
title: Saraff.TwainX — API Reference
---
[All products](../) / [Saraff.TwainX](./index.md)
# API Reference

Namespace: `Saraff.TwainX`

## `TwainX`

### Session

| Member | Description |
|---|---|
| `OpenDSM()` / `CloseDSM()` | Open/close the TWAIN 2.x DSM |
| `SelectSource()` | Source selection dialog |
| `OpenDataSource()` / `CloseDataSource()` | Open/close the selected source |
| `Acquire()` | Synchronous acquisition |
| `SourceIndex` / `SourcesCount` | Source information |
| `GetSourceProductName(int)` / `GetSourceIdentity(int)` | Source details |

### Settings

| Member | Description |
|---|---|
| `ShowUI` | Show source UI; default `false` |
| `ModalUI` | Request a modal source UI |
| `Parent` | `IntPtr` parent window for `EnableDS` |
| `DisableAfterAcquire` | Disable after first transfer; default `true` |
| `DsmPath` | Explicit DSM library path |
| `ImageLayout` | Image layout rectangle |
| `Capabilities` | Strongly typed capabilities |
| `Palette` | Current palette |
| `CustomDSData` | Source-specific custom data |

### Events

| Event | Description |
|---|---|
| `EndXfer` | Image is available as `EndXferEventArgs.Stream` |
| `XferDone` | Metadata and cancellation |
| `MemXferEvent` / `SetupMemXferEvent` | Memory transfer |
| `FileXferEvent` / `SetupFileXferEvent` | Disk-file transfer |
| `AcquireCompleted` | A `XFerReady` cycle completed |
| `AcquireError` | TWAIN error |
| `DeviceEvent` | Device event |
| `TwainStateChanged` | State flags changed |

## Async API

### `TwainXAcquisitionSession`

| Member | Description |
|---|---|
| `TwainXAcquisitionSession()` | Create a session with default options |
| `TwainXAcquisitionSession(TwainXAcquireOptions)` | Create a configured session |
| `AcquireAsync(CancellationToken)` | Run one acquisition |
| `Dispose()` | Stop the session thread |

### `TwainXAcquireOptions`

| Member | Default | Description |
|---|---|---|
| `ShowUI` | `false` | Show source UI |
| `ModalUI` | `false` | Request modal UI |
| `Parent` | `IntPtr.Zero` | Parent window handle |
| `DsmPath` | `null` | Explicit DSM path |
| `MaxImages` | `null` | Page limit |
| `DisableAfterAcquire` | `true` | Disable after first transfer |
| `Progress` | `null` | `IProgress<TwainXAcquisitionProgress>` |

### `TwainXAcquisitionResult`

| Member | Description |
|---|---|
| `Streams` | Result-owned image streams |
| `ImageInfos` | Metadata per page |
| `Dispose()` | Dispose all result streams |

## Types

| Type | Description |
|---|---|
| `TwainX.Range` | Capability range |
| `TwainX.Enumeration` | Capability enumeration |
| `TwainX.ImageInfo` | Width, height, resolution, bits, compression |
| `TwainX.ExtImageInfo` | Extended image information |
| `TwainX.ImageMemXfer` | Memory transfer chunk |
| `TwainX.ImageFileXfer` | File transfer description |
| `TwainX.ColorPalette` / `TwainX.TwainPalette` | Palette data |
| `TwainX.Identity` | Source identity |
| `TwainX.TwainStateFlag` | Session state flags |
| `TwainException` | TWAIN error with status and return code |

## Transfer values

| Value | Meaning |
|---|---|
| `TwSX.Native` | Native bitmap |
| `TwSX.Memory` | Buffered memory |
| `TwSX.File` | Disk file |
| `TwSX.MemFile` | Encoded memory file |

See [Images and Transfer](./TransferAndImages.md) for examples.
