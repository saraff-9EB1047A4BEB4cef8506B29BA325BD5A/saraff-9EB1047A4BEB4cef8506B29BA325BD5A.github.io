[All products](../) / [Saraff.IoC](./index.md)
# Injection Patterns
## Constructor Injection
```c#

internal sealed class MyCustomClass {

    [IoC.ServiceRequired]
    public MyCustomClass(IService1 svc1/* , ... */) { // type of arguments of constructor can be is a interface or/and class
    }

    // ...
}
```

```c#
internal sealed class Service2:Component, IService2 {

    [IoC.ServiceRequired]
    public Service2(IService1 svc1/* , ... */) { // type of arguments of a constructor can be is a interface or/and a class

    }

    // ...
}
```
## Property Setter Injection
```c#
internal sealed class MyCustomClass {

    // ...

    [IoC.ServiceRequired]
    public IService1 Svc1 { // type of a property can be only is a interface
        get;
        set;
    }

    // ...

}
```

```c#
internal sealed class Service2:Component, IService2 {

    // ...

    [IoC.ServiceRequired]
    public IService1 Svc1 { // type of a property can be only is a interface
        get;
        set;
    }

    // ...
}
```
## Method Injection
Available for a components that implements a service's interfaces
```c#
[IoC.ProxyRequired] // The Method Injection pattern require uses a proxy for injections
internal sealed class Service2:Component, IService2 {

    // ...

    public TResult MethodA<T1, T2, TResult>(
        T1 val1, 
        [IoC.ServiceRequired]T2 val2, // type of arguments can be is a interface or/and a class
        [IoC.ServiceRequired]IService1 service1, 
        [IoC.ServiceRequired]IService2 service2 = null) {

        // ...

    }

    // ...
}
```

![]({% link saraffioc/content/m1.jpg%})

## IServiceProvider
```c#
internal sealed class Service2:Component, IService2 {

    // ...

    private IService1 Svc2 {
        get {
            return this.GetService(typeof(IService1)) as IService1;
            // if you want get instance for specific class (context binding), use a IoC.IContextBinder<,>
            return this.GetService(typeof(IoC.IContextBinder<IService1,Service2>) as IService1;
            // or
            return this.GetService(typeof(IoC.IContextBinder<,>.MakeGenericType(typeof(IService1),this.GetType())) as IService1;
        }
    }

    // ...

}
```

```c#
using(var _container = new IoC.ServiceContainer()) {

    //...

    var _provider = _container as IServiceProvider;
    var _svc = _provider.GetService(typeof(IService1)) as IService1;

    //...
}
```
