[All products](../) / [Saraff.Tiff.NET](./index.md)
# Safety and limits

TIFF files are often untrusted input. The 2.x reader is designed to reject malformed data
before it can drive an unbounded allocation or an out-of-range seek. It is still a low-level
library: it validates the TIFF structure, not the meaning of your application tags.

## Threat model

The reader assumes that the bytes may be hostile. The main risks are:

* huge tag counts or values causing excessive allocation;
* offsets pointing outside the file;
* cyclic or very long IFD chains;
* truncated values and silent partial reads;
* integer overflow in `offset + size` or `count * itemSize`;
* malformed data types and zero-count tags.

## Validation layers

```mermaid
flowchart LR
    Input["Untrusted TIFF bytes"] --> Header["Header checks<br/>magic, version 42, first offset"]
    Header --> IFD["IFD checks<br/>offset, count*12+4, cycle, MaxPages"]
    IFD --> Entry["Entry checks<br/>known type, count > 0, MaxValueBytes"]
    Entry --> Value["Value checks<br/>offset + size within stream"]
    Value --> Data["ReadData checks<br/>handle open, count within stream"]
    Data --> Result["Typed values or TiffFormatException"]
```

| Layer | Checks |
|---|---|
| Header | Byte-order marker `II`/`MM`, version `42`, first IFD offset |
| IFD | Offset >= 8, tag count fits into the stream, cycle detection, `MaxPages` |
| Entry | Data type is known, count is non-zero, `count * itemSize <= MaxValueBytes` |
| Value | Out-of-line offset >= 8 and `offset + size <= stream length` |
| Handle data | Handle is open, count is non-negative and fits into `int`, range is valid |
| Writer | Owner is open, tag ids are unique, values are non-empty, offsets fit into 32 bits |

## Limits

| Option | Default | Applies to |
|---|---:|---|
| `TiffReaderOptions.MaxValueBytes` | 64 MiB | One buffered tag value |
| `TiffReaderOptions.MaxPages` | 1,000,000 | IFD chain length |
| `TiffReaderOptions.BaseOffset` | 0 | Header position in the stream |
| `TiffWriterOptions.MaxValueBytes` | 64 MiB | One buffered tag value |
| `TiffWriterOptions.BaseOffset` | 0 | Header position in the stream |

The limits are deliberately configurable. For a server processing arbitrary uploads, consider
lower values such as `MaxValueBytes = 8 * 1024 * 1024` and `MaxPages = 64`. For trusted
scientific or medical files, the defaults are usually sufficient.

## Error taxonomy

| Exception | Meaning | Recovery |
|---|---|---|
| `TiffFormatException` | Malformed, truncated, cyclic or out-of-range TIFF data; limit exceeded | Dispose the reader; fix or reject the file |
| `NotSupportedException` | BigTIFF (`version 43`), data beyond 4 GiB when writing | Convert/reject the file |
| `InvalidOperationException` | Reader/writer method called in the wrong state | Fix the call sequence |
| `ArgumentException` | Closed handle, invalid tag set, duplicate tag, unsupported type | Fix the caller data |
| `ArgumentOutOfRangeException` | Negative count or invalid option | Fix the argument |
| `ObjectDisposedException` | Instance used after `Dispose()` | Create a new instance |
| `IOException` | The underlying stream failed | Handle at the application boundary |

## What is checked

* Every multi-byte primitive is read with an explicit byte order.
* `Stream.Read` is looped until the requested block is complete.
* `offset + size` and `count * itemSize` are checked in 64-bit arithmetic before allocation.
* An IFD cycle is detected with a set of visited offsets.
* Tag values are buffered in one block, bounded by `MaxValueBytes`.
* Handles used by `ReadData` must be open and inside the stream.
* The writer rejects duplicate tags, empty values and values that do not match their count.
* Package, assembly and file versions are checked in CI and before publishing.

## What is not checked

The library does not validate:

* whether the image data is decodable under the declared compression;
* whether the pixel count matches `ImageWidth * ImageLength`;
* whether tag semantics are consistent (for example, whether `StripOffsets` covers all strips);
* whether the value of an application-specific tag is meaningful;
* EXIF/GPS/GeoTIFF semantic constraints.

Those checks belong to the application or to a higher-level image library.

## BigTIFF and 4 GiB

Classic TIFF uses 32-bit offsets. The reader rejects BigTIFF with `NotSupportedException`.
The writer rejects offsets beyond 4 GiB. This is a format limit, not a performance limit; use
a BigTIFF-capable library if you need larger files.

## Testing

The repository contains:

* unit tests for inline/out-of-line values, ASCII, rationals, handles and multi-page files;
* golden little-endian and big-endian files built independently of the writer;
* randomized round-trip tests;
* mutation fuzzing and random-byte fuzzing with bounded exceptions;
* a GDI+ interop test on .NET Framework;
* BenchmarkDotNet read/write benchmarks.

The fuzz tests accept only `TiffFormatException` and `NotSupportedException` for hostile
input; any other unexpected exception is a test failure.

## Recommended usage for untrusted input

```csharp
using var stream = File.OpenRead(uploadPath);
using var reader = TiffReader.Create(stream, new TiffReaderOptions {
    LeaveOpen = true,
    MaxValueBytes = 8 * 1024 * 1024,
    MaxPages = 64
});
```

Then:

1. Read the header and directories inside a `try/catch (TiffFormatException)`.
2. Keep `ReadData` counts under an application limit before allocating pixel buffers.
3. Do not assume that a tag value is meaningful just because it parsed.
4. Treat compressed pixel data as opaque bytes; the library does not decompress it.
