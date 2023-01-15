using System;

namespace Dotnet.Commands
{
    public class CommandCanExecute : CommandCanExecute<object>
    {
        public CommandCanExecute(Func<bool>? canExecute)
            : base(_ => CanExecute(canExecute))
        {
        }
        
        private static bool CanExecute(Func<bool>? canExecute = null)
        {
        	return canExecute == null || canExecute();
        }
    }
    
    public class CommandCanExecute<TArgument> : ICanExecute<TArgument>
    {
        private bool? _canExecutePreviously;
        private readonly Func<TArgument?, bool>? _canExecute;

        public CommandCanExecute(Func<TArgument?, bool>? canExecute)
        {
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        
        public bool CanExecute(TArgument? param)
        {
            var canExecute = _canExecutePreviously ?? true;
            if (_canExecute != null)
            {
                canExecute = _canExecute(param);
                if (canExecute != _canExecutePreviously)
                {
                    _canExecutePreviously = canExecute;
                    RaiseCanExecuteChanged();
                }
            }

            return canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new CanExecuteArgs(_canExecutePreviously ?? false));
        }
    }
}