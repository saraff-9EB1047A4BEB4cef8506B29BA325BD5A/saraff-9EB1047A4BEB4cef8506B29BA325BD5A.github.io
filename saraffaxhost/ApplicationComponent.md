[All products](../) / [Saraff.AxHost.NET](./index.md)
# ApplicationComponent Class
## Syntax
```c#
public class ApplicationComponent:Component
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="ApplicationComponent"/> class.
/// </summary>
public ApplicationComponent()
```
## Methods
```c#
/// <summary>
/// Выполняет инициализацию пользовательского элемента управления.
/// </summary>
/// <param name="args">Коллекция параметров.</param>
protected virtual void Construct(ReadOnlyCollection<object> args)
```
## Remarks
Базовый класс для пользовательских компонентов, требующих хостинг в приложениях c неуправляемым кодом.
The base class for custom components that require hosting in unmanaged code.
## Examples
```c#
[ApplicationComponent]
public sealed partial class ScanComponent:ApplicationComponent {

    public ScanComponent() {
        InitializeComponent();
    }

    protected override void Construct(ReadOnlyCollection<object> args) {
        try {
            this._twain32.OpenDSM();
            if(args!=null&&args.Count>0) {
                this._helper=UploadHelper.Create(args[0](0).ToString());
            } else {
                throw new InvalidOperationException("TwainHandler URI not set.");
            }
        } catch(Exception ex) {
            Debug.WriteLine(string.Format("{1}: {2}{0}{3}{0}", Environment.NewLine, ex.GetType().Name, ex.Message, ex.StackTrace), "ERROR");
        }
        base.Construct(args);
    }

    // ...

    [ApplicationProcessed]
    public void LoadDS(int index) {
        // ...
    }

    [ApplicationProcessed]
    public void Acquire() {
        // ...
    }

    // ...

    [ApplicationProcessed]
    public event EventHandler Uploading;

    [ApplicationProcessed]
    public event EventHandler Uploaded;

    // ...

}
```
