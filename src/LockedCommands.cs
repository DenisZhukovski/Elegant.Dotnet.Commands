using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class LockedCommands : ICommands
    {
        private readonly ICommands _commands;
        private readonly ICommandExecutionLock _commandExecutionLock;
        private long _lockIndex;

        public LockedCommands(
            ICommands commands,
            int commandExecutionInterval)
            : this(
                commands,
                new SingleCommandExecutionLock(commandExecutionInterval)
            )
        {
        }

        public LockedCommands(
            ICommands commands,
            ICommandExecutionLock commandExecutionLock)
        {
            _commands = commands;
            _commandExecutionLock = commandExecutionLock;
        }

        public void ForceRelease()
        {
            Interlocked.Increment(ref _lockIndex);
            _commandExecutionLock.FreeExecutionLock();
        }

        public IAsyncCommand AsyncCommand(
            Func<CancellationToken, Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.AsyncCommand(
                new LockedExecutionAsync<object>(
                    execute,
                    forceExecution,
                    _commandExecutionLock,
                    ref _lockIndex
                ).ExecuteAsync,
                canExecute,
                forceExecution,
                name
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, bool>? canExecute,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.AsyncCommand(
                new LockedExecutionAsync<TParam?>(
                    execute,
                    forceExecution,
                    _commandExecutionLock,
                    ref _lockIndex
                ).ExecuteAsync,
                canExecute,
                forceExecution,
                name
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, Task<bool>>? canExecute = null,
            bool forceExecution = false,
            string? name = null)
        {
            return _commands.AsyncCommand(
                new LockedExecutionAsync<TParam?>(
                    execute,
                    forceExecution,
                    _commandExecutionLock,
                    ref _lockIndex
                ).ExecuteAsync,
                canExecute,
                forceExecution,
                name
            );
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.Command(
                new LockedExecution<object>(
                    execute,
                    forceExecution,
                    _commandExecutionLock
                ).Execute,
                canExecute,
                forceExecution,
                name
            );
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return _commands.Command(
                new LockedExecution<TParam>(
                    execute,
                    forceExecution,
                    _commandExecutionLock
                ).Execute,
                canExecute,
                forceExecution,
                name
            );
        }
    }
}