using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class SafeAsyncCommand<TArgument> : IAsyncCommand<TArgument>
    {
        internal readonly IAsyncCommand _command;
        private readonly Func<Exception, string?, bool> _onError;
        private readonly string? _name;


        public SafeAsyncCommand(
            IAsyncCommand command,
            Func<Exception, string?, bool> onError, 
            [CallerMemberName] string? name = null)
        {
            _command = command;
            _onError = onError;
            _name = name;
        }
        
        public event EventHandler? CanExecuteChanged
        {
            add => _command.CanExecuteChanged += value;
            remove => _command.CanExecuteChanged -= value;
        }

        public Exception? Exception { get; private set; }
        
        void IAsyncCommand.RaiseCanExecuteChanged()
        {
            _command.RaiseCanExecuteChanged();
        }

        public void Cancel()
        {
            _command?.Cancel();
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                Exception = null;
                return _command.CanExecute((TArgument)parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!_onError(e, _name))
                {
                    throw;
                }

                return false;
            }
            
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                Execute((TArgument)parameter);
            }
            else
            {
                if (parameter is TArgument argument)
                {
                    Execute(argument);
                }
                else
                {
                    Execute((TArgument)Convert.ChangeType(parameter, typeof(TArgument)));
                }
            }
        }

        public void Execute(TArgument? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        public Task<bool> ExecuteAsync(object? parameter)
        {
            if (parameter == null)
            {
                return ExecuteAsync((TArgument?)parameter);
            }
            
            if (parameter is TArgument argument)
            {
                return ExecuteAsync(argument);
            }

            return ExecuteAsync((TArgument)Convert.ChangeType(parameter, typeof(TArgument)));
        }

        public async Task<bool> ExecuteAsync(TArgument? parameter)
        {
            try
            {
                Exception = null;
                return await _command.ExecuteAsync(parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!_onError(e, _name))
                {
                    throw;
                }

                return false;
            }
        }
    }
}