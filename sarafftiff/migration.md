[All products](../) / [Saraff.Tiff.NET](./index.md)
# Migration

## Migrating from 2.0 to 2.1

Version 2.1 is additive for existing 2.0 code. Raw `ReadData`/`WriteData` calls keep their
behavior; compression is opt-in through new overloads and registries.

| Area | 2.0 | 2.1 |
|---|---|---|
| Package/assembly version | package `2.0.0`, assembly `2.0.0.0` | package `2.1.0`, assembly `2.1.0.0` |
| Compression | not implemented | PackBits, LZW, zlib-wrapped Deflate, `NONE` |
| Predictor | n/a | integer horizontal predictor (`Predictor = 2`) |
| Custom codecs | not possible | `ITiffCompressionCodec`, `ITiffValueCodec` |
| Custom tag enums | internal registry | public `TiffTagAttribute` and `TiffTagRegistry` |
| Decoded data limit | n/a | `TiffReaderOptions.MaxDecodedBytes` |
| Reader/writer properties | `TiffVersion` | also `ByteOrder`, `Codecs`, `ValueCodecs` |

### Registries are explicit

`new TiffCodecRegistry()` now contains the built-in codecs. If 2.0-era code relied on a new
registry being empty, use `TiffCodecRegistry.CreateEmpty()` and register only what is needed.
`TiffCodecRegistry.Default` is process-wide and mutable; per-reader/per-writer registries are
available through options.

```csharp
var registry = TiffCodecRegistry.CreateEmpty();
registry.Register(new MyCodec());

using var reader = TiffReader.Create(stream, new TiffReaderOptions {
    Codecs = registry,
    MaxDecodedBytes = 64 * 1024 * 1024
});
```

### Raw vs compressed blocks

The existing overloads are unchanged:

```csharp
var raw = reader.ReadData(handle, byteCount);
var handle = writer.WriteData(raw);
```

Use the codec-aware overloads for compressed strips and tiles. The count is the compressed
byte count:

```csharp
var context = new TiffCodecContext(reader.ByteOrder, tagValues);
var pixels = reader.ReadData(handle, compressedByteCount, TiffCompression.LZW, context);
```

See [Codecs](./codecs.md) for required predictor tags, built-in codecs and custom registration.

### Still not implemented

JPEG/OJPEG, JPEG2000, JBIG, CCITT RLE/Group 3/Group 4 and the floating-point predictor
(`Predictor = 3`) remain outside the built-in set. Unsupported compression throws
`NotSupportedException` unless a codec is registered.

## Migrating from 1.x to 2.0

Version 2.0 is a major update. The low-level model is the same, but value semantics, ownership,
target frameworks and the public contracts around tags changed.

## At a glance

| Area | 1.0.1.401 | 2.0.0 |
|---|---|---|
| Target frameworks | .NET 2.0 and .NET 4.0 projects | `netstandard2.0`, `net48`, `net10.0` |
| Package/assembly version | not synchronized | package `2.0.0`, assembly `2.0.0.0` |
| ASCII | `char` per value | one `string` per ASCII tag |
| RATIONAL/SRATIONAL | raw `ulong`/`long` | `TiffRational` |
| `TIFF_IFD` | signed `int` | unsigned `uint` |
| `T6Options` | incorrectly `292` | `293` |
| Malformed input | often accepted or produced zeros | `TiffFormatException` |
| Limits | none | `MaxValueBytes`, `MaxPages` |
| Stream ownership | reader disposed the stream | `LeaveOpen = true` by default |
| Handles | writer disposed supplied handles | writer never disposes caller handles |
| Tag metadata | only `TagId` | `ITagInfo` with type, item type and count |
| Writing tags | internal `_Tag` base class | `ITagValues` / `Tag<T>` |
| Byte order | little-endian only | little-endian and big-endian |
| Embedded TIFF | not supported | `BaseOffset` |
| BigTIFF | not detected | rejected with `NotSupportedException` |

## 1. Choose the target framework

The broken .NET 2.0 project was removed. Use the package from `netstandard2.0` on modern .NET,
or the `net48` asset on .NET Framework 4.8.

```xml
<PackageReference Include="Saraff.Tiff" Version="2.1.0" />
```

## 2. Update the reader loop

The call order is unchanged, but values have new types and tags expose metadata.

**Before**

```csharp
using (var stream = File.OpenRead("sample.tif")) {
    var reader = TiffReader.Create(stream);
    reader.ReadHeader();

    for (var count = reader.ReadImageFileDirectory(); count != 0;
         count = reader.ReadImageFileDirectory()) {
        for (ITag tag = reader.ReadTag(); tag != null; tag = reader.ReadTag()) {
            object value;
            while ((value = reader.ReadValue()) != null) {
                if (value is ulong raw) {
                    var rational = ((float)(raw & 0xffffffff)) / (float)(raw >> 32);
                }
            }
        }
    }
}
```

**After**

```csharp
using var stream = File.OpenRead("sample.tif");
using var reader = TiffReader.Create(stream, new TiffReaderOptions {
    LeaveOpen = true,
    MaxValueBytes = 64 * 1024 * 1024,
    MaxPages = 1000
});

reader.ReadHeader();
for (var count = reader.ReadImageFileDirectory(); count != 0;
     count = reader.ReadImageFileDirectory()) {
    ITag? tag;
    while ((tag = reader.ReadTag()) != null) {
        var info = (ITagInfo)tag;
        object? value;
        while ((value = reader.ReadValue()) != null) {
            if (value is string text) {
                // ASCII is a single string
            } else if (value is TiffRational rational) {
                // 300/1, no raw bit manipulation
            }
        }
    }
}
```

## 3. Update ownership expectations

The reader and writer no longer dispose the source/target stream by default.

```csharp
// 1.x: the reader disposed the stream
// 2.x: the reader leaves it open
using var reader = TiffReader.Create(stream);

// 2.x: opt in to ownership
using var owningReader = TiffReader.Create(stream, new TiffReaderOptions {
    LeaveOpen = false
});
```

The writer also no longer disposes `TiffHandle` objects passed in tags:

```csharp
var handle = writer.WriteData(strip);
writer.WriteImageFileDirectory(owner, new[] {
    Tag<TiffHandle>.Create(TiffTags.StripOffsets, handle)
});
// handle is still open
```

## 4. Replace raw rational values

**Before**

```csharp
Tag<ulong>.Create(TiffTags.XResolution, (1UL << 32) | 300UL)
```

**After**

```csharp
Tag<TiffRational>.Create(TiffTags.XResolution, new TiffRational(300, 1))
```

Reading `RATIONAL` and `SRATIONAL` now returns `TiffRational` instead of a raw 64-bit value.
Use `Numerator`, `Denominator` or `ToDouble()`.

## 5. Replace character arrays for ASCII

**Before**

```csharp
Tag<char>.Create(TiffTags.Software, "SARAFF SOFTWARE".ToCharArray())
```

**After**

```csharp
Tag<string>.Create(TiffTags.Software, "SARAFF SOFTWARE")
```

`Tag<char>` is still available for byte-oriented ASCII data, but the reader always returns a
`string` for ASCII tags.

## 6. Update custom tag implementations

The public `_Tag` base class was removed. Implement `ITagValues` (and `ITagInfo`) directly, or
use `Tag<T>`.

```csharp
public sealed class RawIfdTag : ITagValues {
    private readonly uint _offset;

    public RawIfdTag(uint offset) {
        this._offset = offset;
    }

    public TiffTags TagId => TiffTags.SubIfd;
    public TiffDataType DataType => TiffDataType.TIFF_IFD;
    public Type ItemType => typeof(uint);
    public int Count => 1;
    public object GetValue(int index) => this._offset;
    public IEnumerable<object> GetValues() => new object[] { this._offset };
}
```

## 7. Handle malformed input explicitly

1.x often returned zeros for truncated data or accepted a missing version. 2.x throws:

```csharp
try {
    reader.ReadHeader();
}
catch (TiffFormatException exception) {
    // reject the file
}
catch (NotSupportedException exception) {
    // BigTIFF or another unsupported feature
}
```

Set application-level limits for hostile input:

```csharp
var options = new TiffReaderOptions {
    MaxValueBytes = 8 * 1024 * 1024,
    MaxPages = 64
};
```

## 8. Embedded TIFF

If the TIFF is inside a larger container, use `BaseOffset` instead of seeking manually.

```csharp
using var reader = TiffReader.Create(containerStream, new TiffReaderOptions {
    BaseOffset = headerOffset
});
```

The writer supports the same option; the header must be written at
`BaseOffset` and all stored offsets remain relative to it.

## 9. Namespace notes

The namespaces did not change in 2.0:

* `Saraff.Tiff` - readers, writers, tags, handles, options, exceptions;
* `Saraff.Tiff.Core` - `TiffTags`, `TiffDataType` and tag enums.

Version 2.1 adds `Saraff.Tiff.Compression` for built-in codecs; codec interfaces and registries
remain in `Saraff.Tiff.Core`.

`TiffDataType` is now public, so consumers can inspect `ITagInfo.DataType` without reflection.

## Migration checklist

- [ ] Replace `net20`/`net40` projects with the `netstandard2.0` or `net48` package.
- [ ] Update ASCII handling from `char` to `string`.
- [ ] Replace raw `ulong`/`long` rationals with `TiffRational`.
- [ ] Update custom tags to `ITagValues`.
- [ ] Decide on stream ownership (`LeaveOpen`).
- [ ] Remove assumptions that the writer disposes handles.
- [ ] Add `try/catch (TiffFormatException)` around untrusted input.
- [ ] Set `MaxValueBytes` and `MaxPages` for untrusted uploads.
- [ ] Use `BaseOffset` for embedded TIFF instead of manual seeks.
- [ ] Re-test against big-endian and truncated files.
- [ ] Set `MaxDecodedBytes` when decoding untrusted compressed blocks.
- [ ] Replace `new TiffCodecRegistry()` used as an empty registry with `CreateEmpty()`.
- [ ] Register custom compression, value or tag codecs explicitly.

See [Safety and limits](./safety.md) for validation details and [Architecture](./architecture.md)
for the new internal pipeline.
