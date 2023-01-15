using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public interface ICommands
	{
		ICommand Command(
			Action execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null);

		ICommand Command<TParam>(
			Action<TParam> execute,
			Func<TParam, bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null);
		
		IAsyncCommand AsyncCommand(
			Func<CancellationToken, Task> execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null);

		IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam?, CancellationToken, Task> execute,
			Func<TParam?, bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null);

		IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam?, CancellationToken, Task> execute,
			Func<TParam?, Task<bool>>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null);
	}
}
