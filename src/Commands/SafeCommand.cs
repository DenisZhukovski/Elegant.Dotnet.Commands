using System;
using System.Runtime.CompilerServices;

namespace Dotnet.Commands
{
    public class SafeCommand : ICommand
    {
        private readonly Func<Exception, string?, bool> _onError;
        private readonly string? _name;
        internal readonly ICommand _command;

        public SafeCommand(ICommand command, Func<Exception, string?, bool> onError, [CallerMemberName] string? name = null)
        {
            _command = command;
            _onError = onError;
            _name = name;
        }
        
        public event EventHandler? CanExecuteChanged
        {
            add =>  _command.CanExecuteChanged += value;
            remove => _command.CanExecuteChanged -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            _command.RaiseCanExecuteChanged();
        }

        public Exception? Exception { get; private set; }
        
        public bool CanExecute(object parameter)
        {
            try
            {
                Exception = null;
                return _command.CanExecute(parameter);
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

        public void Execute(object parameter)
        {
            try
            {
                Exception = null;
                _command.Execute(parameter);
            }
            catch (Exception e)
            {
                Exception = e;
                if (!_onError(e, _name))
                {
                    throw;
                }
            }
        }
    }
}