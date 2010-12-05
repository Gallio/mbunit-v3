using System.Collections.Generic;
using Gallio.Icarus.Commands;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    public class CommandFactoryTests
    {
        private CommandFactory commandFactory;
        private IServiceLocator serviceLocator;

        [SetUp]
        public void SetUp()
        {
            serviceLocator = MockRepository.GenerateStub<IServiceLocator>();
            commandFactory = new CommandFactory(serviceLocator);
        }

        [Test]
        public void AddFilesCommand_should_have_files_set()
        {
            serviceLocator.Stub(sl => sl.ResolveByComponentId(typeof (AddFilesCommand).FullName))
                .Return(new AddFilesCommand(null, null));
            IList<string> files = new List<string> { "a", "b" };

            var command = (AddFilesCommand)commandFactory.CreateAddFilesCommand(files);

            Assert.That(command.Files, Is.EqualTo(files));
        }

        [Test]
        public void CreateLoadPackageCommand()
        {
            commandFactory.CreateLoadPackageCommand();

            AssertCommandResolved<LoadPackageCommand>();
        }

        [Test]
        public void OpenProjectCommand_should_have_project_location_set()
        {
            const string projectLocation = "projectLocation";
            StubCommand<OpenProjectCommand>(new OpenProjectCommand(null, null, null));

            var command = (OpenProjectCommand)commandFactory.CreateOpenProjectCommand(projectLocation);

            Assert.That(command.ProjectLocation, Is.EqualTo(projectLocation));
        }

        private void StubCommand<T>(object command)
        {
            serviceLocator.Stub(sl => sl.ResolveByComponentId(typeof(T).FullName))
                .Return(command);
        }

        // TEST: ...

        [Test]
        public void CreateRestoreFilterCommand()
        {
            commandFactory.CreateRestoreFilterCommand();

            AssertCommandResolved<RestoreFilterCommand>();
        }

        private void AssertCommandResolved<T>()
        {
            serviceLocator.AssertWasCalled(sl => sl.ResolveByComponentId(typeof(T).FullName));
        }
    }
}