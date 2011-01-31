using System.Collections.Generic;
using Gallio.Icarus.Commands;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
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

			Assert.AreEqual(files, command.Files);
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

            Assert.AreEqual(projectLocation, command.ProjectLocation);
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
            StubCommand<RestoreFilterCommand>(new RestoreFilterCommand(null, null));
            const string filterName = "filterName";

            var command = (RestoreFilterCommand)commandFactory.CreateRestoreFilterCommand(filterName);

			Assert.AreEqual(filterName, command.FilterName);
		}

        private void AssertCommandResolved<T>()
        {
            serviceLocator.AssertWasCalled(sl => sl.ResolveByComponentId(typeof(T).FullName));
        }
    }
}