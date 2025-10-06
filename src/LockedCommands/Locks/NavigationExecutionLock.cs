using System.Threading.Tasks;

namespace Dotnet.Commands
{
	public class NavigationExecutionLock : ICommandExecutionLock
	{
		private readonly INavigation _navigation;
        private readonly ICommandExecutionLock _executionLock;

        public NavigationExecutionLock(INavigation navigation, ICommandExecutionLock executionLock)
		{
			_navigation = navigation;
            _executionLock = executionLock;
        }

		public bool IsLocked
		{
			get
			{
				return _navigation.IsNavigationInProgress || _executionLock.IsLocked;
			}
		}

		public bool TryLockExecution()
		{
			if (_navigation.IsNavigationInProgress)
			{
				return false;
			}

			return _executionLock.TryLockExecution();
		}

		public Task<bool> FreeExecutionLock()
		{
			return _executionLock.FreeExecutionLock();
		}
	}
}