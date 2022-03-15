# Dotnet.Commands

<img align="right" width="100px" src="https://avatars.githubusercontent.com/u/46710314?v=4" />

The repository implements [Command pattern](https://en.wikipedia.org/wiki/Command_pattern) that usually used in .NET mobile projects supporting [MVVM design pattern](https://en.wikipedia.org/wiki/Model–view–viewmodel). The main goal is to provide an implementation for commands creation.

## How to get started

The main entity is [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) class. The class is commands object factory. It creates a command for an action delegate which is passed into Command method.

```cs
// Validated() extension method adds argument null checks to all public methods
var commands = new Commands().Validated();
var command = commands.Command(
    ()=> { /* some logic here */ }, 
    CanExecute
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

## Cached commands

Somethimes it can be handy to cache the command once its been created by [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) factory. Especially useful case is view models.

```cs

private CachedCommands _commands;

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

## Single command execution lock

 By default [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) factory supports single command execution strategy. [SingleCommandExecutionLock](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Locks/SingleCommandExecutionLock.cs) entity is used to make it possible. The class is responsible for checking if any command is still executing once a new execution request comes for a command. If so the new execution command will be just ignored.
 There is a way to force a command execution even when other command execution is still in progress.

 ```cs
var commnand = _commands.Command(
    OnFooDelegate, 
    CanExecute,
    forceExecution: true
);
// The command OnFooDelegate will be executed even if
// the execution flow is globally locked in commands factory.
command.Execute();

```

## Navigation command execution lock

 Sometimes it can be useluf to lock the command execution once navigation operation is in progress. [NavigationExecutionLock](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Locks/NavigationExecutionLock.cs) entity can be used to make it possible. The class is responsible for checking if any navigation process is still happenning once a new execution request comes for a command. If so the new execution command will be just ignored.

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

Its important to now that by default commands factory has DefaultCommandExecutionInterval parameter

```cs

/// <summary>
/// This interval is necessary to avoid multi tapping command from the user
/// It can happen when user clicks simuntainiusly on several buttons on the screen
/// </summary>
public static int DefaultCommandExecutionInterval = 300;

```

As a result it can affect unit tests execution process when commands can execute in concurrency. Its possible to set it to 0 once commands factory is created.
