# Saraff.Twain.NET Dependency Injection Samples
Download [Saraff.Twain.DependencyInjectionSamples.zip](https://goo.gl/1iGKug) samples

```c#
// Define a IStreamProvider Service class
internal sealed class _StreamProvider:Component, IStreamProvider {

    public Stream GetStream() {
        return new FileStream(Path.GetTempFileName(),FileMode.Create,FileAccess.ReadWrite,FileShare.Read,64 * 1024,FileOptions.DeleteOnClose);
    }
}
```

```c#
// Define a IImageHandler Service class
internal sealed class _DibImageHandler:Component, IImageHandler {
    private const int BufferSize = 256 * 1024;

    public Stream PtrToStream(IntPtr ptr,IStreamProvider provider) {
        var _stream = provider?.GetStream() ?? new MemoryStream();
        var _writer = new BinaryWriter(_stream);

        int _size = _DibImageHandler.GlobalSize(_DibImageHandler.GlobalHandle(ptr));

        #region Write BITMAPFILEHEADER to stream

        BITMAPINFOHEADER _header = Marshal.PtrToStructure(ptr,typeof(BITMAPINFOHEADER)) as BITMAPINFOHEADER;

        _writer.Write((ushort)0x4d42);
        _writer.Write(14 + _size);
        _writer.Write(0);
        _writer.Write(14 + _header.biSize + (_header.ClrUsed << 2));

        #endregion

        byte[] _buffer = new byte[_DibImageHandler.BufferSize];

        #region  Copy data to stream

        for(int _offset = 0, _len = 0; _offset < _size; _offset += _len) {
            _len = Math.Min(_DibImageHandler.BufferSize,_size - _offset);
            Marshal.Copy((IntPtr)(ptr.ToInt64() + _offset),_buffer,0,_len);
            _writer.Write(_buffer,0,_len);
        }

        #endregion

        return _stream;
    }

    // ...

}
```

```c#
// Bind a Services
using IoC = Saraff.IoC;

// ...

[assembly: IoC.BindService(typeof(IStreamProvider),typeof(_StreamProvider))]
[assembly: IoC.BindService(typeof(IImageHandler),typeof(_DibImageHandler))]
```

```c#
// Create the IoC Container, Load bindings, Create instance of the Twain32 class and Acquire image.
using IoC = Saraff.IoC;

//...

_container = new IoC.ServiceContainer();
_container.Load(Assembly.GetExecutingAssembly());

// ...

_twain32 = _container.CreateInstance<Twain32>();

// ...

_twain32.Acquire();
```
