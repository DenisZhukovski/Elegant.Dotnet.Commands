using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class ValidatedCommandsUnitTests
    {
        [Fact]
        public void CreatesCommand()
        {
            Assert.NotNull(new Commands().Validated().Command(() => { }));
        }

        [Fact]
        public void CreatesAsyncCommand()
        {
            Assert.NotNull(new Commands().Validated().AsyncCommand(() => Task.CompletedTask));
        }

        [Fact]
        public void CanExecuteFalse()
        {
            Assert.False(
                new Commands().Validated()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void CanExecuteTrue()
        {
            Assert.False(
                new Commands().Validated()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void Throws_ArgumentNullException_WhenNoAction()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Commands().Validated()
                    .Command(null, () => false)
            );
        }

        [Fact]
        public void Throws_ArgumentNullException_WhenNoAsyncAction()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new Commands().Validated()
                    .AsyncCommand(null, () => false)
            );
        }
    }
}
