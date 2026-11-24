---
layout: default
title: Saraff.Twain.NET — API Reference
---
[All products](../) / [Saraff.Twain.NET](./index.md)
# API Reference

## `Twain32`

Namespace: `Saraff.Twain`

### Session

| Member | Description |
|---|---|
| `OpenDSM()` / `CloseDSM()` | Open/close the Data Source Manager |
| `SelectSource()` | Show the source selection dialog |
| `OpenDataSource()` / `CloseDataSource()` | Open/close the selected source |
| `Acquire()` | Open DSM/DS, enable and start scanning |
| `SourceIndex` / `SourcesCount` | Current/default source information |
| `SetDefaultSource(int)` / `GetDefaultSource()` | Default source management |
| `ImageCount` / `GetImage(int)` | Acquired images |

### Source settings

| Member | Description |
|---|---|
| `ShowUI` | Show the source user interface; default `true` |
| `ModalUI` | Request a modal source UI |
| `Parent` | `IWin32Window` owner of the source UI |
| `DisableAfterAcquire` | Disable the source after the first transfer; default `true` |
| `IsTwain2Enable` | Use TWAIN 2.x DSM |
| `IsTwain2Supported` | DSM supports TWAIN 2.x |
| `DsmPath` | Explicit DSM library path |
| `Language` / `Country` | TWAIN language/country |
| `ImageLayout` | Image layout rectangle (in `RectangleF`) |
| `Capabilities` | Strongly typed capability access |
| `Palette` | Current palette |
| `CustomDSData` | Source-specific custom data |

### Capabilities

| Member | Description |
|---|---|
| `GetCap(TwCap)` | Read a capability |
| `GetCurrentCap(TwCap)` | Read the current value |
| `GetDefaultCap(TwCap)` | Read the default value |
| `SetCap(...)` | Set a capability (value, array, `Range`, `Enumeration`) |
| `IsCapSupported(TwCap)` | `TwQC` flags |
| `ResetCap(TwCap)` / `ResetAllCap()` | Reset capability values |

### Events

| Event | Description |
|---|---|
| `XferDone` | Native/memory transfer is ready for inspection |
| `EndXfer` | Image has been converted and added to the collection |
| `MemXferEvent` | Buffered memory chunk |
| `SetupMemXferEvent` | Memory buffer setup |
| `FileXferEvent` | Disk-file transfer completed |
| `SetupFileXferEvent` | Disk-file name/format setup |
| `AcquireCompleted` | A `XFerReady` cycle completed |
| `AcquireError` | TWAIN error |
| `DeviceEvent` | Scanner device event |
| `TwainStateChanged` | Session state flags changed |

### Nested helper types

| Type | Description |
|---|---|
| `Range` | Capability range: min/max/step/default/current |
| `Enumeration` | Capability enumeration |
| `ImageInfo` | Width, height, resolution, bits, compression |
| `ExtImageInfo` | Extended image information |
| `ImageMemXfer` | Memory transfer chunk |
| `ImageFileXfer` | Disk file description |
| `ColorPalette` / `TwainPalette` | Palette data |
| `Identity` | Source identity |
| `TwainStateFlag` | Session state flags |

### Exceptions

`TwainException` contains the TWAIN status (`TwCC`) and return code (`TwRC`) and is raised for
unsupported capabilities, DSM errors and source errors.
