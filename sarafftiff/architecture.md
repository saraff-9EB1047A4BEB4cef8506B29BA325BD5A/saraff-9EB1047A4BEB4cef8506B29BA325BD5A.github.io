[All products](../) / [Saraff.Tiff.NET](./index.md)
# Architecture

Saraff.Tiff.NET is deliberately small. It has a **public facade**, a **reader state machine**,
a **writer pipeline** and a **binary I/O layer**. Everything is built around classic TIFF
structures: an 8-byte header, a chain of IFDs, 12-byte directory entries and typed value blocks.

## Layers

```mermaid
flowchart TB
    App["Application code"] --> API["Public API<br/>TiffReader / TiffWriter"]
    API --> Options["TiffReaderOptions<br/>TiffWriterOptions"]
    API --> Reader["Reader automaton<br/>Header -> IFD -> Tag -> Value"]
    API --> Writer["Writer pipeline<br/>validate -> sort -> serialize -> patch -> write"]

    Reader --> BR["TiffBinaryReader<br/>seekable stream / value buffer"]
    Writer --> BW["TiffBinaryWriter<br/>seekable stream"]

    Reader --> Types["TiffTypeTable"]
    Reader --> Tags["TiffTagRegistry"]
    Writer --> Types

    BR --> Stream["Seekable Stream"]
    BW --> Stream
    Stream --> Container["File / MemoryStream / embedded container"]
```

The public API never exposes the automaton or the binary primitives. It exposes TIFF concepts:
`ITag`, `TiffHandle`, `TiffTags`, `TiffDataType` and raw `byte[]` blocks.

## Public surface

| Type | Responsibility |
|---|---|
| `TiffReader` | Header, IFD chain, tag iteration, typed values, `ReadData` for raw blocks |
| `TiffWriter` | Header, raw data, IFD writing, multi-page chaining |
| `TiffReaderOptions` | `LeaveOpen`, `MaxValueBytes`, `MaxPages`, `BaseOffset` |
| `TiffWriterOptions` | `LeaveOpen`, `ByteOrder`, `MaxValueBytes`, `BaseOffset` |
| `Tag<T>` / `ITagValues` | Typed tag values for writing |
| `ITagInfo` | Tag id, TIFF data type, CLR item type and count for reading |
| `TiffHandle` | A relative offset to a TIFF location, used for strips/tiles/IFDs |
| `TiffRational` | RATIONAL and SRATIONAL value |
| `TiffFormatException` | Malformed or unsafe TIFF data |

## Reader state machine

The reader is a strict automaton. `ReadValue()` must consume every value of the current tag
before the next `ReadTag()`, and the next `ReadTag()` must not be called until the current
directory is exhausted. This is what makes the reader forward-only and non-cached.

```mermaid
stateDiagram-v2
    [*] --> Header
    Header --> ImageFileDirectory: ReadHeader
    ImageFileDirectory --> Tag: directory has tags
    ImageFileDirectory --> [*]: no more directories
    Tag --> Value: ReadTag
    Value --> Tag: all values read
    Tag --> ImageFileDirectory: all tags read
```

* `ReadHeader()` validates the byte-order marker, version 42 and reads the first IFD offset.
* `ReadImageFileDirectory()` validates the IFD offset, tag count, cycle detection and `MaxPages`.
* `ReadTag()` reads one 12-byte entry and buffers its value block in one bounded read.
* `ReadValue()` parses the buffer. ASCII returns `string`; RATIONAL/SRATIONAL return
  `TiffRational`; known tags are converted to CLR enums.
* `ReadHandle()` is `ReadValue()` followed by offset conversion to `TiffHandle`.

> **Important:** the automaton does not recover after an exception. A malformed file produces
> `TiffFormatException` (or `NotSupportedException` for BigTIFF) and the reader should be
> disposed.

## Writer pipeline

The writer is also forward-only. It validates and serializes all tags before touching the
owner handle, so most argument errors do not leave a half-written directory.

```mermaid
sequenceDiagram
    participant App
    participant W as TiffWriter
    participant IO as TiffBinaryWriter
    participant S as Stream

    App->>W: WriteHeader()
    W->>IO: byte order marker, version 42, placeholder
    W-->>App: header handle

    App->>W: WriteData(strip bytes)
    W->>IO: raw bytes at current position
    W-->>App: data handle

    App->>W: WriteImageFileDirectory(owner, tags)
    W->>W: validate, deduplicate, sort, serialize
    W->>IO: patch owner with new IFD offset
    W->>IO: write tag count and 12-byte entries
    W->>IO: write next-IFD = 0
    W->>IO: write delayed values at aligned offsets
    W-->>App: next-IFD handle
```

Key rules:

* Tags are sorted by tag id; duplicate tag ids are rejected.
* Values of 4 bytes or less are stored inline in the entry value field.
* Larger values are written after the IFD and their offsets are patched afterwards.
* Delayed values are aligned to an even offset.
* `WriteImageFileDirectory()` returns a handle to the **next-IFD field**. Pass it as the
  `owner` of the next page to build a chain.

## Offset patching

TIFF is a graph of offsets. The writer creates placeholder fields and patches them later:

```mermaid
flowchart LR
    H["Header<br/>dirOffset"] -->|patched by first IFD| I0["IFD 0"]
    I0 -->|patched by second IFD| I1["IFD 1"]
    I1 -->|next = 0| End["End of chain"]

    E["IFD entry<br/>value / offset"] -->|patched after IFD| V["Out-of-line value block"]
```

The same mechanism is used for strips/tiles: `WriteData()` returns a `TiffHandle`, and
`Tag<TiffHandle>` writes that relative offset into the `StripOffsets` or `TileOffsets` tag.
The writer never disposes caller handles.

## Embedded TIFF and `BaseOffset`

Every offset inside a TIFF file is relative to the TIFF header, not to the physical start of the
stream. `BaseOffset` tells the reader/writer where the header lives:

```mermaid
flowchart LR
    Prefix["Container prefix<br/>length = BaseOffset"] --> Header["TIFF header"]
    Header --> IFD["IFD at relative offset 8"]
    IFD --> Data["Values and strips<br/>relative offsets"]
    Header -.->|reader/writer add BaseOffset| Physical["Physical stream position"]
```

With `BaseOffset = 0` the behavior is the classic "TIFF starts at byte 0" case.

## Data and tag caches

| Component | Purpose |
|---|---|
| `TiffTypeTable` | Maps `TiffDataType` to size and CLR type, and CLR type to TIFF data type |
| `TiffTagRegistry` | Maps known tag ids to their CLR enums; built once and cached |
| `TiffBinaryReader` | Explicit little/big-endian primitives, bounds checks, value buffer mode |
| `TiffBinaryWriter` | Explicit little/big-endian primitives, relative offsets, patching |

The registry is built from the assembly's tag attributes on first use. It is not rebuilt for
every value, which removes the reflection cost of the 1.x reader.

## Release pipeline

```mermaid
flowchart LR
    Push["push / pull request"] --> CI["CI<br/>Windows: net10.0 + net48<br/>Linux: net10.0"]
    CI --> Pack["pack + version verification"]
    Release["GitHub Release<br/>or workflow_dispatch"] --> Publish["publish.yml"]
    Publish --> Build["build + test<br/>with release version"]
    Build --> Verify["verify nupkg, assembly and file versions"]
    Verify --> OIDC["NuGet/login<br/>OIDC temporary key"]
    OIDC --> NuGet["push .nupkg + .snupkg<br/>nuget.org"]
```

The package and assembly versions are verified before any push. See
[Safety and limits](./safety.md) and [Migration](./migration.md) for the behavioral details.

## Continue reading

* [TIFF data model](./data-model.md) - header, IFD and entry layout.
* [Reader](./reader.md) - practical reading recipes.
* [Writer](./writer.md) - practical writing recipes.
