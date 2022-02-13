using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public interface ICommands
	{
		bool IsLocked { get; }

		IAsyncCommand AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null);

        IAsyncCommand<TParam> AsyncCommand<TParam>(Func<TParam, Task> execute, Func<TParam, bool>? canExecute = null);

		ICommand Command(Action execute, Func<bool>? canExecute = null);

		ICommand Command<TParam>(Action<TParam> execute, Func<TParam, bool>? canExecute = null);

		void ForceRelease();
	}
}
