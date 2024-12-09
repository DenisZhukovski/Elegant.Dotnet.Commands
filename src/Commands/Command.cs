using System;

namespace Dotnet.Commands
{
    public class Command<TypeArgument> : ICommand
    {
        private readonly Action<TypeArgument> _action;
        private readonly ICanExecute<TypeArgument> _canExecute;

        public Command(Action action, ICanExecute<TypeArgument> canExecute)
            : this(_ => action(), canExecute)
        {
        }
        
        public Command(Action<TypeArgument> action, ICanExecute<TypeArgument> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }
        
        public event EventHandler? CanExecuteChanged
        {
            add => _canExecute.CanExecuteChanged += value;
            remove => _canExecute.CanExecuteChanged -= value;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
            {
                return _canExecute.CanExecute((TypeArgument?)parameter);
            }

            if (parameter is TypeArgument argument)
            {
                return _canExecute.CanExecute(argument);
            }
            return _canExecute.CanExecute((TypeArgument)Convert.ChangeType(parameter, typeof(TypeArgument)));
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
            {
                Execute((TypeArgument)parameter);
            }
            else
            {
                if (parameter is TypeArgument argument)
                {
                    Execute(argument);
                }
                else
                {
                    Execute((TypeArgument)Convert.ChangeType(parameter, typeof(TypeArgument)));
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            _canExecute.RaiseCanExecuteChanged();
        }
        
        private void Execute(TypeArgument parameter)
        {
            if (CanExecute(parameter))
            {
                _action(parameter);
            }
        }
    }
}
