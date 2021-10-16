# Convey service provider issue

This repository was created to provide an example of wrong convey behavior when using reloadable configuration providers. This affects version 0.5.512 and huge? portion of older versions.

## How to reproduce the problem

1. Create configuration provider (e.x `TimeProvider` in this repository) that reloads `IConfiguration` when configuration source changes. 
2. Use that provider through `ConfigureAppConfiguration`
3. Add Convey to application in `ConfigureServices`. `services.AddConvey()` is enough to trigger the error.

After executing those steps **all** configuration providers are going to stop reporting configuration changes. 

## Launching code inside repository

Just launch it and observer output. Time value is not going change.

You can also comment `AddConvey` line in `Startup.cs` to see that after you commented it then time output is working as it should. 

## Workaround

You can create simple service class that requires `IConfiguration` instance and reload it manually detecting moment when you should do that.

Example code:

```csharp
var configuration = configuration as ConfigurationRoot;

// (...)

configuration.Reload();
```

## Where the issue originates

Issue resides inside [Extensions.cs:GetOptions<TModel>](https://github.com/snatch-dev/Convey/blob/a53903a980e893628ea23cd9ce0620cd227464fd/src/Convey/src/Convey/Extensions.cs#L57). Every time when Convey wants to get options from service collection it builds new service provider based on service collection from main application and retrieves `IConfiguration` to fetch options values. 
By using word using on `ServiceProvider` Convey ensures it is going to be disposed along with objects it created. This is going to dispose `IConfiguration` object which is `ConfigurationRoot` under the hood. `ConfigurationRoot` is going to dispose all configuration providers attached to it which is going to break reloadable functionality.


## Proposed solutions

1. Stop building service provider from service collection provided by users. It can have unintended side effects that are very hard to diagnose. 

Instead supply your convey builder with `IConfiguration` instance in addition to `ServiceCollection`. `IConfiguration` should be easily accessible from every `Startup.cs`. Therefore all existing users should be able to modify their code with ease to cope with this change.
Unfortunately it is going to be a breaking change :/.

Additionally this approach would require an inner instance of `ServiceCollection` to instantiate your objects during configuring process. All configured objects could be added to main application's `ServiceCollection` that was provided to Convey. (Personally I think this should be reworked as it looks weird to constantly build and dispose service providers. This is my own opinion though)

This is an approach that is going to eliminate all possible side effects of using `ServiceCollection` you do **not own**.

2. Supply ConveyBuilder with `IConfiguration` and rewrite method [Extensions.cs:GetOptions<TModel>](https://github.com/snatch-dev/Convey/blob/a53903a980e893628ea23cd9ce0620cd227464fd/src/Convey/src/Convey/Extensions.cs#L57) to use it. There might be other methods that needs to be rewritten to use this approach
