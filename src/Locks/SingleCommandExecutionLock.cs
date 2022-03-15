using System.Threading.Tasks;

namespace Dotnet.Commands
{
	public class SingleCommandExecutionLock : ICommandExecutionLock
	{
		private readonly object _lockObject;
        private readonly int _commandExecutionInterval;
        private bool _isExecutionLock;

		public SingleCommandExecutionLock(int commandExecutionInterval)
		{
			_lockObject = new object();
			_isExecutionLock = false;
            _commandExecutionInterval = commandExecutionInterval;
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
			if (_commandExecutionInterval > 0)
            {
				await Task.Delay(_commandExecutionInterval);
			}
			
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