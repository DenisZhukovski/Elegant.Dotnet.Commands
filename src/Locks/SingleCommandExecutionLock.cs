using System.Threading.Tasks;

namespace Dotnet.Commands
{
	public class SingleCommandExecutionLock : ICommandExecutionLock
	{
		/// <summary>
		/// This interval is necessary to avoid multi tapping command from the user
		/// It can happen when user clicks simuntainiusly on several buttons on the screen
		/// </summary>
		public static int CommandExecutionInterval = 300;
		private readonly object _lockObject;
		private bool _isExecutionLock;

		public SingleCommandExecutionLock()
		{
			_lockObject = new object();
			_isExecutionLock = false;
		}

		public bool IsLocked
		{
			get
			{
				lock (_lockObject)
				{
					return _isExecutionLock;
				}
			}
		}

		public bool TryLockExecution()
		{
			if (_isExecutionLock)
			{
				return false;
			}

			lock (_lockObject)
			{
				if (_isExecutionLock)
				{
					return false;
				}

				return _isExecutionLock = true;
			}
		}

		public async Task<bool> FreeExecutionLock()
		{
			await Task.Delay(CommandExecutionInterval);
			if (!_isExecutionLock)
			{
				return false;
			}

			lock (_lockObject)
			{
				if (!_isExecutionLock)
				{
					return false;
				}


				_isExecutionLock = false;
				return true;
			}
		}
	}
}