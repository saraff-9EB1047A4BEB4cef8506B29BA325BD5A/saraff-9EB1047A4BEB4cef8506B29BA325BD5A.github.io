[All products](../) / [Saraff.AxHost.NET](./index.md)
# Вызов методов. Method Invocation
Для вызова метода необходимо (_to call a method, you must_):
* получить дескриптор метода (метод IAxHost.CreateMethodDescriptor) (_get a handle method_)
* добавить необходимое количество параметров с указанием их типов (метод IMethodDescriptor.AddParam) (_add the required number of parameters with their types_)
* вызвать метод (метод IAxHost.PerformMethod) (_call the method_)
Вызываемый метод должен быть отмечен атрибутом ApplicationProcessedAttribute (_the called method must be marked a ApplicationProcessedAttribute_).
Допустимые типы параметров перечислены в ParamType (_valid types of parameters listed in a  ParamType_).

```c#
// Типы параметров. Types of parameters.
[ComVisible(true)]
[Guid("CB73C100-5462-46DC-AFF1-E7B37845568E")]
public enum ParamType {
    Int=1, // Целое число.
    Double=2, // Числос плавающей точкой.
    String=3, // Строка.
    Char=4, // Символ.
    Bool=5, // Булево значение.
    DateTime=6, // Дата/время.
    Unknown=7 // Неизвесный тип.
}
```

## Вызов метода без параметров. Invoke a method without parameters.
**Объявление метода (C#) (_a method declaration_)**
```c#
[ApplicationProcessed]
public int GetSourcesCount() {
    // ...
}
```

**Вызов метода (JavaScript) (_calling_)**
```javascript
_count=AxHost.PerformMethod(AxHost.CreateMethodDescriptor("GetSourcesCount"));
```

## Вызов метода с параметрами. Invoke a method with parameters.
**Объявление метода (C#) (_a method declaration_)**
```c#
[ApplicationProcessed]
public string GetSourceProductName(int index) {
    // ...
}
```

**Вызов метода (JavaScript) (_calling_)**
```javascript
_method = AxHost.CreateMethodDescriptor("GetSourceProductName");
_method.AddParam(sourceIndex, 1);
_name = AxHost.PerformMethod(_method);
```

![](./content/Saraff.AxHost_SEQ.jpg)
[download fullsize image](./content/Saraff.AxHost_SEQ.jpg)
