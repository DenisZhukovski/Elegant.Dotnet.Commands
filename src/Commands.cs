using System;
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
			Func<Task> execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false)
		{
			return AsyncCommand<object>(
				o => execute(),
				o => CanExecute(canExecute),
				forceExecution
			);
		}

		public IAsyncCommand<TParam> AsyncCommand<TParam>(
			Func<TParam, Task> execute,
			Func<TParam, bool>? canExecute = null,
			bool forceExecution = false)
		{
			Func<TParam, Task> func = async param =>
			{
				if (forceExecution)
                {
					await ExceptionHandledExecution(execute, param);
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
					await ExceptionHandledExecution(execute, param);
				}
				finally
				{
					if (Interlocked.Read(ref _lockIndex) == currentLockIndex)
					{
						_ = _commandExecutionLock.FreeExecutionLock();
					}
				}
			};

			return new AsyncCommand<TParam>(o => func(o), par => CanExecute(par, canExecute));
		}

		public void ForceRelease()
		{
			Interlocked.Increment(ref _lockIndex);
			_commandExecutionLock.FreeExecutionLock();
		}

		public ICommand Command(
			Action execute,
			Func<bool>? canExecute = null,
			bool forceExecution = false)
		{
			return Command<object>(
				o => execute(),
				o => CanExecute(canExecute),
				forceExecution
			);
		}

		public ICommand Command<TParam>(
			Action<TParam> execute,
			Func<TParam, bool>? canExecute = null,
			bool forceExecution = false)
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

		private async Task ExceptionHandledExecution<TParam>(Func<TParam, Task> execute, TParam param)
		{
			try
			{
				await execute(param);
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