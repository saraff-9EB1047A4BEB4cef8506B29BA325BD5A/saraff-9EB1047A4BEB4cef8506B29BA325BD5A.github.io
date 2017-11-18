[All products](../) / [Saraff.AxHost.NET](./index.md)
# Обработка событий. Event Processing.
Для обработки событий необходимо (_for the event processing must be_):
* обработать событие FireEvent (исходящий интерфейс IAxHostEvents) (_processed a FireEvent event_)
* извлечь дескриптор события (_get a handle of event_)
* извлечь имя события (_get a name of event_)
* при необходимости извлечь значения параметров события (_if necessary, get a values of arguments of event_)
* при необходимости установить значения параметров события (_if necessary, set a values of arguments of event_)
Событие должно быть с типом делегата System.EventHandler и отмечено атрибутом ApplicationProcessedAttribute (_The event should be a type System.EventHandler and marked a ApplicationProcessedAttribute_). 
Параметры события передаются через свойства класса, унаследованного от System.EventArgs (_the parameters transferred via the event properties of a class derived from the System.EventArgs_).

**Объявление и генерация события (C#)**
```c#
[ApplicationProcessed]
public event EventHandler Uploaded;
```

```c#
if(this.Uploaded!=null) {
    this.Uploaded(this, new UploadEventArgs(_name));
}
```

```c#
public sealed class UploadEventArgs:EventArgs {

    internal UploadEventArgs(string name) {
        this.Name=name;
    }

    public string Name {
        get;
        private set;
    }
}
```

**Обработка события (JavaScript)**
```javascript
<object id="AxHost" 
        name="AxHost"
        classid="clsid:7067A712-CDFD-4780-B6C0-B8F68A9BA84F"
        codebase="Saraff.AxHost.cab"></object>
<script language="javascript" type="text/javascript">
  function AxHost::FireEvent(eventId) {
    switch(eventId.EventName){
      // ...
      case "Uploaded":
        Uploaded(eventId.GetParam("Name"));
        break;
      // ...
    }
  }
</script>
```

![](./content/Saraff.AxHost_SEQ.jpg)
[download fullsize image](./content/Saraff.AxHost_SEQ.jpg)
