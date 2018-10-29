[All products](../) / [Saraff.IoC](./index.md)
# IoC Container
The IoC container is a framework to manage automatic dependency injection throughout the application so that we as a programmer do not need to put more time and effort on it.

IoC container creates an object of the specified class and also injects all the dependency objects through constructor, property or method at run time and disposes it at the appropriate time. This is done so that we don't have to create and manage objects manually. 

All the containers must provide easy support for the following DI lifecycle. 

* Register: 
The container must know which dependency to instantiate when it encounters a particular type. This process is called registration. Basically, it must include some way to register type-mapping. 

* Resolve: 
When using IoC container, we don't need to create objects manually. Container does it for us. This is called resolution. Container must include some methods to resolve the specified type; container creates an object of specified type, injects required dependencies if any and returns it. 

* Dispose: 
Container must manage the lifetime of dependent objects.

[Inversion of Control](./InversionOfControl.md)
[Dependency Inversion Principle](./DependencyInversionPrinciple.md)
[Dependency Injection](./DependencyInjection.md)
