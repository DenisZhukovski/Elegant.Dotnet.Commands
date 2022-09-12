using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class AsyncCommand<TArgument> : IAsyncCommand<TArgument>
    {
        private readonly Func<TArgument?, CancellationToken, Task> _action;
        private readonly ICanExecuteAsync<TArgument> _canExecute;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool? _canExecuteValue;

        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action,
            Func<TArgument?, Task<bool>>? canExecute = null)
            : this(action, new CanExecute<TArgument>(canExecute))
        {
        }
        
        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action,
            Func<TArgument?, bool>? canExecute = null)
        : this(action, new CanExecute<TArgument>(canExecute))
        {
        }
        
        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action, 
            ICanExecuteAsync<TArgument> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
            _canExecute.CanExecuteChanged += delegate(object sender, EventArgs args)
            {
                _canExecuteValue = ((CanExecuteArgs)args).CanExecute;
            };
        }

        public event EventHandler? CanExecuteChanged
        {
            add => _canExecute.CanExecuteChanged += value;
            remove => _canExecute.CanExecuteChanged -= value;
        }

        void IAsyncCommand.RaiseCanExecuteChanged()
        {
            RaiseCanExecuteChanged();
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
        
        public bool CanExecute(TArgument? parameter)
        {
            if (_canExecuteValue.HasValue)
            {
                var result = _canExecuteValue.Value;
                _canExecuteValue = null;
                return result;
            }

            _ = Task.Run(async () =>
            {
                await _canExecute.CanExecuteAsync(parameter);
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

        public bool CanExecute(object parameter)
        {
            return CanExecute((TArgument)parameter);
        }

        public void Execute(object parameter)
        {
            Execute((TArgument)parameter);
        }

        public void Execute(TArgument? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object? parameter)
        {
            return ExecuteAsync((TArgument?)parameter);
        }

        public async Task ExecuteAsync(TArgument? parameter)
        {
            if (await _canExecute.CanExecuteAsync(parameter))
            {
                // Cancel the previous operation, if one is pending
                _cancellationTokenSource?.Cancel();

                var cancellationTokenSource = _cancellationTokenSource = new CancellationTokenSource();
                await _action(parameter, cancellationTokenSource.Token);
            }
        }

        protected void RaiseCanExecuteChanged()
        {
            _canExecute.RaiseCanExecuteChanged();
        }
    }
}
