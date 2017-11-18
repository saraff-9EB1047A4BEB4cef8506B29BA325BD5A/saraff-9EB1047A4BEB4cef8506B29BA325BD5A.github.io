[All products](../) / [Saraff.AxHost.NET](./index.md)
# ApplicationControlAttribute Class
## Syntax
```c#
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
public sealed class ApplicationControlAttribute:Attribute
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="ApplicationControlAttribute"/> class.
/// </summary>
public ApplicationControlAttribute()
```
## Properties
```c#
/// <summary>
/// Возвращает или устанавливает заголовок окна.
/// </summary>
public string Title {get; set;}
```

```c#
/// <summary>
/// Возвращает или устанавливает ширину окна.
/// </summary>
public int Width {get; set;}
```

```c#
/// <summary>
/// Возвращает или устанавливает высоту окна.
/// </summary>
public int Height {get; set;}
```
## Remarks
Указывает на возможность предоставления классу хостинга в приложениях c неуправляемым кодом.
Indicates that the control require hosting on applications with unmanaged code. 
## Examples
```c#
[ApplicationControl(Width=640,Height=480)]
public sealed partial class ScanControl:ApplicationControl {
    // ...
}
```
