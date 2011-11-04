using System;
using System.Windows.Forms;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Projects;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.WindowManager;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Projects
{
	public class ProjectReloadPackageTest
	{
		private ProjectReloadPackage package;
		private IProjectController projectController;
		private IWindowManager windowManager;
		private ICommandFactory commandFactory;
		private ITaskManager taskManager;

		[SetUp]
		public void SetUp()
		{
			projectController = MockRepository.GenerateStub<IProjectController>();
			windowManager = MockRepository.GenerateStub<IWindowManager>();
			commandFactory = MockRepository.GenerateStub<ICommandFactory>();
			taskManager = MockRepository.GenerateStub<ITaskManager>();		

			package = new ProjectReloadPackage(projectController, windowManager, commandFactory, taskManager);
		}

		[SyncTest]
		public void Reload_project_if_user_so_wishes()
		{
			const string projectLocation = "projectLocation";
			package.Load();
			windowManager.Stub(wm => wm.ShowDialog(Arg<ProjectReloadDialog>.Is.Anything)).Return(DialogResult.OK);
			var openProjectCommand = MockRepository.GenerateStub<ICommand>();
			commandFactory.Stub(cf => cf.CreateOpenProjectCommand(projectLocation)).Return(openProjectCommand);

			Raise(new ProjectChangedEventArgs(projectLocation));

			taskManager.AssertWasCalled(tm => tm.QueueTask(openProjectCommand));
		}

		[SyncTest]
		public void Do_not_reload_project_if_user_cancels()
		{
			package.Load();
			windowManager.Stub(wm => wm.ShowDialog(Arg<ProjectReloadDialog>.Is.Anything)).Return(DialogResult.Cancel);

			Raise(new ProjectChangedEventArgs("projectLocation"));

			taskManager.AssertWasNotCalled(tm => tm.QueueTask(Arg<ICommand>.Is.Anything));
		}

		private void Raise(EventArgs eventArgs)
		{
			projectController.Raise(pc => pc.ProjectChanged += null, projectController, eventArgs);
		}
	}
}