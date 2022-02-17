using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class CachedCommands
    {
        private readonly ICommands _commands;
        private readonly Dictionary<string, ICommand> _cache = new Dictionary<string, ICommand>();

        public CachedCommands(ICommands commands)
        {
            _commands = commands;
        }

        public bool IsLocked => _commands.IsLocked;

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return _cache.GetOrAdd(
                name,
                () => _commands.Command(execute, canExecute, forceExecution)
            );
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return _cache.GetOrAdd(
                name,
                () => _commands.Command(execute, canExecute, forceExecution)
            );
        }

        public IAsyncCommand AsyncCommand(
            Func<Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return (IAsyncCommand)_cache.GetOrAdd(
                name,
                () => _commands.AsyncCommand(execute, canExecute, forceExecution)
            );
        }

        public IAsyncCommand AsyncCommand<TParam>(
            Func<TParam, Task> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return (IAsyncCommand)_cache.GetOrAdd(
                name,
                () => _commands.AsyncCommand(execute, canExecute, forceExecution)
            );
        }

        public void ForceRelease()
        {
            _commands.ForceRelease();
        }
    }
}
