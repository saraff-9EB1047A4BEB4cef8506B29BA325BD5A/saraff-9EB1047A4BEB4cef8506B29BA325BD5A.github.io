[All products](../) / [Saraff.Tiff.NET](./index.md)
# Reader
```c#
using(var _stream=File.Open("sample.tif",FileMode.Open)) {
    var _reader=TiffReader.Create(_stream);

    _reader.ReadHeader(); // Read Header

    // Read Image File Directories
    for(var _count=_reader.ReadImageFileDirectory(); _count!=0; _count=_reader.ReadImageFileDirectory()) {
        Console.WriteLine("ImageFileDirectory: {0} tags.",_count);
        var _dict=new Dictionary<TiffTags,Collection<object>>();

        // Read Tags
        for(ITag _tag=_reader.ReadTag(); _tag!=null; _tag=_reader.ReadTag()) {
            Console.Write("{0}: { { ",_tag.TagId);
            _dict.Add(_tag.TagId,new Collection<object>());
            switch(_tag.TagId) {
                case TiffTags.StripOffsets:
                    // Read Values of Tag
                    for(object _value=_reader.ReadHandle(); _value!=null; _value=_reader.ReadHandle()) {
                        Console.Write("{0} ",_value);
                        _dict[_tag.TagId].Add(_value);
                    }
                    break;
                default:
                    // Read Values of Tag
                    for(object _value=_reader.ReadValue(); _value!=null; _value=_reader.ReadValue()) {
                        Console.Write("{0} ",(_value is ulong)?((float)((ulong)_value&0xffffffff)/(float)((ulong)_value>>32)):_value);
                        _dict[_tag.TagId].Add(_value);
                    }
                    break;
            }
            Console.WriteLine("}");
        }

        // Read Strips
        for(int i=0; i<_dict[TiffTags.StripOffsets].Count; i++) {
            Console.WriteLine("Strip {0}: ",i);
            var _data=_reader.ReadData((TiffHandle)_dict[TiffTags.StripOffsets][i],Convert.ToInt64(_dict[TiffTags.StripByteCounts][i]));
            for(int ii=0; ii<_data.Length; ii++) {
                // Show Data
            }
        }
    }
}
```

**Example of the read a TIFF file**
```
ImageFileDirectory: 14 tags.
ImageWidth: { 150 }
ImageLength: { 160 }
BitsPerSample: { 8 8 8 }
Compression: { NONE }
PhotometricInterpretation: { RGB }
StripOffsets: { 00000008 0000119C 00002330 000034C4 00004658 000057EC 00006980 00007B14 00008CA8 00009E3C 0000AFD0 0000C164 0000D2F8 0000E48C 0000F620 000107B4 }
SamplesPerPixel: { 3 }
RowsPerStrip: { 10 }
StripByteCounts: { 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 4500 }
XResolution: { 300 }
YResolution: { 300 }
ResolutionUnit: { INCH }
Software: { S A R A F F   S O F T W A R E }
Copyright: { ( c )   S A R A F F   2 0 1 4 }


Strip 0: 

0000: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
0010: 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 
...

Strip 1: 

0000: 11 00 00 11 00 00 11 00 00 11 00 00 11 00 00 11 
0010: 00 00 11 00 00 11 00 00 11 00 00 11 00 00 11 00 
...
```
[Download File](./content/Home_1.txt)