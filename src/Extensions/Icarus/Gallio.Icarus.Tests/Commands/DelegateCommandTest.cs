using System;
using Gallio.Icarus.Commands;
using Gallio.Runtime.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    [TestsOn(typeof(DelegateCommand))]
    public class DelegateCommandTest
    {
        [Test]
        public void Ctor_should_throw_if_action_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DelegateCommand(null));
        }

        [Test]
        public void Execute_should_run_action()
        {
            bool flag = false;
            var command = new DelegateCommand(pm => flag = true);

            command.Execute(MockRepository.GenerateStub<IProgressMonitor>());

            Assert.IsTrue(flag);
        }

        [Test]
        public void Execute_should_run_action_with_progress_monitor()
        {
            var monitor = MockRepository.GenerateStub<IProgressMonitor>();
            var command = new DelegateCommand(pm => Assert.AreEqual(monitor, pm));
            
            command.Execute(monitor);
        }
    }
}
