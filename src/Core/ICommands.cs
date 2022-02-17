using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public interface ICommands
	{
		bool IsLocked { get; }

		IAsyncCommand AsyncCommand(
			Func<Task> execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false);

        IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam, Task> execute,
			Func<TParam, bool>? canExecute = null,
			bool forceExecution = false);

		ICommand Command(
			Action execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false);

		ICommand Command<TParam>(
			Action<TParam> execute,
			Func<TParam, bool>? canExecute = null,
			bool forceExecution = false);

		void ForceRelease();
	}
}
