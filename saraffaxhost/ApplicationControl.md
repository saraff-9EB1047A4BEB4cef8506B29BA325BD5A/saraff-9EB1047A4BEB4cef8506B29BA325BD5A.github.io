[All products](../) / [Saraff.AxHost.NET](./index.md)
# ApplicationControl Class
## Syntax
```c#
public class ApplicationControl:UserControl
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="ApplicationControl"/> class.
/// </summary>
public ApplicationControl()
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
Базовый класс для пользовательских элементов управления, требующих хостинг в приложениях c неуправляемым кодом.
The base class for a users controls that require hosting in unmanaged code.
## Examples
```c#
[ApplicationControl]
public sealed partial class ScanControl:ApplicationControl {

    protected override void Construct(ReadOnlyCollection<object> args) {
        try {
            this._LoadDSM();
            if(args!=null&&args.Count>0) {
                this._helper=UploadHelper.Create(args[0](0).ToString());
            } else {
                throw new InvalidOperationException("TwainHandler URI not set.");
            }
        } catch(Exception ex) {
            MessageBox.Show(string.Format("{1}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace), ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        base.Construct(args);
    }

    // ...

    [ApplicationProcessed]
    public void _LoadDS() {
        // ...
    }

    [ApplicationProcessed]
    public event EventHandler Uploading;

    [ApplicationProcessed]
    public event EventHandler Uploaded;

}
```
