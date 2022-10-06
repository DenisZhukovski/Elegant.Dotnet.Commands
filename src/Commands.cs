using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public class Commands : ICommands
	{
		/// <summary>
		/// This interval is necessary to avoid multi tapping command from the user
		/// It can happen when user clicks simuntainiusly on several buttons on the screen
		/// </summary>
		public static int DefaultCommandExecutionInterval = 300;

		private readonly ICommandExecutionLock _commandExecutionLock;
		private long _lockIndex;

		public bool IsLocked
		{
			get { return _commandExecutionLock.IsLocked; }
		}

		public Commands()
			: this(DefaultCommandExecutionInterval)
        {
        }

		public Commands(int commandExecutionInterval)
			: this(new SingleCommandExecutionLock(commandExecutionInterval))
		{
		}

		public Commands(ICommandExecutionLock commandExecutionLock)
		{
			_commandExecutionLock = commandExecutionLock;
		}

		public IAsyncCommand AsyncCommand(
			Func<CancellationToken, Task> execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null)
		{
			return AsyncCommand<object>(
				(_, ct) => execute(ct),
				_ => CanExecute(canExecute),
				forceExecution
			);
		}

		public IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam?, CancellationToken, Task> execute,
			Func<TParam?, bool>? canExecute,
			bool forceExecution = false,
			[CallerMemberName] string? name = null)
		{
			return AsyncCommand(
				execute,
				new CommandCanExecute<TParam>(canExecute),
				forceExecution: forceExecution
			);
		}

		public IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam?, CancellationToken, Task> execute, 
			Func<TParam?, Task<bool>>? canExecute = null, 
			bool forceExecution = false,
			string? name = null)
		{
			return AsyncCommand(
				execute,
				new CommandCanExecuteAsync<TParam>(canExecute),
				forceExecution: forceExecution
			);
		}

		public IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam?, CancellationToken, Task> execute,
			ICanExecute<TParam> canExecute,
			bool forceExecution = false,
			[CallerMemberName] string? name = null)
		{
			Func<TParam?, CancellationToken, Task > func = async (param, ct) =>
			{
				if (forceExecution)
                {
					await ExceptionHandledExecution(execute, param, ct);
					return;
				}
				if (IsLocked)
				{
					return;
				}

				long currentLockIndex = 0;
				try
				{
					if (!_commandExecutionLock.TryLockExecution())
					{
						return;
					}

					currentLockIndex = Interlocked.Increment(ref _lockIndex);
					await ExceptionHandledExecution(execute, param, ct);
				}
				finally
				{
					if (Interlocked.Read(ref _lockIndex) == currentLockIndex)
					{
						_ = _commandExecutionLock.FreeExecutionLock();
					}
				}
			};

			return new AsyncCommand<TParam>(
				(o, ct) => func(o, ct),
				canExecute
			);
		}

		public void ForceRelease()
		{
			Interlocked.Increment(ref _lockIndex);
			_commandExecutionLock.FreeExecutionLock();
		}

		public ICommand Command(
			Action execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null)
		{
			return Command<object>(
				_ => execute(),
				_ => CanExecute(canExecute),
				forceExecution
			);
		}

		public ICommand Command<TParam>(
			Action<TParam> execute,
			Func<TParam, bool>? canExecute = null,
			bool forceExecution = false,
			[CallerMemberName] string? name = null)
		{
			Action<TParam> act = p =>
			{
				if (forceExecution)
                {
					execute(p);
					return;
				}

				if (_commandExecutionLock.IsLocked)
				{
					return;
				}

				try
				{
					if (!_commandExecutionLock.TryLockExecution())
					{
						return;
					}

					execute(p);
				}
				finally
				{
					_commandExecutionLock.FreeExecutionLock();
				}
			};

			return new Command<TParam>(act, par => CanExecute(par, canExecute));
		}

		protected virtual bool HandleException(Exception e)
		{
			return false;
		}

		private async Task ExceptionHandledExecution<TParam>(
			Func<TParam, CancellationToken, Task> execute, 
			TParam param, 
			CancellationToken cancellationToken)
		{
			try
			{
				await execute(param, cancellationToken);
			}
			catch (Exception e)
			{
				if (!HandleException(e))
				{
					throw;
				}
			}
		}

		private bool CanExecute<TParam>(TParam par, Func<TParam, bool>? canExecute = null)
		{
			return canExecute == null || canExecute(par);
		}

		private bool CanExecute(Func<bool>? canExecute = null)
		{
			return canExecute == null || canExecute();
		}
	}
}