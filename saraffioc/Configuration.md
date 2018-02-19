[All products](../) / [Saraff.IoC](./index.md)
# Configuration
You can redefine a types that a ServiceContainer use for binding and injection. It require to implement a IoC.IConfiguration interface and bind it.
```c#
internal sealed class _MyConfiguration:Component, IoC.IConfiguration {

    public Type BindServiceAttributeType {
        get {
            return typeof(MyBindAttribute);
        }
    }

    public BindServiceCallback BindServiceCallback {
        get {
            return new BindServiceCallback((x,callback) => {
                var _attr = x as MyBindAttribute;
                if(_attr!=null) {
                    callback(_attr.Service,_attr.ObjectType);
                }
            });
        }
    }

    public Type ContextBinderType {
        get {
            return typeof(IMyContextBinder<,>);
        }
    }

    public Type ServiceRequiredAttributeType {
        get {
            return typeof(MyServiceRequiredAttribute);
        }
    }

    public Type ProxyRequiredAttributeType 
        get {
            return typeof(MyProxyRequiredAttribute);
        }
    }

    public Type ListenerType 
        get {
            return typeof(IMyListener);
        }
    }

    public InvokingCallback InvokingCallback 
        get {
            return (listener, method, instance, parameters) => (listener as IMyListener)?.OnInvoking(method, instance, parameters);
        }
    }

    public InvokedCallback InvokedCallback 
        get {
            return (listener, method, instance, result) => (listener as IMyListener)?.OnInvoked(method, instance, result);
        }
    }

    public CatchCallback CatchCallback 
        get {
            return (listener, method, instance, ex) => (listener as IMyListener)?.OnCatch(method, instance, ex);
        }
    }
}

// ...

_container.Bind<IoC.IConfiguration,_MyConfiguration>();
```