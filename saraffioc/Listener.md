[All products](../) / [Saraff.IoC](./index.md)
# Listener of invocations
It can be used for logging, check security permissions, unit-testing with a mock-objects, etc.
Available for a components that uses the `ProxyRequiredAttribute`.

```c#
[IoC.ProxyRequired] // The Listener of invocations require uses a proxy
internal sealed class Service2:Component, IService2 {

    // ...

    public TResult MethodA<T1, T2, TResult>(
        T1 val, 
        [IoC.ServiceRequired]T2 val, // type of arguments can be is a interface or/and a class
        [IoC.ServiceRequired]IService1 service1, 
        [IoC.ServiceRequired]IService2 service2 = null) {

        // ...

    }

    // ...
}
```

You must bind class (or instance of class) that implement the IListener interface.

```c#
internal sealed class _Listener : Component, IoC.IListener {

    public object OnInvoking(MethodBase method, object instance, object[] parameters) {
        if(method.Name == "MethodA") {
            //parameters[0] = new Core.Service2(null, null); // You can replace parameters of a method.
            //return "AABBCC";
        }
        return null; // If return non-null value, a method will not be invoke, return value will be used as result of invocation. Also, you can throw a exception.
    }

    public object OnInvoked(MethodBase method, object instance, object result) {
        if(method.Name == "MethodA") {
            return result.ToString() + DateTime.Now.ToString();
        }
        return null; // If return non-null value, a method will not be invoke, return value will be used as result of invocation. Also, you can throw a exception.
    }
}
```

```c#
_container.Bind<IoC.IListener, _Listener>();
// or
[assembly: IoC.BindService(typeof(IoC.IListener), typeof(_Listener))]
```

Also, you can use the `IContextBinder<TService, TContext>` that bind a Listener to specified a type of class.

```c#
_container.Bind<IContextBinder<IoC.IListener, Core.Service1A>, _Listener2>();
// or
[assembly: IoC.BindService(IoC.IContextBinder<IoC.IListener, Core.Service1A>, typeof(_Listener2))]
```

