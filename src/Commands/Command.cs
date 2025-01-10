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
            return parameter switch
            {
                null => _canExecute.CanExecute((TypeArgument?)parameter),
                TypeArgument argument => _canExecute.CanExecute(argument),
                _ => _canExecute.CanExecute((TypeArgument)Convert.ChangeType(parameter, typeof(TypeArgument)))
            };
        }

        public void Execute(object? parameter)
        {
            switch (parameter)
            {
                case null:
                    Execute((TypeArgument)parameter);
                    break;
                case TypeArgument argument:
                    Execute(argument);
                    break;
                default:
                    Execute((TypeArgument)Convert.ChangeType(parameter, typeof(TypeArgument)));
                    break;
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
