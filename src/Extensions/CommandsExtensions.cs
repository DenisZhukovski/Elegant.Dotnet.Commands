using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public static class CommandsExtensions
    {
        public static ICommands Safe(this ICommands commands, Action<Exception> onError)
        {
            return new SafeCommands(commands, (exception) =>
            {
                onError(exception);
                return true;
            });
        }
        
        public static ICommands Safe(this ICommands commands, Func<Exception, bool> onError)
        {
            return new SafeCommands(commands, onError);
        }
        
        public static ICommands Cached(this ICommands commands)
        {
            return new CachedCommands(commands);
        }

        public static ICommands Validated(this ICommands commands)
        {
            return new ValidatedCommands(commands);
        }

        public static void Execute(this ICommand command)
        {
            command.Execute(null);
        }

        public static Task ExecuteAsync(this IAsyncCommand command)
        {
            return command.ExecuteAsync(null);
        }

        public static void Execute(this IAsyncCommand command)
        {
            command.Execute(null);
        }

        public static Task ExecuteAsync(this ICommand command, object parameter)
        {
            if (command is IAsyncCommand asyncCommand)
            {
                return asyncCommand.ExecuteAsync(parameter);
            }
            command.Execute(parameter);
            return Task.CompletedTask;
        }

        public static IAsyncCommand AsyncCommand(
            this ICommands commands,
            Func<Task> execute,
            Func<bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return commands.AsyncCommand(
                (ct) => execute(),
                canExecute,
                forceExecution,
                name
            );
        }

        public static IAsyncCommand<TParam> AsyncCommand<TParam>(
            this ICommands commands,
            Func<TParam?, Task> execute,
            Func<TParam?, bool>? canExecute = null,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return commands.AsyncCommand(
                (param, ct) => execute(param),
                canExecute,
                forceExecution,
                name
            );
        }
        
        public static IAsyncCommand<TParam> AsyncCommand<TParam>(
            this ICommands commands,
            Func<TParam?, Task> execute,
            Func<TParam?, Task<bool>> canExecute,
            bool forceExecution = false,
            [CallerMemberName] string? name = null)
        {
            return commands.AsyncCommand(
                (param, ct) => execute(param),
                canExecute,
                forceExecution,
                name
            );
        }
    }
}
