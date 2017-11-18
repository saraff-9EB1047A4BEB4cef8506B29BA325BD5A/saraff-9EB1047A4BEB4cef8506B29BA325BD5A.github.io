[All products](../) / [Saraff.IoC](./index.md)
# Factory
```c#
using(var _container = new IoC.ServiceContainer()) {

    // ...

    var _obj2 = _container.CreateInstance(typeof(MyCustomClass));
    // or
    _obj2 = _container.CreateInstance<MyCustomClass>();

    // If the type is derived from a System.ComponentModel.Component
    // that instance will be placed into a container
    var _obj3 = _container.CreateInstance(typeof(MyCustomComponent));
    // or
    _obj3=_container.CreateInstance<MyCustomComponent>();

    // ...

}
```
Also you can directly set some values of constructor parameters
```c#
internal sealed class MyCustomClass {

    [IoC.ServiceRequired]
    public MyCustomClass(ArgType1 arg1, IService1 svc1, ArgType2 arg2) {
        // ...
    }

    // ...
}

// ...

var _obj = _container.CreateInstance<MyCustomClass>(x => x("arg1",value1), x => x("arg2",value2));
```
