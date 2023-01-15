using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class Commands : ICommands
    {
        public IAsyncCommand AsyncCommand(
            Func<CancellationToken, Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return AsyncCommand(
                (_, ct) => execute(ct),
                new CommandCanExecute(canExecute)
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, bool>? canExecute,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return AsyncCommand(
                execute,
                new CommandCanExecute<TParam>(canExecute)
            );
        }

        public IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            Func<TParam?, Task<bool>>? canExecute = null,
            bool forceExecution = false,
            string? name = null)
        {
            return AsyncCommand(
                execute,
                new CommandCanExecuteAsync<TParam>(canExecute)
            );
        }

        public ICommand Command(
            Action execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new Command<object>(
                execute,
                new CommandCanExecute(canExecute)
            );
        }

        public ICommand Command<TParam>(
            Action<TParam> execute,
            Func<TParam, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return new Command<TParam>(
                execute,
                new CommandCanExecute<TParam>(canExecute)
            );
        }

        private IAsyncCommand<TParam> AsyncCommand<TParam>(
            Func<TParam?, CancellationToken, Task> execute,
            ICanExecute<TParam> canExecute)
        {
            return new AsyncCommand<TParam>(
                execute,
                canExecute
            );
        }
    }
}