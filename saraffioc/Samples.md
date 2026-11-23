[All products](../) / [Saraff.IoC](./index.md)
# Samples

## Download

The original sample applications are available as a single archive:

* [Download Saraff.IoC.Samples.zip](./content/Saraff.IoC.Samples.zip)

The archive contains the classic samples listed below. The snippets in this documentation show
the current 2.x API and are the best starting point for new code.

## Classic samples

### SimpleApp

* Dependency injection through a constructor
* Context-aware injection
* Basic container lifetime

### EnhancementApp

* Constructor injection with parameters
* Property injection
* Method injection through a proxy
* Context-aware injection

### LazyApp

* Property injection
* Lazy (deferred) injection
* Method injection
* Generic method injection
* Breaking a dependency cycle with `Lazy<T>`

### TraceApp

* Constructor injection
* Invocation interception
* Exception handling
* Logging through `IListener`

## Modern recipes

### Console application

```csharp
using System;
using Saraff.IoC;

public interface IGreeting {
    string Greet();
}

public sealed class Greeting : IGreeting {
    public string Greet() => "hello";
}

public sealed class App {
    private readonly IGreeting _greeting;

    [ServiceRequired]
    public App(IGreeting greeting) {
        this._greeting = greeting;
    }

    public void Run() {
        Console.WriteLine(this._greeting.Greet());
    }
}

using (var container = new ServiceContainer()) {
    container.Register<IGreeting, Greeting>();
    container.Register<App, App>();

    container.Resolve<App>().Run();
}
```

### Plugin host with multiple registrations

```csharp
public interface IPlugin {
    string Name { get; }
    void Run();
}

public sealed class EmailPlugin : IPlugin {
    public string Name => "email";
    public void Run() => Console.WriteLine("email");
}

public sealed class SmsPlugin : IPlugin {
    public string Name => "sms";
    public void Run() => Console.WriteLine("sms");
}

public sealed class PluginHost {
    private readonly IEnumerable<IPlugin> _plugins;

    [ServiceRequired]
    public PluginHost(IEnumerable<IPlugin> plugins) {
        this._plugins = plugins;
    }

    public void RunAll() {
        foreach (var plugin in this._plugins) {
            plugin.Run();
        }
    }
}

using (var container = new ServiceContainer()) {
    container.Register<IPlugin, EmailPlugin>(ServiceLifetime.Transient);
    container.Register<IPlugin, SmsPlugin>(ServiceLifetime.Transient);
    container.Register<PluginHost, PluginHost>();

    container.Resolve<PluginHost>().RunAll();
}
```

### Per-request scope

```csharp
public interface IRequestContext {
    Guid Id { get; }
}

public sealed class RequestContext : IRequestContext {
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class RequestHandler {
    [ServiceRequired]
    public RequestHandler(IRequestContext context) {
        this.Context = context;
    }

    public IRequestContext Context { get; }
}

using (var container = new ServiceContainer()) {
    container.Register<IRequestContext, RequestContext>(ServiceLifetime.Scoped);
    container.Register<RequestHandler, RequestHandler>();

    using (var request = container.CreateScope()) {
        var handler = request.Resolve<RequestHandler>();
        Console.WriteLine(handler.Context.Id);
    }
}
```

### Interception with logging

```csharp
using System.Reflection;

[ProxyRequired]
public sealed class Calculator : Component, ICalculator {
    public int Add(int a, int b) => a + b;
}

public sealed class LoggingListener : Component, IListener {
    public InvocationResult OnInvoking(MethodBase method, object instance, object[] parameters) {
        Console.WriteLine($"-> {method.Name}({string.Join(", ", parameters)})");
        return InvocationResult.NotHandled;
    }

    public InvocationResult OnInvoked(MethodBase method, object instance, object result) {
        Console.WriteLine($"<- {result}");
        return InvocationResult.NotHandled;
    }

    public InvocationResult OnCatch(MethodBase method, object instance, Exception ex) {
        Console.WriteLine($"!! {ex.Message}");
        return InvocationResult.NotHandled;
    }
}

using (var container = new ServiceContainer()) {
    container.Register<IListener, LoggingListener>();
    container.Register<ICalculator, Calculator>();

    var calculator = container.Resolve<ICalculator>();
    Console.WriteLine(calculator.Add(2, 3));
}
```

### Async initialization (`netstandard2.0`)

```csharp
public sealed class Cache : ICache, IAsyncInitializable, IAsyncDisposable {
    public async Task InitializeAsync(CancellationToken cancellationToken = default) {
        // load data
        await Task.Delay(100, cancellationToken);
    }

    public async ValueTask DisposeAsync() {
        // flush data
        await Task.CompletedTask;
    }
}

using (var container = new ServiceContainer()) {
    container.Register<ICache, Cache>(ServiceLifetime.Scoped);

    await using (var scope = container.CreateScope()) {
        var cache = await scope.ResolveAsync<ICache>();
        // cache is initialized
    }
}
```

### Factory-created view model

```csharp
public sealed class ReportViewModel {
    [ServiceRequired]
    public ReportViewModel(string title, IReportService service) {
        this.Title = title;
        this.Service = service;
    }

    public string Title { get; }
    public IReportService Service { get; }
}

using (var container = new ServiceContainer()) {
    container.Register<IReportService, ReportService>();

    var viewModel = container.CreateInstance<ReportViewModel>(
        x => x("title", "Annual report"));
}
```

### Legacy assembly bindings

```csharp
[assembly: BindService(typeof(IService1), typeof(Service1))]

using (var container = new ServiceContainer()) {
    container.Load(typeof(Program).Assembly);

    var service = container.Resolve<IService1>();
}
```

## Recommended sample structure

```text
MyApp/
  Program.cs              // composition root
  Services/               // interfaces + implementations
  Handlers/               // consumers with [ServiceRequired]
  Infrastructure/         // container configuration
```

Keep the container setup in one place. Do not resolve services from static locators deep inside
the application.

## See also

* [Getting Started](./GettingStarted.md)
* [Registration](./Registration.md)
* [Lifetimes and Scopes](./Lifetimes.md)
* [Interception and Listeners](./Listener.md)
* [Async Initialization](./AsyncInitialization.md)
