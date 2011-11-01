using System.Drawing;
using Gallio.Common;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ProjectExplorer;
using Gallio.Icarus.Tests.WindowManager;
using Gallio.Icarus.WindowManager;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.UI.Menus;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using NHamcrest.Core;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.ProjectExplorer
{
    public class ProjectExplorerPackageTests
    {
        private IWindowManager windowManager;
        private IMenuManager menuManager;
        private ProjectExplorerPackage package;
        private MenuCommand menuCommand;

        [SetUp]
        public void SetUp()
        {
            windowManager = TestWindowManager.Create();

            menuManager = MockRepository.GenerateStub<IMenuManager>();
            windowManager.Stub(wm => wm.MenuManager).Return(menuManager);
            menuManager.Stub(mm => mm.Add(Arg<string>.Is.Anything, Arg<Func<MenuCommand>>.Is.Anything))
                .Do((Action<string, Func<MenuCommand>>)((m, f) => menuCommand = f()));

            var projectController = MockRepository.GenerateStub<IProjectController>();
            var reportController = MockRepository.GenerateStub<IReportController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
        	var commandFactory = MockRepository.GenerateStub<ICommandFactory>();
        	
			package = new ProjectExplorerPackage(windowManager, projectController, reportController, taskManager, commandFactory);
        }

        [Test]
        public void Load_registers_action_with_correct_id()
        {
            package.Load();

			windowManager.AssertWasCalled(wm => wm.Register(Arg.Is(ProjectExplorerPackage.WindowId), 
                Arg<Action>.Is.Anything, Arg<Location>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_to_be_added()
        {
            package.Load();

			windowManager.AssertWasCalled(wm => wm.Add(Arg.Is(ProjectExplorerPackage.WindowId), Arg<ProjectExplorerView>.Is.Anything,
                Arg.Is(ProjectExplorerResources.Project_Explorer), Arg<Icon>.Is.Anything));
        }

        [Test]
        public void Load_registers_window_with_correct_default_location()
        {
            package.Load();

            windowManager.AssertWasCalled(wm => wm.Register(Arg<string>.Is.Anything, 
                Arg<Action>.Is.Anything, Arg.Is(Location.Left)));
        }

        [Test]
        public void Load_adds_menu_item_to_correct_menu()
        {
            package.Load();

            menuManager.AssertWasCalled(mm => mm.Add(Arg.Is("View"), Arg<Func<MenuCommand>>.Is.Anything));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_text()
        {
            package.Load();

            Assert.That(menuCommand, Is.NotNull());
            Assert.That(menuCommand.Text, Is.EqualTo(ProjectExplorerResources.Project_Explorer));
        }

        [Test]
        public void Load_adds_menu_item_with_correct_command()
        {
            package.Load();

            Assert.That(menuCommand, Is.NotNull());
            menuCommand.Command.Execute(NullProgressMonitor.CreateInstance());
            windowManager.AssertWasCalled(wm => wm.Show(ProjectExplorerPackage.WindowId));
        }
    }
}
