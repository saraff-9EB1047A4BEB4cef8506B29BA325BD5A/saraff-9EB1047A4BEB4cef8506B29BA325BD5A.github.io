[All products](../) / [Saraff.AxHost.NET](./index.md)
# AxHostComponent Class
## Syntax
```c#
[ComVisible(true)]
[ProgId("Saraff.AxHost.AxHostComponent")]
[Guid("7067A712-CDFD-4780-B6C0-B8F68A9BA84F")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IAxHost))]
[ComSourceInterfaces(typeof(IAxHostEvents))]
[DefaultProperty("ApplicationComponentName")]
[DefaultEvent("FireEvent")]
[Description("Предоставляет хостинг для пользовательских компонентов в приложениях c неуправляемым кодом.")]
public sealed class AxHostComponent:Component, IAxHost
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="AxHostComponent"/> class.
/// </summary>
public AxHostComponent()
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
Предоставляет хостинг для пользовательских компонентов в приложениях c неуправляемым кодом.
Provide hosting by custom components in unmanaged code.
## Examples
```javascript
<script language="javascript" type="text/javascript">
  function LoadScanComponent() {
    try {
      if (AxHost && AxHost.object) {
        req.innerHTML = "";
        AxHost.WorkingDirectory = location.href.substr(0, location.href.lastIndexOf("/"));
        AxHost.ApplicationTypeName = "Saraff.Twain.WebSample.dll!Saraff.Twain.WebSample.ScanComponent";
        AxHost.AddComponentParameter(AxHost.WorkingDirectory + "TwainHandler.ashx")
        AxHost.Load();
        FillDS();
        SelectDS();
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
  function Acquire() {
    AxHost.PerformMethod(AxHost.CreateMethodDescriptor("Acquire"));
  }
  // ...
</script>
```
```javascript
<body onload="LoadScanComponent()">

  <input id="acquireButton" type="button" value="Acquire" onclick="Acquire()"/>

  <object id="AxHost" 
          name="AxHost" 
          classid="clsid:7067A712-CDFD-4780-B6C0-B8F68A9BA84F" 
          codebase="Saraff.AxHost.cab"></object>
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
