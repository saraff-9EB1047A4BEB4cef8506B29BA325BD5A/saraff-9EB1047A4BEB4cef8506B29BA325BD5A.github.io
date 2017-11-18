[All products](../) / [Saraff.AxHost.NET](./index.md)
# ApplicationProcessedAttribute Class
## Syntax
```c#
[AttributeUsage(AttributeTargets.Event)]
public sealed class ApplicationProcessedAttribute:Attribute
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="ApplicationProcessedAttribute"/> class.
/// </summary>
public ApplicationProcessedAttribute()
```
## Remarks
Указывает на возможности передачи вызовов из класса в приложение c неуправляемым кодом и обратно.
Indicates that a method may be called from unmanaged code or a event may be processed in unmanaged code.
## Examples
**Пример вызова метода (a example of the method call):**

C#
```c#
[ApplicationProcessed]
public void Acquire() {
    // ...
}
```

javascript
```javascript
function __Acquire() {
    AxHost.PerformMethod(AxHost.CreateMethodDescriptor("Acquire"));
}
```

**Пример обработки события (a example of the event processing):**

C#
```c#
[ApplicationProcessed]
public event EventHandler Uploading;
```

javascript
```javascript
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
    }
  }
</script>
```