using System.Threading.Tasks;

namespace Dotnet.Commands
{
	public interface ICommandExecutionLock
	{
		bool TryLockExecution();

		Task<bool> FreeExecutionLock();

		bool IsLocked { get; }
	}

    public class NoLock : ICommandExecutionLock
    {
        public bool IsLocked => false;

        public Task<bool> FreeExecutionLock()
        {
            return Task.FromResult(true);
        }

        public bool TryLockExecution()
        {
            return true;
        }
    }
}