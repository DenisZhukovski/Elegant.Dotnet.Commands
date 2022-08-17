using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class AsyncCommand<TypeArgument> : IAsyncCommand<TypeArgument>
    {
        private bool? _canExecutePreviously;
        private readonly Func<TypeArgument?, CancellationToken, Task> _action;
        private readonly Func<TypeArgument?, bool>? _canExecuteDelegate;
        private CancellationTokenSource? _cancellationTokenSource;

        public AsyncCommand(Func<TypeArgument?, CancellationToken, Task> action, Func<TypeArgument?, bool>? canExecute)
        {
            _action = action;
            _canExecuteDelegate = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        void IAsyncCommand.RaiseCanExecuteChanged()
        {
            RaiseCanExecuteChanged();
        }

        public bool CanExecute(TypeArgument? parameter)
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

        public void Execute(object parameter)
        {
            Execute((TypeArgument)parameter);
        }

        public void Execute(TypeArgument? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object? parameter)
        {
            return ExecuteAsync((TypeArgument?)parameter);
        }

        public Task ExecuteAsync(TypeArgument? parameter)
        {
            if (CanExecute(parameter))
            {
                // Cancel the previous operation, if one is pending
                _cancellationTokenSource?.Cancel();

                var cancellationTokenSource = _cancellationTokenSource = new CancellationTokenSource();
                return _action(parameter, cancellationTokenSource.Token);
            }

            return Task.CompletedTask;
        }

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
