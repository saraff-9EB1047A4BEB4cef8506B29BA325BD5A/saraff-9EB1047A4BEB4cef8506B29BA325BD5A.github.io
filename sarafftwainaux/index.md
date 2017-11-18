[All products](../)
# Saraff.Twain.Aux
Provides outproc interaction with the [Saraff.Twain.NET](../sarafftwain/). 

```c#
// creating host in external process
internal sealed class Program {

    [STAThread]
    private static void Main(string[] args) {
        using(var _twain32=new Twain32()) {
            _twain32.IsTwain2Enable=true;
            _twain32.ShowUI=false;
            _twain32.OpenDSM();

            TwainExternalProcess.Handler(_twain32);
        }
    }
}
```

```c#
// invoking instance of a Twain32 class, that hosted in external process
private void _Load() {
    var _result = new Collection<DS>();
    var _index = 0;

    TwainExternalProcess.Execute(
        Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location),"Saraff.Twain.Aux_MSIL.exe"),
        twain => {
            for(var i = 0; i<twain.SourcesCount; i++) {
                _result.Add(new DS {
                    Id=i,
                    Identity=twain.GetSourceIdentity(i),
                    IsX86=true,
                    IsTwain2=twain.IsTwain2Supported
                });
            }
            _index=twain.SourceIndex;
        });

    this.dsBindingSource.DataSource=_result;
    this.dsBindingSource.Position=_index;
}
```

Also, you can see:
* [Saraff.Twain.NET Outproc Samples]({% link sarafftwain/samples/outproc.md %})
* [Saraff.Twain.NET Service Samples]({% link sarafftwain/samples/service.md %})
