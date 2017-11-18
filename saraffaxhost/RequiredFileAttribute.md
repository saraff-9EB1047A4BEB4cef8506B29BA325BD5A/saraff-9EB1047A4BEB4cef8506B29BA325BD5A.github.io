[All products](../) / [Saraff.AxHost.NET](./index.md)
# RequiredFileAttribute Class
## Syntax
```c#
[AttributeUsage(AttributeTargets.Class)]
public sealed class RequiredFileAttribute:Attribute
```
## Constructors
```c#
/// <summary>
/// Initializes a new instance of the <see cref="RequiredFileAttribute"/> class.
/// </summary>
/// <param name="fileName">Имя файла.</param>
public RequiredFileAttribute(string filename)
```
## Properties
```c#
/// <summary>
/// Возвращает имя файла.
/// </summary>
/// <value>Имя файла.</value>
public string FileName {get;}
```
## Remarks
Указывает на необходимость присутствия указанного файла в том же каталоге, что и сборка класса.
Indicates that the specified file is required for the application.
## Examples
```c#
[assembly: RequiredFile("Saraff.Twain.dll")]
[assembly: RequiredFile("Saraff.Tiff.dll")]
```
