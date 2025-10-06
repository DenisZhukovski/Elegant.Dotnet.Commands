using System;

namespace Dotnet.Commands
{
    public class CanExecuteArgs : EventArgs
    {
        public CanExecuteArgs(bool canExecute)
        {
            CanExecute = canExecute;
        }
        
        public bool CanExecute { get; }
    }
}