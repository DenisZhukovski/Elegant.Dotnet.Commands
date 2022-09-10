using System;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class CanExecute<TArgument> : ICanExecuteAsync<TArgument>
    {
        private bool? _canExecutePreviously;
        private readonly Func<TArgument?, Task<bool>>? _canExecute;

        public CanExecute(Func<TArgument?, bool>? canExecute)
            : this(param => Task.FromResult(canExecute?.Invoke(param) ?? true))
        {
        }
        
        public CanExecute(Func<TArgument?, Task<bool>>? canExecute)
        {
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        
        public async Task<bool> CanExecuteAsync(TArgument? param)
        {
            var canExecute = _canExecutePreviously ?? true;
            if (_canExecute != null)
            {
                canExecute = await _canExecute(param);
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