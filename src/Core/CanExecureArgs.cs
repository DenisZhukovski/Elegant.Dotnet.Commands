using System;

namespace Dotnet.Commands
{
    public class CanExecureArgs : EventArgs
    {
        public CanExecureArgs(bool canExecute)
        {
            CanExecute = canExecute;
        }
        
        public bool CanExecute { get; }
    }
}