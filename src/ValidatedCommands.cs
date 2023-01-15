using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class ValidatedCommands : ICommands
    {
        private readonly ICommands _commands;

        public ValidatedCommands(ICommands commands)
        {
            _commands = commands;
        }

        public IAsyncCommand AsyncCommand(
            Func<CancellationToken, Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.AsyncCommand(execute, canExecute, forceExecution, name);
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.AsyncCommand(execute, canExecute, forceExecution, name);
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, Task<bool>>? canExecute = null,
            bool forceExecution = false,
            string? name = null)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.AsyncCommand(execute, canExecute, forceExecution, name);
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.Command(execute, canExecute, forceExecution, name);
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.Command(execute, canExecute, forceExecution, name);
        }
    }
}