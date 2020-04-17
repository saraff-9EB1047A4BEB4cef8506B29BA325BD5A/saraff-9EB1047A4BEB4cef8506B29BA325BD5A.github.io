[All products](../) / [Saraff.IoC](./index.md)
# Nested containers
The nested container can be used to dispose components without waiting for the the main container to be disposed. 
Also it can be used to isolate dependencies, you can build a container tree that goes up to the root (main) container.
Services are resolved begin from the current (nested) container to the root (main) container.
```c#
using(var _container = new IoC.ServiceContainer()) {

    // ...

    var _nested = _container.CreateNestedContainer();

    // ...

}
```