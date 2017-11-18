[All products](../) / [Saraff.AxHost.NET](./index.md)
# ApplicationComponentAttribute Class
## Syntax
```c#
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
public sealed class ApplicationComponentAttribute:Attribute
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="ApplicationComponentAttribute"/> class.
/// </summary>
public ApplicationComponentAttribute()
```
## Remarks
Указывает на возможность предоставления компоненту хостинга в приложениях c неуправляемым кодом.
Indicates that the component require hosting on applications with unmanaged code.
## Examples
```c#
[ApplicationComponent]
public sealed partial class ScanComponent:ApplicationComponent {
    // ...
}
```
