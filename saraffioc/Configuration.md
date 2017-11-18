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
}

// ...

_container.Bind<IoC.IConfiguration,_MyConfiguration>();
```