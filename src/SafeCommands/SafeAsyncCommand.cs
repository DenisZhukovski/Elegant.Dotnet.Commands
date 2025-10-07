using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class SafeAsyncCommand<TArgument> : IAsyncCommand<TArgument>
    {
        internal readonly IAsyncCommand Command;
        internal readonly IErrorHandler OnError;
        private readonly string? _name;


        public SafeAsyncCommand(
            IAsyncCommand command,
            IErrorHandler onError, 
            [CallerMemberName] string? name = null)
        {
            Command = command;
            OnError = onError;
            _name = name;
        }
        
        public event EventHandler? CanExecuteChanged
        {
            add => Command.CanExecuteChanged += value;
            remove => Command.CanExecuteChanged -= value;
        }

        public Exception? Exception { get; private set; }
        
        public void RaiseCanExecuteChanged()
        {
            Command.RaiseCanExecuteChanged();
        }

        public void Cancel()
        {
            Command?.Cancel();
        }

        public bool CanExecute(object? parameter)
        {
            try
            {
                Exception = null;
                return Command.CanExecute((TArgument)parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                return OnError.Handle(e, _name);
            }
        }

        public void Execute(object? parameter)
        {
            switch (parameter)
            {
                case null:
                    Execute((TArgument)parameter);
                    break;
                case TArgument argument:
                    Execute(argument);
                    break;
                default:
                    Execute((TArgument)Convert.ChangeType(parameter, typeof(TArgument)));
                    break;
            }
        }

        public void Execute(TArgument? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        public Task<bool> ExecuteAsync(object? parameter)
        {
            return parameter switch
            {
                null => ExecuteAsync((TArgument?)parameter),
                TArgument argument => ExecuteAsync(argument),
                _ => ExecuteAsync((TArgument)Convert.ChangeType(parameter, typeof(TArgument)))
            };
        }

        public async Task<bool> ExecuteAsync(TArgument? parameter)
        {
            try
            {
                Exception = null;
                return await Command.ExecuteAsync(parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!await OnError.HandleAsync(e, _name))
                {
                    throw;
                }

                return false;
            }
        }
    }
}