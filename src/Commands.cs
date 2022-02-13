using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dotnet.Commands
{
	public class Commands : ICommands
	{
		private readonly ICommandExecutionLock _commandExecutionLock;
		private long _lockIndex;

		public bool IsLocked
		{
			get { return _commandExecutionLock.IsLocked; }
		}

		public Commands()
			: this(new SingleCommandExecutionLock())
		{
		}

		public Commands(ICommandExecutionLock commandExecutionLock)
		{
			_commandExecutionLock = commandExecutionLock;
		}

		public IAsyncCommand AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
		{
			return AsyncCommand<object>(o => execute(), o => CanExecute(canExecute));
		}

		public IAsyncCommand<TParam> AsyncCommand<TParam>(Func<TParam, Task> execute, Func<TParam, bool>? canExecute = null)
		{
			Func<TParam, Task> func = async param =>
			{
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

		public ICommand Command(Action execute, Func<bool>? canExecute = null)
		{
			return Command<object>(o => execute(), o => CanExecute(canExecute));
		}

		public ICommand Command<TParam>(Action<TParam> execute, Func<TParam, bool>? canExecute = null)
		{
			Action<TParam> act = p =>
			{
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