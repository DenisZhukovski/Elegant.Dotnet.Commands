using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class CachedCommands : ICommands
    {
        private readonly ICommands _commands;
        private readonly Dictionary<string, ICommand> _cache = new();

        public CachedCommands(ICommands commands)
        {
            _commands = commands;
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute, 
            Func<TParam?, Task<bool>>? canExecute = null, 
            bool forceExecution = false,
            string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return (IAsyncCommand<TParam>)_cache.GetOrAdd(
                name,
                () =>_commands.AsyncCommand(execute, canExecute, forceExecution, name)
            );
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return _cache.GetOrAdd(
                name,
                () => _commands.Command(execute, canExecute, forceExecution, name)
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
                () => _commands.Command(execute, canExecute, forceExecution, name)
            );
        }

        public IAsyncCommand AsyncCommand(
            Func<CancellationToken, Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return (IAsyncCommand)_cache.GetOrAdd(
                name,
                () => _commands.AsyncCommand(execute, canExecute, forceExecution, name)
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            _ = name ?? throw new ArgumentNullException(nameof(name));
            return (IAsyncCommand<TParam>)_cache.GetOrAdd(
                name,
                () => _commands.AsyncCommand(execute, canExecute, forceExecution, name)
            );
        }
    }
}
