using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class SafeCommands : ICommands
    {
        private readonly ICommands _commands;
        private readonly Func<Exception, string?, bool> _onError;

        public SafeCommands(ICommands commands, Func<Exception, string?, bool> onError)
        {
            _commands = commands;
            _onError = onError;
        }

        public IAsyncCommand AsyncCommand(
            Func<CancellationToken, Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new SafeAsyncCommand<object>(
                _commands.AsyncCommand(
                    execute,
                    canExecute,
                    forceExecution,
                    name
                ),
                _onError,
                name
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new SafeAsyncCommand<TParam>(
                _commands.AsyncCommand(
                    execute,
                    canExecute,
                    forceExecution,
                    name
                ),
                _onError,
                name
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, Task<bool>>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new SafeAsyncCommand<TParam>(
                _commands.AsyncCommand(
                    execute,
                    canExecute,
                    forceExecution,
                    name
                ),
                _onError,
                name
            );
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new SafeCommand(
                _commands.Command(
                    execute,
                    canExecute,
                    forceExecution,
                    name
                ),
                _onError,
                name
            );
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new SafeCommand(
                _commands.Command(
                    execute,
                    canExecute,
                    forceExecution,
                    name
                ),
                _onError,
                name
            );
        }
    }
}