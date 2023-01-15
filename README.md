# Dotnet.Commands

<img align="right" width="100px" src="https://avatars.githubusercontent.com/u/46710314?v=4" />

<h3 align="center">
   
  [![NuGet](https://img.shields.io/nuget/v/Elegant.Dotnet.Commands.svg)](https://www.nuget.org/packages/Elegant.Dotnet.Commands/) 
  [![Downloads](https://img.shields.io/nuget/dt/Elegant.Dotnet.Commands.svg)](https://www.nuget.org/Elegant.Dotnet.Commands/)
  [![Stars](https://img.shields.io/github/stars/DenisZhukovski/Elegant.Dotnet.Commands?color=brightgreen)](https://github.com/DenisZhukovski/Elegant.Dotnet.Commands/stargazers) 
  [![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.md) 
  [![Hits-of-Code](https://hitsofcode.com/github/DenisZhukovski/Elegant.Dotnet.Commands?branch=main)](https://hitsofcode.com/github/DenisZhukovski/Elegant.Dotnet.Commands/view)
  [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=DenisZhukovski_Dotnet.Commands&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=DenisZhukovski_Dotnet.Commands)
  [![EO principles respected here](https://www.elegantobjects.org/badge.svg)](https://www.elegantobjects.org)
</h3>

The package implements [Command pattern](https://en.wikipedia.org/wiki/Command_pattern) that is usually used in .NET mobile projects supporting [MVVM design pattern](https://en.wikipedia.org/wiki/Model–view–viewmodel). The main goal is to provide an implementation for commands instantiation.

## How to get started

[Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) is the main entity. The class is commands object factory. It creates a command for an action delegate which is passed into Command method.

```cs
// Validated() extension method adds argument null checks to all public methods
var commands = new Commands().Validated();
var command = commands.Command(
    ()=> { /* some logic here */ }, 
    CanExecute
);

```

Recommened registration line with full featured commands factory.

```cs
container.RegisterInstance<ICommands>(
   new Commands()
      .Locked() // Single command execution. Handy to avoid multitapping problem from UI.
      .Validated()
      .Safe(ex => 
      {
         // Log the exception to monitoring service.
         // Show the error to the user. DialogService can be used for mobile apps.
      })
);

```

## Async Command

Commands factory also supports async commands delegates. It can be especially useful in unit tests. Sometimes the unit test has to wait while command async delegate will be executed before it can check the result.

```cs
var commands = new Commands().Validated();

var asyncCommand = commands.AsyncCommand(
    async ()=> { /* some async logic here */ }, 
    CanExecute
);

await asyncCommnad.ExecuteAsync();

```

## Cancellation Token

The async command execution can be time comsuming and sometimes it can be useful to have a possibility to cancel the command execution. To make it possible [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-6.0) entity is been passed into Async command execution delegate. To cancel the token async command Cancel method should be called.

```cs
var commands = new Commands().Validated();

var asyncCommand = commands.AsyncCommand(
    async (cancellationToken) => 
    { 
      /* some async logic here */ 
      if (cancellationToken.IsCancellationRequested)
      {
         /* continue async logic here */ 
      }
    }, 
    CanExecute
);

var commandTask = asyncCommnad.ExecuteAsync();
await Task.Delay(1000);
asyncCommnad.Cancel();

```

## Can Execute Async

The async commands also support CanExecuteAsync delegate. It can be useful when
async operation has to be executed to detect of a command can execute.

```cs
var commands = new Commands().Validated();

var asyncCommand = commands.AsyncCommand<int>(
    async (number)=> { /* some async logic here */ }, 
    async (number) => {
         /* some async logic here to detect can execute async*/ 
    }, 
);

await asyncCommnad.ExecuteAsync(12);

```

## Safe commands

Somethimes it can be handy to create a command and catch all the exceptions that can occur during execution. [SafeCommands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/SafeCommands.cs) factory decorator can be used to do so.

```cs

private ICommands _commands;

public ViewModel(ICommmands commands)
{
    // Cached() extension method adds caching for commands.
    _commands = commands
        .Cached()
        .Safe(ex => _dialog.ShowAlert("Command Error", ex.Message));
}

public ICommand FooCommand => _commands
    .Safe(ex => {
        if (ex is ValidationException)
        {
            ErrorText = "Incorrect Validation";
            return true;
        }
        return false;
    })
    .Command(OnFooDelegate);

```

It can be noticed that Safe extension method was used twice in the code example above.
The first method uses Action<Exception> as argument what causing not further exception propagation while the second method conditionally can stop exception propagation.

## Cached commands

Somethimes it can be handy to cache the command once its been created by [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) factory. Especially useful case is view models.

```cs

private ICommands _commands;

public ViewModel(ICommmands commands)
{
    _commands = commands.Cached();
}

/* 
    Here the command will be create only once
    All the other calls of this property will be using the commands cache.
 */
public ICommand FooCommand => _commands.Command(
    OnFooDelegate, 
    CanExecute
);

```

**Important:** _Under the hood [CachedCommands](https://github.com/DenisZhukovski/Elegant.Dotnet.Commands/blob/main/src/Commands/CachedCommands.cs) uses [CallerMemberName](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callermembernameattribute?view=net-6.0) attribute to detect the same call. It means this entity should be created for each view model independently otherwise the collisions are possible._

## Locked commands

### Single command execution lock

 By default [LockedCommands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/LockedCommands.cs) factory decorator supports single command execution strategy ([SingleCommandExecutionLock](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Locks/SingleCommandExecutionLock.cs) entity is used to make it possible). The class is responsible for checking if any command is still executing once a new execution request comes for a command. If so the new execution command will be just ignored.
 There is a way to force a command execution even when other command execution is still in progress.

 ```cs
var commands = new Commands().Locked().Validated();
var commnand = _commands.Command(
    OnFooDelegate, 
    CanExecute,
    forceExecution: true
);
// The command OnFooDelegate will be executed even if
// the execution flow is globally locked in commands factory.
command.Execute();

```

### Navigation command execution lock

 Sometimes it can be useful to lock the command execution once navigation operation is in progress. [NavigationExecutionLock](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Locks/NavigationExecutionLock.cs) entity can be used to make it possible. The class is responsible for checking if any navigation process is still happenning once a new execution request comes for a command. If so the new execution command will be just ignored.

## Useful extensions

Usually navigation flow should be happening on UI Thread. This extension method tries to make the navigation flow to happen always on UI Thread.

```cs

public static class CommandsExtensions
{
    public static IAsyncCommand NavigationCommand(
        this CachedCommands commands,
        Func<Task<INavigationResult>> onNavigation,
        [CallerMemberName] string? name = null)
    {
        return commands.AsyncCommand(() =>
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var navigationResult = await onNavigation();
                    if (navigationResult.Exception != null)
                    {
                        throw navigationResult.Exception;
                    }
                    taskCompletionSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });
            return taskCompletionSource.Task;
        }, name: name);
    }
}

```

## In Unit Tests

Its important to be aware that once Locked commands decorator is used in the project the unit test execution can be affected. Locked commands factory has DefaultCommandExecutionInterval parameter.

```cs

/// <summary>
/// This interval is necessary to avoid multi tapping command from the user
/// It can happen when user clicks simuntainiusly on several buttons on the screen
/// </summary>
public static int DefaultCommandExecutionInterval = 300;

```

As a result it can affect unit tests execution process when commands can execute in concurrency or one by one. Its possible to set it to 0 once commands factory is created.
   
```cs
void SomeUnitTest()
{
   var commands = new Commands().Locked(0);
}

```

## Build status

<div align="center">
  
   [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=DenisZhukovski_Dotnet.Commands&metric=alert_status)](https://sonarcloud.io/dashboard?id=DenisZhukovski_Dotnet.Commands) 
   [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=DenisZhukovski_Dotnet.Commands&metric=coverage)](https://sonarcloud.io/dashboard?id=DenisZhukovski_Dotnet.Commands)
   [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=DenisZhukovski_Dotnet.Commands&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=DenisZhukovski_Dotnet.Commands)
   [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=DenisZhukovski_Dotnet.Commands&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=DenisZhukovski_Dotnet.Commands) 
</div>
