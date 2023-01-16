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
        private readonly Func<Exception, string, bool> _onError;

        public SafeCommands(ICommands commands, Func<Exception, string, bool> onError)
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
            return _commands.AsyncCommand(
                execute.Safe(_onError, name),
                (Func<bool>)(() => CanExecute(name, canExecute)),
                forceExecution,
                name
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.AsyncCommand(
                execute.Safe(_onError, name),
                p => CanExecute(name, p, canExecute),
                forceExecution,
                name
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam, CancellationToken, Task> execute,
            Func<TParam, Task<bool>> canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.AsyncCommand(
                execute.Safe(_onError, name),
                (p) => canExecute == null
                    ? Task.FromResult(true)
                    : canExecute.Safe(_onError, name)(p),
                forceExecution,
                name
            );
        }

        public ICommand Command(
            Action execute,
            Func<bool> canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.Command(
                execute.Safe(_onError, name),
                () => CanExecute(name, canExecute),
                forceExecution,
                name
            );
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool> canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.Command(
                execute.Safe(_onError, name),
                (p) => CanExecute(name, p, canExecute),
                forceExecution,
                name
            );
        }

        private bool CanExecute<TParam>(
            string name,
            TParam par, 
            Func<TParam, bool>? canExecute = null)
        {
            return canExecute == null || canExecute.Safe(_onError, name)(par);
        }

        private bool CanExecute(
            string name,
            Func<bool>? canExecute = null)
        {
            return canExecute == null || canExecute.Safe(_onError, name)();
        }
    }
}