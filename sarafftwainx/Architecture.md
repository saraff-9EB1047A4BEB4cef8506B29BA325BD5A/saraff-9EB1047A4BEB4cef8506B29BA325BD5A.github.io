---
layout: default
title: Saraff.TwainX — Architecture
---
[All products](../) / [Saraff.TwainX](./index.md)
# Architecture

`Saraff.TwainX` is a managed TWAIN 2.x client. It talks to the native Data Source Manager and
converts transferred data to .NET `Stream` objects.

## Components

```mermaid
flowchart TB
    subgraph Public["Public API"]
        TX["TwainX"]
        AS["TwainXAcquisitionSession"]
        OPT["TwainXAcquireOptions"]
        RES["TwainXAcquisitionResult"]
    end

    subgraph Internal["Internal session layer"]
        ST["TwainSessionState"]
        RUN["TwainXAcquisitionRunner"]
        THR["TwainXSessionThread"]
        ADP["TwainXSessionAdapter"]
    end

    subgraph Interop["Interop and shared code"]
        DSM["_DsmEntry"]
        MEM["_Memory / _Platform"]
        DEF["TwainDefs / capabilities / image handlers"]
    end

    subgraph Native["TWAIN 2.x"]
        NDSM["TWAINDSM"]
        DS["DS2 source"]
    end

    TX --> ST
    TX --> DEF
    TX --> DSM
    AS --> THR
    AS --> RUN
    RUN --> ADP
    ADP --> TX
    RUN --> RES
    AS --> OPT
    DSM --> NDSM
    NDSM --> DS
    MEM --> DSM
```

### Main types

| Type | Role |
|---|---|
| `TwainX` | Synchronous component and TWAIN session |
| `TwainXAcquisitionSession` | Async facade; owns a dedicated session thread |
| `TwainXAcquireOptions` | Source UI, parent, page limit and progress |
| `TwainXAcquisitionResult` | Result-owned stream copies and image metadata |
| `TwainSessionState` | Session flags: DSM/DS open, enabled, ready |
| `TwainXAcquisitionRunner` | Opens/enables/closes the session and collects streams |
| `TwainXSessionThread` | Dedicated thread, STA and Win32 pump on Windows |
| `TwainXSessionAdapter` | Internal adapter used by the async runner |

## Synchronous flow

```mermaid
sequenceDiagram
    participant App
    participant TX as TwainX
    participant DSM
    participant DS

    App->>TX: Acquire()
    TX->>DSM: OpenDSM
    TX->>DS: OpenDS
    TX->>DS: EnableDS
    DS-->>TX: XferReady
    TX->>TX: transfer + EndXfer
    TX-->>App: events
    App->>TX: CloseDSM / Dispose
```

`Acquire()` runs on the caller's thread. The caller must provide a message loop on Windows (for
example, a WinForms/WPF application thread).

## Asynchronous flow

```mermaid
sequenceDiagram
    participant App
    participant AS as Session
    participant TH as Session thread
    participant TX as TwainX
    participant DSM
    participant DS

    App->>AS: AcquireAsync()
    AS->>TH: post work
    TH->>TX: new + configure
    TH->>DSM: OpenDSM
    TH->>DS: OpenDS
    TH->>DS: EnableDS
    loop transfer
        DS-->>TX: XferReady
        TX-->>TH: EndXfer
        TH->>TH: copy stream
    end
    TH->>DSM: CloseDSM
    TH-->>AS: result
    AS-->>App: Task<result>
```

The async path does not expose `TwainX` to the caller. This keeps all TWAIN calls on the session
thread and avoids cross-thread `Dispose` issues.

## Event flow

```mermaid
stateDiagram-v2
    [*] --> DSMClosed
    DSMClosed --> DSMOpen: OpenDSM
    DSMOpen --> DSOpen: OpenDataSource
    DSOpen --> DSEnabled: EnableDS
    DSEnabled --> DSEnabled: XferReady
    DSEnabled --> DSOpen: DisableDS
    DSOpen --> DSMOpen: CloseDataSource
    DSMOpen --> DSMClosed: CloseDSM
```

Events:

* `EndXfer` — a transferred image is ready as a `Stream`;
* `XferDone` — image metadata is available and a transfer can be cancelled;
* `AcquireCompleted` — a `XFerReady` cycle completed;
* `AcquireError` — a TWAIN error;
* `TwainStateChanged` — session flags changed;
* `DeviceEvent` — a source device event.

`AcquireCompleted` can fire more than once. For “scan finished”, combine it with
`TwainStateChanged`, `DisableAfterAcquire`, `MaxImages` or the async result API.

## Design constraints

* TWAIN 2.x/DS2 only; TWAIN 1.x is handled by `Saraff.Twain`.
* `netstandard2.0`; no WinForms/WPF types.
* One TWAIN session per acquisition.
* All session calls must use one thread.
* Native memory is released through `TwainDsmBuffer`/`_Memory` helpers.
* Events are invoked through a safe dispatcher that isolates handler exceptions.
* Native transfer can be customised through `IStreamProvider` and `IImageHandler`; custom image
  types are created through `IImageFactory<T>` — see [Image Pipeline](./ImagePipeline.md).

## Product boundary

```mermaid
flowchart LR
    subgraph Win["Saraff.Twain"]
        T32["Twain32<br/>net462 / net8.0-windows"]
        UI["WinForms / WPF<br/>Image / ImageSource"]
    end
    subgraph X["Saraff.TwainX"]
        TX["TwainX<br/>netstandard2.0"]
        ST["Streams<br/>no UI framework"]
    end
    T32 --- UI
    TX --- ST
```

The products share only source-level interop definitions and low-level helpers; their public APIs
and packages are independent.

## Next

* [Async Acquisition](./AsyncAcquisition.md)
* [Threading and UI](./Threading.md)
* [Images and Transfer](./TransferAndImages.md)
