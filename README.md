# Dotnet.Commands

<img align="right" width="100px" src="https://avatars.githubusercontent.com/u/46710314?v=4" />

The repository implements [Command pattern](https://en.wikipedia.org/wiki/Command_pattern) that usually used in .NET mobile projects supporting [MVVM design pattern](https://en.wikipedia.org/wiki/Model–view–viewmodel). The main goal is to provide an implementation for commands creation.

## How to get started

The main entity is [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) class. The class is commands object factory. It creates a command for an action delegate which is passed into Command method.

```cs
var commands = new Commands().Validated();
var command = commands.Command(
    ()=> { /* some logic here */ }, 
    CanExecute
);

```

## Async Command

Commands factory also supports async commands delegates.

```cs
var commands = new Commands().Validated();

var asyncCommand = commands.AsyncCommand(
    async ()=> { /* some async logic here */ }, 
    CanExecute
);

```

## Cached commands

Somethimes it can be handy to cache the command once its been created by [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) factory.

```cs

private CachedCommands _commands;

public ViewModel()
{
    _commands = new Commands().Validated().Cached();
}

/* 
    Here the command will be create only once
    All the other calls of this property will be using the commands cache. Under the hood CallerMemberName attribute is used to detect the same call.
 */
public ICommand FooCommand => _commands.Command(
    OnFooDelegate, 
    CanExecute
);

```

## Single command execution lock

 By default [Commands](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Commands.cs) factory supports single command execution stargety. [SingleCommandExecutionLock](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Locks/SingleCommandExecutionLock.cs) entity is used to make it possible. The class is responsible for checking if any command is still executing once a new execution request comes for a command. If so the new execution command will be just ignored.

## Navigation command execution lock

 Sometimes it can be useluf to lock the command execution once navigation operation is in progress. [NavigationExecutionLock](https://github.com/DenisZhukovski/Dotnet.Commands/blob/main/src/Locks/NavigationExecutionLock.cs) entity can be used to make it possible. The class is responsible for checking if any navigation process is still happenning once a new execution request comes for a command. If so the new execution command will be just ignored.
