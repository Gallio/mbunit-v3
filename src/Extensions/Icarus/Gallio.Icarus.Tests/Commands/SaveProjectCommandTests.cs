using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Projects;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Commands
{
    public class SaveProjectCommandTests
    {
        private SaveProjectCommand command;
        private IProjectController projectController;
        private IProjectUserOptionsController projectUserOptionsController;
        private ICommandFactory commandFactory;
        private ICommand saveFilterCommand;

        [SetUp]
        public void SetUp()
        {
            projectController = MockRepository.GenerateStub<IProjectController>();
            projectUserOptionsController = MockRepository.GenerateStub<IProjectUserOptionsController>();
            commandFactory = MockRepository.GenerateStub<ICommandFactory>();
            saveFilterCommand = MockRepository.GenerateStub<ICommand>();
            commandFactory.Stub(cf => cf.CreateSaveFilterCommand("AutoSave"))
                .Return(saveFilterCommand);
            command = new SaveProjectCommand(projectController, projectUserOptionsController, commandFactory);
        }

        [Test]
        public void Execute_should_save_the_current_test_filter()
        {
            command.Execute(NullProgressMonitor.CreateInstance());

            saveFilterCommand.AssertWasCalled(c => c.Execute(Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_save_the_project()
        {
            const string projectLocation = "projectLocation";
            command.ProjectLocation = projectLocation;

            command.Execute(NullProgressMonitor.CreateInstance());

            projectController.AssertWasCalled(pc => pc.Save(Arg.Is(projectLocation), 
                Arg<IProgressMonitor>.Is.Anything));
        }

        [Test]
        public void Execute_should_save_the_project_user_options()
        {
            const string projectLocation = "projectLocation";
            command.ProjectLocation = projectLocation;

            command.Execute(NullProgressMonitor.CreateInstance());

            projectUserOptionsController.AssertWasCalled(uoc => uoc.SaveUserOptions(Arg.Is(projectLocation),
                Arg<IProgressMonitor>.Is.Anything));
        }
    }
}