using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public static class CommandsExtensions
    {
        /// <summary>
        /// This interval is necessary to avoid multi tapping command from the user
        /// It can happen when user clicks simultaneously on several buttons on the screen
        /// </summary>
        public static readonly int DefaultCommandExecutionInterval = 300;        
        
        public static LockedCommands Locked(this ICommands commands)
        {
            return commands.Locked(DefaultCommandExecutionInterval);
        }
        
        public static LockedCommands Locked(this ICommands commands, int commandExecutionInterval)
        {
            return new LockedCommands(commands, commandExecutionInterval);
        }
        
        public static SafeCommands Safe(this ICommands commands, Action<Exception> onError)
        {
            return commands.Safe((exception, _) => onError(exception));
        }
        
        public static SafeCommands Safe(this ICommands commands, Action<Exception, string?> onError)
        {
            return commands.Safe((exception, name) =>
            {
                onError(exception, name);
                return true;
            });
        }
        
        public static SafeCommands Safe(this ICommands commands, Func<Exception, bool> onError)
        {
            return commands.Safe((exception, _) => onError(exception));
        }
        
        public static SafeCommands Safe(this ICommands commands, Func<Exception, string?, Task<bool>> onError)
        {
            _  = commands ?? throw new ArgumentNullException(nameof(commands));
            _  = onError ?? throw new ArgumentNullException(nameof(onError));
            if (commands is not SafeCommands safeCommands)
            {
                return new SafeCommands(commands, new ErrorHandler(onError));
            }
            
            return new SafeCommands(safeCommands.Commands, safeCommands.OnError.Add(onError));
        }
        
        public static SafeCommands Safe(this ICommands commands, Func<Exception, string?, bool> onError)
        {
            _  = commands ?? throw new ArgumentNullException(nameof(commands));
            _  = onError ?? throw new ArgumentNullException(nameof(onError));
            if (commands is not SafeCommands safeCommands)
            {
                return new SafeCommands(commands, new ErrorHandler(onError));
            }
            
            return new SafeCommands(safeCommands.Commands, safeCommands.OnError.Add(onError));
        }
        
        public static CachedCommands Cached(this ICommands commands)
        {
            return new CachedCommands(commands);
        }

        public static ValidatedCommands Validated(this ICommands commands)
        {
            return new ValidatedCommands(commands);
        }

        public static void Execute(this ICommand command)
        {
            command.Execute(null);
        }

        public static Task<bool> ExecuteAsync(this IAsyncCommand command)
        {
            return command.ExecuteAsync(null);
        }

        public static void Execute(this IAsyncCommand command)
        {
            command.Execute(null);
        }

        public static async Task<bool?> ExecuteAsync(this ICommand command, object parameter)
        {
            if (command is IAsyncCommand asyncCommand)
            {
                return await asyncCommand.ExecuteAsync(parameter);
            }
            command.Execute(parameter);
            return null;
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

        public static bool HasError(this ICommand command)
        {
            if (command is SafeCommand safeCommand)
            {
                return safeCommand.Exception != null;
            }

            return false;
        }
        
        public static bool HasError(this IAsyncCommand command)
        {
            if (command is SafeAsyncCommand<object> safeCommand)
            {
                return safeCommand.Exception != null;
            }

            return false;
        }
        
        public static bool HasError<T>(this IAsyncCommand<T> command)
        {
            if (command is SafeAsyncCommand<T> safeCommand)
            {
                return safeCommand.Exception != null;
            }

            return false;
        }

        public static ICommand Unsafe(this ICommand command)
        {
            if (command is SafeCommand safeCommand)
            {
                return safeCommand.Command;
            }

            return command;
        }
        
        public static IAsyncCommand Unsafe(this IAsyncCommand command)
        {
            if (command is SafeAsyncCommand<object> safeCommand)
            {
                return safeCommand.Command;
            }

            return command;
        }
        
        public static IAsyncCommand Unsafe<T>(this IAsyncCommand<T> command)
        {
            if (command is SafeAsyncCommand<T> safeCommand)
            {
                return safeCommand.Command;
            }

            return command;
        }
        
        public static ICommand Safe(
            this ICommand command,
            Action<Exception> onError,
            [CallerMemberName] string? callerName = null)
        {
            return command.Safe(
                (exception, _) =>
                {
                    onError(exception);
                    return true;
                },
                callerName
            );
        }
        
        public static ICommand Safe(
            this ICommand command,
            Action<Exception, string?> onError,
            [CallerMemberName] string? callerName = null)
        {
            return command.Safe(
                (exception, name) =>
                {
                    onError(exception, name);
                    return true;
                },
                callerName
            );
        }
        
        public static ICommand Safe(
            this ICommand command, 
            Func<Exception, string?, bool> onError,
            [CallerMemberName] string? name = null)
        {
            return command.Safe(
                new List<Func<Exception, string?, bool>> { onError },
                name
            );
        }
        
        public static IAsyncCommand Safe(
            this IAsyncCommand command, 
            Func<Exception, string?, bool> onError,
            [CallerMemberName] string? name = null)
        {
            return command.Safe<object>(
                new List<Func<Exception, string?, bool>> { onError },
                name
            );
        }
        
        public static ICommand Safe(
            this ICommand command,
            IList<Func<Exception, string?, bool>> onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeCommand safeCommand)
            {
                return new SafeCommand(
                    safeCommand.Command,
                    safeCommand.OnError.Add(onError),
                    name
                );
            }
            
            return new SafeCommand(
                command,
                new ErrorHandler(onError),
                name
            );
        }
        
        public static IAsyncCommand Safe<T>(
            this IAsyncCommand command,
            IList<Func<Exception, string?, bool>> onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeAsyncCommand<T> safeCommand)
            {
                return new SafeAsyncCommand<T>(
                    safeCommand.Command,
                    safeCommand.OnError.Add(onError),
                    name
                );
            }
            
            return new SafeAsyncCommand<T>(
                command,
                new ErrorHandler(onError),
                name
            );
        }
        
        public static ICommand Safe(
            this ICommand command,
            IErrorHandler onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeCommand safeCommand)
            {
                return new SafeCommand(
                    safeCommand.Command,
                    new MultiErrorHandler(
                        safeCommand.OnError,
                        onError
                    ),
                    name
                );
            }
            
            return new SafeCommand(
                command,
                onError,
                name
            );
        }
        
        public static IAsyncCommand Safe(
            this IAsyncCommand command,
            Action<Exception> onError,
            [CallerMemberName] string? callerName = null)
        {
            return command.Safe(
                (exception, _) =>
                {
                    onError(exception);
                    return Task.FromResult(true);
                },
                callerName
            );
        }
        
        public static IAsyncCommand Safe(
            this IAsyncCommand command, 
            Func<Exception, string?, Task<bool>> onError,
            [CallerMemberName] string? name = null)
        {
            return command.Safe(
                new List<Func<Exception, string?, Task<bool>>> { onError },
                name
            );
        }
        
        public static IAsyncCommand Safe(
            this IAsyncCommand command,
            IList<Func<Exception, string?, Task<bool>>> onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeAsyncCommand<object> safeCommand)
            {
                return new SafeAsyncCommand<object>(safeCommand.Command,safeCommand.OnError.Add(onError), name);
            }
            
            return new SafeAsyncCommand<object>(command, new ErrorHandler(onError), name);
        }
        
        public static IAsyncCommand Safe(
            this IAsyncCommand command,
            IErrorHandler onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeAsyncCommand<object> safeCommand)
            {
                return new SafeAsyncCommand<object>(
                    command,
                    new MultiErrorHandler(
                        safeCommand.OnError,
                        onError
                    ),
                    name
                );
            }
            
            return new SafeAsyncCommand<object>(command, onError, name);
        }
        
        public static IAsyncCommand<T> Safe<T>(
            this IAsyncCommand<T> command,
            IErrorHandler onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeAsyncCommand<T> safeCommand)
            {
                return new SafeAsyncCommand<T>(
                    command,
                    new MultiErrorHandler(
                        safeCommand.OnError,
                        onError
                    ),
                    name
                );
            }
            
            return new SafeAsyncCommand<T>(command, onError, name);
        }
        
        public static SafeAsyncCommand<T> Safe<T>(
            this IAsyncCommand<T> command,
            Action<Exception, string?> onError,
            [CallerMemberName] string? callerName = null)
        {
            return command.Safe(
                (exception, name) =>
                {
                    onError(exception, name);
                    return Task.FromResult(true);
                },
                callerName
            );
        }
        
        public static SafeAsyncCommand<T> Safe<T>(
            this IAsyncCommand<T> command, 
            Func<Exception, string?, Task<bool>> onError,
            [CallerMemberName] string? name = null)
        {
            return command.Safe(
                new List<Func<Exception, string?, Task<bool>>> { onError },
                name
            );
        }
        
        public static SafeAsyncCommand<T> Safe<T>(
            this IAsyncCommand<T> command,
            IList<Func<Exception, string?, Task<bool>>> onError,
            [CallerMemberName] string? name = null)
        {
            if (command is SafeAsyncCommand<T> safeCommand)
            {
                return new SafeAsyncCommand<T>(safeCommand.Command, safeCommand.OnError.Add(onError), name);
            }
            
            return new SafeAsyncCommand<T>(command, new ErrorHandler(onError), name);
        }
    }
}
