﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xunit;

namespace Dotnet.Commands.UnitTests
{
    public class CachedCommandsUnitTests
    {
        [Fact]
        public void CreatesCommand()
        {
            Assert.NotNull(new Commands().Cached().Command(() => { }));
        }

        [Fact]
        public void CreatesAsyncCommand()
        {
            Assert.NotNull(new Commands().Cached().AsyncCommand(() => Task.CompletedTask));
        }

        [Fact]
        public void CanExecuteFalse()
        {
            Assert.False(
                new Commands().Cached()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void CanExecuteTrue()
        {
            Assert.False(
                new Commands().Cached()
                    .Command(() => { }, () => false)
                    .CanExecute(null)
            );
        }

        [Fact]
        public void ReturnsTheSameCommand()
        {
            var commands = new Commands().Cached();
            Assert.Same(
                commands.Command(() => { }),
                commands.Command(() => { })
            );
        }

        [Fact]
        public void ReturnsTheSameAsyncCommand()
        {
            var commands = new Commands().Cached();
            Assert.Same(
                commands.AsyncCommand(() => Task.CompletedTask),
                commands.AsyncCommand(() => Task.CompletedTask)
            );
        }
    }
}
