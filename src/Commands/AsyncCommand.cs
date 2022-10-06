using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class AsyncCommand<TArgument> : IAsyncCommand<TArgument>
    {
        private readonly Func<TArgument?, CancellationToken, Task> _action;
        private readonly ICanExecute<TArgument> _canExecute;
        private CancellationTokenSource? _cancellationTokenSource;

        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action,
            Func<TArgument?, Task<bool>>? canExecute = null)
            : this(action, new CommandCanExecuteAsync<TArgument>(canExecute))
        {
        }
        
        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action,
            Func<TArgument?, bool>? canExecute = null)
        : this(action, new CommandCanExecute<TArgument>(canExecute))
        {
        }
        
        public AsyncCommand(
            Func<TArgument?, CancellationToken, Task> action, 
            ICanExecute<TArgument> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
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

        public bool CanExecute(object parameter)
        {
            return _canExecute.CanExecute((TArgument)parameter);
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
            bool canExecute;
            if (_canExecute is ICanExecuteAsync<TArgument> canExecuteAsync)
            {
                canExecute = await canExecuteAsync.CanExecute(parameter);
            }
            else
            {
                canExecute = _canExecute.CanExecute(parameter);
            }
            if (canExecute)
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
