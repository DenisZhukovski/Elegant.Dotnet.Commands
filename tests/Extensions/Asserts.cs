using System;
using System.Threading.Tasks;

namespace Dotnet.Commands.UnitTests
{
    public static class Asserts
    {
        public static async Task DoesNotThrow(Func<Task> testCode)
        {
            Xunit.Assert.Null(await RecordExceptionAsync(testCode));
        }

        private static async Task<Exception> RecordExceptionAsync(Func<Task> testCode)
        {
            try
            {
                await testCode();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}

