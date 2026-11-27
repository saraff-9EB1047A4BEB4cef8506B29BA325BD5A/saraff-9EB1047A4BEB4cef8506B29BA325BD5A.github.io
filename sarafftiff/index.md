[All products](../)
# Saraff.Tiff.NET

[![NuGet](https://img.shields.io/nuget/v/Saraff.Tiff.svg)](https://www.nuget.org/packages/Saraff.Tiff/)
[![CI](https://github.com/saraff-9EB1047A4BEB4cef8506B29BA325BD5A/Saraff.Tiff.NET/actions/workflows/ci.yml/badge.svg)](https://github.com/saraff-9EB1047A4BEB4cef8506B29BA325BD5A/Saraff.Tiff.NET/actions/workflows/ci.yml)
![license: LGPL 3.0 or later](https://img.shields.io/badge/license-LGPL%203.0%20or%20later-blue?style=flat&logo=git)

**Saraff.Tiff.NET** is a small, dependency-free C# library for low-level reading and writing
of classic TIFF files. It parses and writes headers, image file directories (IFDs), tags,
typed values and arbitrary binary blocks. It includes built-in PackBits, LZW and Deflate
codecs, but it does **not** decode or render pixels or perform color conversion.

| | |
|---|---|
| **Package** | [`Saraff.Tiff`](https://www.nuget.org/packages/Saraff.Tiff/) |
| **Version** | 2.1.0 |
| **Target frameworks** | `netstandard2.0`, `net48`, `net10.0` |
| **Namespaces** | `Saraff.Tiff`, `Saraff.Tiff.Core`, `Saraff.Tiff.Compression` |
| **TIFF support** | Classic TIFF 6.0, little-endian and big-endian |
| **Compression** | NONE, PackBits, LZW, zlib-wrapped Deflate; integer predictor |
| **Not supported** | BigTIFF, JPEG/JPEG2000/JBIG/CCITT, floating-point predictor, pixel conversion |
| **License** | LGPL-3.0-or-later |

> **Upgrading from 1.0.1.401?** Version 2.0 is a major update. ASCII values are returned as
> `string`, RATIONAL values as `TiffRational`, malformed input throws `TiffFormatException`,
> streams are not disposed by default, and the broken .NET 2.0 project was removed.
> Version 2.1 adds built-in compression codecs and codec registries. See
> [Migration](./migration.md).

## Install

```powershell
dotnet add package Saraff.Tiff
```

Package Manager Console:

```powershell
Install-Package Saraff.Tiff
```

## The model in one picture

```mermaid
flowchart TB
    App["Application code"] --> API["Public API<br/>TiffReader / TiffWriter"]
    API --> Options["Options<br/>LeaveOpen, limits, BaseOffset, codecs"]
    API --> R["Reader<br/>strict state machine"]
    API --> W["Writer<br/>IFD pipeline"]
    R --> IOR["TiffBinaryReader<br/>bounds + byte order"]
    W --> IOW["TiffBinaryWriter<br/>offsets + byte order"]
    R --> Codecs["Codec registries<br/>PackBits / LZW / Deflate<br/>custom codecs"]
    W --> Codecs
    R --> Cache["TiffTypeTable<br/>TiffTagRegistry"]
    W --> Cache
    IOR --> Stream["Seekable Stream"]
    IOW --> Stream
    Codecs --> Stream
    Stream --> File["TIFF file / MemoryStream / container"]
```

The reader and writer are **forward-only and non-cached** at the TIFF level. They do not build
an image object model and they do not touch pixel values unless you explicitly call `ReadData`
or `WriteData`. Compression is applied only when a compression-aware overload is used; the
`ReadData(handle, count)` and `WriteData(bytes)` overloads still operate on raw blocks.

## Quick start: writing

```csharp
using System.Collections.ObjectModel;
using System.IO;
using Saraff.Tiff;
using Saraff.Tiff.Core;

using var stream = File.Create("sample.tif");
using var writer = TiffWriter.Create(stream);

var owner = writer.WriteHeader();          // header at position 0
var pixels = writer.WriteData(pixelBytes); // raw strip/tile data

writer.WriteImageFileDirectory(owner, new Collection<ITag> {
    Tag<ushort>.Create(TiffTags.ImageWidth, 2),
    Tag<ushort>.Create(TiffTags.ImageLength, 2),
    Tag<ushort>.Create(TiffTags.BitsPerSample, 8, 8, 8),
    Tag<TiffCompression>.Create(TiffTags.Compression, TiffCompression.NONE),
    Tag<TiffPhotoMetric>.Create(TiffTags.PhotometricInterpretation, TiffPhotoMetric.RGB),
    Tag<TiffHandle>.Create(TiffTags.StripOffsets, pixels),
    Tag<ushort>.Create(TiffTags.SamplesPerPixel, 3),
    Tag<ushort>.Create(TiffTags.RowsPerStrip, 2),
    Tag<ushort>.Create(TiffTags.StripByteCounts, 12)
});

writer.Flush();
```

To compress the block, pass the compression scheme and a codec context:

```csharp
using System.Collections.Generic;

var context = new TiffCodecContext(TiffByteOrder.LittleEndian,
    new Dictionary<TiffTags, object> {
        [TiffTags.Predictor] = 2,
        [TiffTags.ImageWidth] = 2,
        [TiffTags.BitsPerSample] = new[] { 8, 8, 8 },
        [TiffTags.SamplesPerPixel] = 3,
        [TiffTags.PlanarConfiguration] = 1
    });

var compressed = writer.WriteData(pixelBytes, TiffCompression.LZW, context);
// StripByteCounts must contain the compressed length.
```

The built-in codecs and custom codec registration are described in [Codecs](./codecs.md).

## Quick start: reading

```csharp
using System.IO;
using Saraff.Tiff;
using Saraff.Tiff.Core;

using var stream = File.OpenRead("sample.tif");
using var reader = TiffReader.Create(stream, new TiffReaderOptions {
    MaxValueBytes = 64 * 1024 * 1024,
    MaxDecodedBytes = 256 * 1024 * 1024,
    MaxPages = 1000
});

reader.ReadHeader();
for (var count = reader.ReadImageFileDirectory(); count != 0; count = reader.ReadImageFileDirectory()) {
    ITag? tag;
    while ((tag = reader.ReadTag()) != null) {
        object? value;
        while ((value = reader.ReadValue()) != null) {
            // ASCII -> string, RATIONAL/SRATIONAL -> TiffRational,
            // known tags -> CLR enums, other numeric values -> raw values.
        }
    }
}
```

`ReadData(handle, count)` returns the raw block. For a compressed strip or tile, use the
codec-aware overload and pass the tag values the codec needs:

```csharp
var context = new TiffCodecContext(reader.ByteOrder, tagValues);
var pixels = reader.ReadData(handle, compressedByteCount, TiffCompression.LZW, context);
```

## Documentation map

| Page | What it covers |
|---|---|
| [Architecture](./architecture.md) | Layers, reader state machine, writer pipeline, codec layer, release pipeline |
| [TIFF data model](./data-model.md) | Header, IFD, entry layout, inline/out-of-line values, data types |
| [Reader](./reader.md) | Call order, values, handles, pixel blocks, codecs, options, errors |
| [Writer](./writer.md) | Header/data/IFD pipeline, multi-page files, compressed blocks, limits |
| [Codecs](./codecs.md) | Built-in PackBits/LZW/Deflate, predictors, codec registries, custom types |
| [Safety and limits](./safety.md) | Validation layers, threat model, limits, error taxonomy |
| [Migration](./migration.md) | Breaking changes, 2.1 additions and before/after code |
| [API reference](./api.md) | Public types, members, options and exceptions |

## What 2.1 adds

| Area | Changes |
|---|---|
| **Compression** | Built-in PackBits, LZW and zlib-wrapped Deflate codecs; `NONE` pass-through |
| **Predictor** | Integer horizontal predictor (`Predictor = 2`) for 8/16/32-bit samples, contiguous and planar |
| **Extensibility** | `ITiffCompressionCodec`/`TiffCodecRegistry`, `ITiffValueCodec`/`TiffValueCodecRegistry`, public `TiffTagRegistry` |
| **Safety** | `MaxDecodedBytes` (256 MiB by default) bounds decompressed blocks; codecs reject malformed streams |
| **API** | Compression-aware `ReadData`/`WriteData` overloads, `TiffCodecContext`, reader/writer `ByteOrder` and codec registries |
| **Tooling** | Codec round-trip, known-vector, fuzz, Deflate and GDI+ LZW interop tests; codec benchmarks |

## What 2.0 added

| Area | Changes |
|---|---|
| **Correctness** | `T6Options` fixed to 293; `TIFF_IFD` is unsigned; header/version validation; IFD cycles and truncated data are rejected |
| **Safety** | Bounds checks, `MaxValueBytes`, `MaxPages`, checked counts, one bounded value block per tag |
| **I/O** | Little- and big-endian; `BaseOffset` for embedded TIFF; write at a non-zero stream position |
| **API** | `ITagInfo`, `ITagValues`, public `TiffDataType`, `TiffRational`, `TiffFormatException`, reader/writer options |
| **Ownership** | `LeaveOpen` defaults to `true`; writer no longer disposes caller `TiffHandle` objects |
| **Tooling** | Multi-target build, xUnit tests, golden/fuzz/interop tests, BenchmarkDotNet, CI, version verification, NuGet trusted publishing |

## Design principles

* **Low level, not an image library.** Tags, IFDs and raw strips/tiles are exposed as they are in TIFF.
* **Compression is explicit.** Raw `ReadData`/`WriteData` stay raw; compression is selected per call.
* **Forward-only and non-cached.** The reader enforces a deterministic call order and does not
  build an in-memory TIFF tree.
* **Explicit ownership.** The caller owns the stream and the handles unless `LeaveOpen = false`.
* **Relative offsets.** Every offset is relative to the TIFF header. `BaseOffset` moves the
  header inside a larger container.
* **Bounded work.** All tag values are checked against `MaxValueBytes`, decompressed blocks against
  `MaxDecodedBytes`, and the IFD chain against `MaxPages`; all offsets and counts are validated
  before use.
* **No hidden dependencies.** The library has no runtime NuGet dependencies.

## Limitations

* Classic TIFF only. BigTIFF (version 43) is rejected with `NotSupportedException`.
* JPEG/OJPEG, JPEG2000, JBIG, CCITT fax and vendor-specific compression schemes are not built in;
  they can be supplied through `ITiffCompressionCodec`.
* The floating-point predictor (`Predictor = 3`) is not implemented.
* No pixel decoding, color conversion, EXIF/GPS object model or rendering.
* Input/output streams must be readable/writable and seekable.
* Classic TIFF offsets are 32-bit: data beyond 4 GiB is not addressable.

## Links

* [NuGet package](https://www.nuget.org/packages/Saraff.Tiff/)
* [GitHub repository](https://github.com/saraff-9EB1047A4BEB4cef8506B29BA325BD5A/Saraff.Tiff.NET)
* [CI workflow](https://github.com/saraff-9EB1047A4BEB4cef8506B29BA325BD5A/Saraff.Tiff.NET/actions/workflows/ci.yml)
