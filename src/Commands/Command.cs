using System;
using System.Windows.Input;

namespace Dotnet.Commands
{
    public class Command<TypeArgument> : ICommand
    {
        private bool? _canExecutePreviously;
        private readonly Action<TypeArgument> _action;
        private readonly Func<TypeArgument, bool>? _canExecuteDelegate;

        public event EventHandler? CanExecuteChanged;

        public Command(Action<TypeArgument> action, Func<TypeArgument, bool>? canExecute)
        {
            _action = action;
            _canExecuteDelegate = canExecute;
        }

        public bool CanExecute(TypeArgument parameter)
        {
            var canExecute = _canExecutePreviously ?? true;
            if (_canExecuteDelegate != null)
            {
                canExecute = _canExecuteDelegate?.Invoke(parameter) ?? true;
                if (canExecute != _canExecutePreviously)
                {
                    _canExecutePreviously = canExecute;
                    RaiseCanExecuteChanged();
                }
            }
            
            return canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute((TypeArgument)parameter);
        }

        public void Execute(TypeArgument parameter)
        {
            if (CanExecute(parameter))
            {
                _action(parameter);
            }
        }

        public void Execute(object parameter)
        {
            Execute((TypeArgument)parameter);
        }

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new CanExecuteArgs(_canExecutePreviously ?? false));
        }
    }
}
