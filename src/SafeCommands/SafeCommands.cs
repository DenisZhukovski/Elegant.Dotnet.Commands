using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class SafeCommands : ICommands
    {
        public SafeCommands(ICommands commands, IErrorHandler onError)
        {
            Commands = commands;
            OnError = onError;
        }
        
        internal IErrorHandler OnError { get; }
        
        public ICommands Commands { get; }

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