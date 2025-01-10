using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class SafeCommands : ICommands
    {
        public SafeCommands(ICommands commands, Func<Exception, string?, bool> onError)
            : this(commands, new List<Func<Exception, string?, bool>> { onError })
        {
        }

        public SafeCommands(ICommands commands, IList<Func<Exception, string?, bool>> onError)
        {
            Commands = commands;
            OnError = onError;
        }
        
        internal IList<Func<Exception, string?, bool>> OnError { get; }
        
        internal ICommands Commands { get; }

        public IAsyncCommand AsyncCommand(
            Func<CancellationToken, Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return Commands.AsyncCommand(
                execute,
                canExecute,
                forceExecution,
                name
            ).Safe(OnError, name);
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return Commands.AsyncCommand(
                execute,
                canExecute,
                forceExecution,
                name
            ).Safe(OnError, name);
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, Task<bool>>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return Commands.AsyncCommand(
                execute,
                canExecute,
                forceExecution,
                name
            ).Safe(OnError, name);
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return Commands.Command(
                execute,
                canExecute,
                forceExecution,
                name
            ).Safe(OnError, name);
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return Commands.Command(
                execute,
                canExecute,
                forceExecution,
                name
            ).Safe(OnError, name);
        }
    }
}