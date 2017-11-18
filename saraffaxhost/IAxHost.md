[All products](../) / [Saraff.AxHost.NET](./index.md)
# IAxHost Interface
## Syntax
```c#
[ComVisible(true)]
[Guid("3A0972A6-BF82-440A-8097-D0B8F802B5C8")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IAxHost:IDisposable
```
## Methods
```c#
/// <summary>
/// Создает и возвращает описание метода.
/// </summary>
/// <param name="methodName">Имя метода.</param>
/// <returns>Описание метода.</returns>
[DispId(0x60020003)]
[Description("Создает и возвращает описание метода.")]
MethodDescriptor CreateMethodDescriptor(string methodName);
```

```c#
/// <summary>
/// Выполняет указанный метод и возвращает результат.
/// </summary>
/// <param name="methodDescriptor">Описание метода.</param>
/// <returns>Результат.</returns>
[DispId(0x60020004)]
[Description("Выполняет указанный метод и возвращает результат.")]
object PerformMethod(MethodDescriptor methodDescriptor);
```

```c#
/// <summary>
/// Добавляет параметр компонента.
/// </summary>
/// <param name="param">Значение параметра.</param>
[DispId(0x60020005)]
[Description("Добавляет параметр компонента.")]
void AddComponentParameter(object param);
```

```c#
/// <summary>
/// Загружает и инициализирует компонент.
/// </summary>
[DispId(0x60020006)]
[Description("Загружает и инициализирует компонент, указанный в ApplicationTypeName.")]
void Load();
```

```c#
/// <summary>
/// Освобождает ресурсы занятые компонентом.
/// </summary>
[DispId(0x60020007)]
[Description("Освобождает ресурсы занятые компонентом.")]
void Dispose();
```
## Properties
```c#
/// <summary>
/// Возвращает или устанавливает имя пользовательского элемента управления.
/// </summary>
/// <remarks>Имя имеет формат: "имя файла сборки"!"полное имя элемента управления"</remarks>
/// <example>MyAssembly.dll!MyNamespace.MyControl</example>
[DispId(0x60020001)]
string ApplicationTypeName {
    [Description("Возвращает или устанавливает имя пользовательского элемента управления.")]
    get;
    set;
}
```

```c#
/// <summary>
/// Возвращает или устанавливает рабочий каталог.
/// </summary>
[DispId(0x60020002)]
string WorkingDirectory {
    [Description("Возвращает или устанавливает рабочий каталог.")]
    get;
    set;
}
```
## Remarks
Интерфейс компонента хостинга для пользовательских элементов управления в приложениях c неуправляемым кодом.