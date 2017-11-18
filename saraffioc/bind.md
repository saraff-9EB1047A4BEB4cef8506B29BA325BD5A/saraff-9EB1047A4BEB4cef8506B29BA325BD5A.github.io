[All products](../) / [Saraff.IoC](./index.md)
# Bind
## Bind to a type of class (metadata)
```c#
// declaring binds
[assembly: IoC.BindService(typeof(IService1),typeof(Service1))] // a type must be inherit from System.ComponentModel.Component
[assembly: IoC.BindService(typeof(IService2),typeof(Service2))] // because it's will be placed into a container
[assembly: IoC.BindService(typeof(IoC.IContextBinder<IService2,MyCustomComponent>),typeof(Service2A))] // Also, you can define a bind for specific classes.
```

```c#
using(var _container = new IoC.ServiceContainer()) {
    // loading binds from assembly metadata
    _container.Load(typeof(Program).Assembly);
    // ...
}
```
## Bind to a type of class (runtime)
```c#
using(var _container = new IoC.ServiceContainer()) {
    // ...
    // adding new bind
    _container.Bind(typeof(IService1),typeof(Core.Service1));
    // or
    _container.Bind<IService1,Core.Service1>();
    // or
    _container.Bind<IoC.IContextBinder<IService1,MyCustomComponent>,Core.Service1>();
    // ...
}
```
## Bind to instance of class
```c#
using(var _container = new IoC.ServiceContainer()) {
    // ...
    var _obj1 = new Core.Service1();
    var _obj1a = new Core.Service1A();

    // adding new bind to object
    _container.Bind(typeof(IService2),_obj1);
    // or
    _container.Bind<IService2>(_obj1);
    // or
    _container.Bind<IoC.IContextBinder<IService2,MyCustomComponent>>(_obj1a);
    // ...
}
```
