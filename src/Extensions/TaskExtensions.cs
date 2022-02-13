using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnet.Commands
{
    public static class TaskExtensions
    {
        private static readonly TaskFactory TaskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default
        );

        public static TResult RunSync<TResult>(this Func<Task<TResult>> func)
        {
            return TaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(this Func<Task> func)
        {
            TaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(this Task task)
        {
            TaskFactory
              .StartNew(() => task)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }
    }
}
