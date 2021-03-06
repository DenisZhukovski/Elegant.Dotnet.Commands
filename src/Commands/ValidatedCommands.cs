using System;
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

        public bool IsLocked => _commands.IsLocked;

        public IAsyncCommand AsyncCommand(
            Func<Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.AsyncCommand(execute, canExecute, forceExecution);
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam, Task> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.AsyncCommand(execute, canExecute, forceExecution);
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.Command(execute, canExecute, forceExecution);
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false)
        {
            _ = execute ?? throw new ArgumentNullException(nameof(execute));
            return _commands.Command(execute, canExecute, forceExecution);
        }

        public void ForceRelease()
        {
            _commands.ForceRelease();
        }
    }
}
