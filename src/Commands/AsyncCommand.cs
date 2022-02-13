using System;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public class AsyncCommand<TypeArgument> : IAsyncCommand<TypeArgument>
    {
        private bool? _canExecutePreviously;
        private readonly Func<TypeArgument, Task> _action;
        private readonly Func<TypeArgument, bool>? _canExecuteDelegate;

        public AsyncCommand(Func<TypeArgument, Task> action, Func<TypeArgument, bool>? canExecute)
        {
            _action = action;
            _canExecuteDelegate = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        void IAsyncCommand.RaiseCanExecuteChanged()
        {
            RaiseCanExecuteChanged();
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

        public void Execute(object parameter)
        {
            Execute((TypeArgument)parameter);
        }

        public void Execute(TypeArgument parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        public Task ExecuteAsync(object parameter)
        {
            return ExecuteAsync((TypeArgument)parameter);
        }

        protected void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public Task ExecuteAsync(TypeArgument parameter)
        {
            if (CanExecute(parameter))
            {
                return _action(parameter);
            }

            return Task.CompletedTask;
        }
    }
}
