[All products](../) / [Saraff.IoC](./index.md)
# Dependency Injection
Dependency Injection (DI) is a design pattern which implements IoC principle to invert the creation of dependent objects.

Dependency Injection pattern involves 3 types of classes. 
* Client Class: The client class (dependent class) is a class which depends on the service class 
* Service Class: The service class (dependency) is a class that provides service to the client class. 
* Injector Class: The injector class injects service class object into the client class. 

## Types of Dependency Injection
As you have learned above, the injector class injects the service (dependency) to the client (dependent). The injector class injects dependencies broadly in three ways: through constructor, through property, or through method. 

### Constructor Injection
In the constructor injection, injector supplies service (dependency) through the client class constructor. 
```c#
public class UserLogic {
    private IAuthService _authService;
    private IEmailService _emailService;

    public UserLogic(IEmailSevice emailService, IAuthService authService) {
        _authService = authService;
        _emailService = emailService;
    }

    public void Register(string emailAddress, string password) {
        this._authService.RegisterUser(emailAddress,password);
        this._emailService.SendMail(emailAddress, authResult.ConfirmationMessage);
    }

    // ...
}
```
### Property (Setter) Injection
In property injection (aka Setter Injection), injector supplies dependency through a public property of the client class. 
```c#
public class UserLogic {

    public void Register(string emailAddress, string password) {
        this.AuthService.RegisterUser(emailAddress,password);
        this.EmailService.SendMail(emailAddress, authResult.ConfirmationMessage);
    }

    public IAuthService AuthService {
        get; set;
    }

    public IEmailService EmailService {
        get; set;
    }

    // ...
}
```
### Method Injection
In this type of injection, client class implements an interface which declares method(s) to supply dependency and the injector uses this interface to supply dependency to the client class. 
```c#
public class UserLogic {

    public void Register(string emailAddress, string password, IEmailService emailService, IAuthService authService) {
        authService.RegisterUser(emailAddress,password);
        emailService.SendMail(emailAddress, authResult.ConfirmationMessage);
    }

    // ...
}
```

[Inversion of Control](./InversionOfControl.md)
[Dependency Inversion Principle](./DependencyInversionPrinciple.md)
[IoC Container](./IoCContainer.md)