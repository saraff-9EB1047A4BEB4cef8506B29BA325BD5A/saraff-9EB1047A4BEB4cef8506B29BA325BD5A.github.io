[All products](../) / [Saraff.AxHost.NET](./index.md)
# AxHostControl Class
## Syntax
```c#
[ComVisible(true)]
[ProgId("Saraff.AxHost.AxHostControl")]
[Guid("83368D28-D24A-4543-9B1A-6D7B0152C54F")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IAxHost))]
[ComSourceInterfaces(typeof(IAxHostEvents))]
[DefaultProperty("ApplicationControlName")]
[DefaultEvent("FireEvent")]
[Description("Предоставляет хостинг для пользовательских элементов управления в приложениях c неуправляемым кодом.")]
public sealed class AxHostControl:AxHostBaseControl, IAxHost
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="AxHostControl"/> class.
/// </summary>
public AxHostControl()
```
## Methods
```c#
/// <summary>
/// Создает и возвращает описание метода.
/// </summary>
/// <param name="methodName">Имя метода.</param>
/// <returns>Описание метода.</returns>
public MethodDescriptor CreateMethodDescriptor(string methodName)
```

```c#
/// <summary>
/// Выполняет указанный метод и возвращает результат.
/// </summary>
/// <param name="methodDescriptor">Описание метода.</param>
/// <returns>Результат.</returns>
public object PerformMethod(MethodDescriptor methodDescriptor)
```

```c#
/// <summary>
/// Добавляет параметр компонента.
/// </summary>
/// <param name="param">Значение параметра.</param>
public void AddComponentParameter(object param)
```

```c#
/// <summary>
/// Загружает и инициализирует компонент.
/// </summary>
public void Load()
```
## Properties
```c#
/// <summary>
/// Возвращает или устанавливает имя пользовательского элемента управления.
/// </summary>
/// <remarks>Имя имеет формат: "имя файла сборки"!"полное имя элемента управления"</remarks>
/// <example>MyAssembly.dll!MyNamespace.MyControl</example>
[DefaultValue("")]
[Category("Behavior")]
public string ApplicationTypeName {get; set;}
```

```c#
/// <summary>
/// Возвращает или устанавливает рабочий каталог.
/// </summary>
public string WorkingDirectory {get; set;}
```
## Events
```c#
/// <summary>
/// Возникает в момент, когда необходимо обработать одно из событий пользовательского компонента.
/// </summary>
[Category("Action")]
public event AxHostControlFireEventHandler FireEvent;
```
## Remarks
Предоставляет хостинг для пользовательских элементов управления в приложениях c неуправляемым кодом.
Provide hosting by custom controls in unmanaged code.
## Examples
```javascript
<script language="javascript" type="text/javascript">
  function LoadScanControl() {
    try {
      if (AxHost && AxHost.object) {
        req.innerHTML = "";
        AxHost.WorkingDirectory = location.href.substr(0, location.href.lastIndexOf("/"));
        AxHost.ApplicationTypeName = "Saraff.Twain.WebSample.dll!Saraff.Twain.WebSample.ScanControl";
        AxHost.AddComponentParameter(AxHost.WorkingDirectory + "TwainHandler.ashx")
        AxHost.Load();
      }
    } catch (ex) {
      alert(ex.message);
    }
  }
  // ...
  function Uploaded(imageName) {
    StatusBar.innerText = imageName;
    ImageList.insertRow(1).insertCell(0).innerText = imageName;
  }
  // ...
</script>
```

```javascript
<body onload="LoadScanControl()">

  <object id="Object1" 
          name="AxHost" 
          width="640" 
          height="480" 
          classid="clsid:83368D28-D24A-4543-9B1A-6D7B0152C54F" 
          codebase="Saraff.AxHost.cab" ></object>
  <script language="javascript" type="text/javascript">
    function AxHost::FireEvent(eventId) {
      switch(eventId.EventName){
        case "Uploading":
          // ...
          break;
        case "Uploaded":
          Uploaded(eventId.GetParam("Name"));
          break;
        case "Error":
          // ...
          break;
      }
    }
  </script>
</body>
```