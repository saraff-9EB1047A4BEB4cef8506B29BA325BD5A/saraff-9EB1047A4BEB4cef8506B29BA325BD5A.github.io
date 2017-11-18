[All products](../) / [Saraff.AxHost.NET](./index.md)
# IAxHostEvents Interface
## Syntax
```c#
[ComVisible(true)]
[Guid("46E1F9FD-E9B0-4F07-AACA-9D62F35D9F47")]
[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
public interface IAxHostEvents
```
## Methods
```c#
/// <summary>
/// Возникает в момент, когда необходимо обработать одно из событий пользовательского элемента управления.
/// </summary>
/// <param name="eventId">Описание события.</param>
[DispId(0x60020001)]
[Description("Возникает в момент, когда необходимо обработать одно из событий пользовательского элемента управления.")]
void FireEvent(EventDescriptor eventId);
```
## Remarks
Исходящий интерфейс компонента хостинга для пользовательских элементов управления в приложениях c неуправляемым кодом.