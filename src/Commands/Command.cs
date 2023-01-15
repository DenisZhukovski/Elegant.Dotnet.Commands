using System;
using System.Windows.Input;

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

        public bool CanExecute(object parameter)
        {
            return _canExecute.CanExecute((TypeArgument)parameter);
        }

        public void Execute(object parameter)
        {
            Execute((TypeArgument)parameter);
        }

        protected void RaiseCanExecuteChanged()
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
