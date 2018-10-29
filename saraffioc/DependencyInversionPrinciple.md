[All products](../) / [Saraff.IoC](./index.md)
# Dependency Inversion Principle
DIP principle also helps in achieving loose coupling between the classes. It is highly recommended to use DIP and IoC together in order to achieve loose coupling. 

DIP suggests that high-level modules should not depend on low level modules. Both should depend on abstraction.
## DIP Definition
* High-level modules should not depend on low-level modules. Both should depend on abstraction. 
* Abstractions should not depend on details. Details should depend on abstractions. 

In dependency injection, a dependent object or service is provided with the object it needs at run time. The provided object will satisfy the dependency during program execution but would not be known at compile time. Rather than directly instantiating dependencies, or using static references, the objects a class needs in order to perform its actions are provided to the class in some abstracted form.

An example of the traditional way: 
```c#
public class UserLogic {
    private SomeOAuthService _authService;
    private SomeEmailService _emailService;

    public UserLogic() {
        _authService = new SomeOAuthService();
        _emailService = new SomeEmailService();
    }

    public void Register(string emailAddress, string password) {
        var authResult = _authService.RegisterUser(emailAddress,password);
        _emailService.SendMail(emailAddress, authResult.ConfirmationMessage);
    }
}

public class SomeOAuthService {
    public SomeOAuthResult RegisterUser(string emailAddress, string password) {
        // <<< Register a new user
    }
}

public class SomeEmailService {
    public SendMail(string emailAddress, string message) {
        // <<< Send an email
    }
}
```
Looking at the services used in this register action of the account controller, we can observe that changing the service would imply changing a lot of the codebase, especially if the service has been used in multiple parts of the project.

For instance, if the email service is replaced with a new one as shown below, we can see that the tightly coupled email service would need to be changed everywhere is it used in the project.
```c#
public class UserLogic {
    private SomeOAuthService _authService;
    private CustomEmailService _emailService;

    public UserLogic() {
        _authService = new SomeOAuthService();
        _emailService = new CustomEmailService();
    }

    public void Register(string emailAddress, string password) {
        var authResult = _authService.RegisterUser(emailAddress,password);
        _emailService.SendMail(emailAddress, authResult.ConfirmationMessage);
    }
}

public class CustomEmailService {
    public void SendMail(string emailAddress, string message) {
        // <<< Send an email
    }
}
```
From the examples shown, we can observe that there is a generic function that all email services will provide. It sends an email which can help us provide an abstraction of the email service, which is going to be used even when we do not know which exact service is to be used.
```c#
public interface IEmailService {
    void SendMail(string emailAddress, string message);
}

public interface IAuthService {
    public AuthResult RegisterUser(string emailAddress, string password);
}

public class UserLogic {
    private IAuthService _authService;
    private IEmailService _emailService;

    public void Register(string emailAddress, string password) {
        var _authResult = _authService.RegisterUser(emailAddress,password);
        _emailService.SendMail(emailAddress, authResult.ConfirmationMessage);
    }
}
```

[Inversion of Control](./InversionOfControl.md)
[Dependency Injection](./DependencyInjection.md)
[IoC Container](./IoCContainer.md)