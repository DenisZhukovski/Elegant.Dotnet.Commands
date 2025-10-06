using System;
using System.Diagnostics.CodeAnalysis;
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

        /// <summary>
        /// Fires the <see cref="Task"/> and safely forget and in case of exception call <paramref name="onError"/> handler if any.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="onError">The on error action handler.</param>
        /// <param name="continueOnCapturedContext">sets ConfigureAwait argument.</param>
        public static async void FireAndForget(this Task task, Action<Exception>? onError = null, bool continueOnCapturedContext = false)
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
        
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
