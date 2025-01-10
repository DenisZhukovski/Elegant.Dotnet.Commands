using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class SafeAsyncCommand<TArgument> : IAsyncCommand<TArgument>
    {
        internal readonly IAsyncCommand _command;
        internal readonly IList<Func<Exception, string?, bool>> _onError;
        private readonly string? _name;


        public SafeAsyncCommand(
            IAsyncCommand command,
            IList<Func<Exception, string?, bool>> onError, 
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
        
        public void RaiseCanExecuteChanged()
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
                if (!e.TryToHandle(_onError, _name))
                {
                    throw;
                }

                return false;
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
                return await _command.ExecuteAsync(parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!e.TryToHandle(_onError, _name))
                {
                    throw;
                }

                return false;
            }
        }
    }
}