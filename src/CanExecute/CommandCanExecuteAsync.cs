using System;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class CommandCanExecuteAsync<TArgument> : ICanExecuteAsync<TArgument>, ICanExecute<TArgument>
    {
        private bool? _canExecutePreviously;
        private readonly Func<TArgument?, Task<bool>>? _canExecute;
        private bool? _canExecuteValue;
        
        public CommandCanExecuteAsync(Func<TArgument?, Task<bool>>? canExecute)
        {
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        
        public async Task<bool> CanExecute(TArgument? param)
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

        bool ICanExecute<TArgument>.CanExecute(TArgument? param)
        {
            if (_canExecuteValue.HasValue)
            {
                var result = _canExecuteValue.Value;
                _canExecuteValue = null;
                return result;
            }

            _ = Task.Run(async () =>
            {
                await CanExecute(param);
                /*
                 * By default when can execute has been changed the internal subscription should
                 * update _canExecuteValue field but if it hasn't happened the code has to raise the event
                 * to complete the flow properly.
                 */ 
                if (!_canExecuteValue.HasValue)
                {
                    RaiseCanExecuteChanged();
                }
            });
            return false;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new CanExecuteArgs(_canExecutePreviously ?? false));
        }
    }
}