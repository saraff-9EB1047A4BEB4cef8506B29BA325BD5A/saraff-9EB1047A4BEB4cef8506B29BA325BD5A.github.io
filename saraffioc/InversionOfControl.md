[All products](../) / [Saraff.IoC](./index.md)
# Inversion of Control Principle
IoC is a design principle which recommends inversion of different kinds of controls in object oriented design to achieve loose coupling between the application classes. Here, the control means any additional responsibilities a class has other than its main responsibility, such as control over the flow of an application, control over the dependent object creation and binding (Remember SRP-Single Responsibility Principle). If you want to do TDD (Test Driven Development) then you must use IoC principle without which TDD is not possible.

IoC is all about inverting the control. To explain in layman's term, suppose you drive a car to your work place, it means you control the car. IoC principle suggests to invert the control, meaning instead of driving the car yourself, you hire a cab where another person will drive the car. Thus it is called inversion of the control from you to the cab driver. You don't have to drive a car yourself and let the driver do the driving so that you can focus on your main work. 

IoC principle helps in designing loosely coupled classes which make them testable, maintainable and extensible. 

In an object oriented design, classes should be designed in loosely coupled way. Loosely coupled means changes in one class should not force other classes to change, so the whole application can become maintainable and extensible.

[Dependency Inversion Principle](./DependencyInversionPrinciple.md)
[Dependency Injection](./DependencyInjection.md)
[IoC Container](./IoCContainer.md)