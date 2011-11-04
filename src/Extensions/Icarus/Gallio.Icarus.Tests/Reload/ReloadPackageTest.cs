using System.Windows.Forms;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Reload;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.WindowManager;
using Gallio.UI.ProgressMonitoring;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Reload
{
	public class ReloadPackageTest
	{
		private ReloadPackage reloadPackage;
		private IOptionsController optionsController;
		private ICommandFactory commandFactory;
		private IProjectController projectController;
		private ITaskManager taskManager;
		private IWindowManager windowManager;

		[SetUp]
		public void SetUp()
		{
			projectController = MockRepository.GenerateStub<IProjectController>();
			optionsController = MockRepository.GenerateStub<IOptionsController>();
			commandFactory = MockRepository.GenerateStub<ICommandFactory>();
			taskManager = MockRepository.GenerateStub<ITaskManager>();
			windowManager = MockRepository.GenerateStub<IWindowManager>();

			reloadPackage = new ReloadPackage(projectController, optionsController, commandFactory, taskManager, windowManager);
		}

		[SyncTest]
		public void File_changes_trigger_a_reload_if_always_reload_files_is_true()
		{
			reloadPackage.Load();
			optionsController.AlwaysReloadFiles = true;
			var reloadCommand = MockRepository.GenerateStub<ICommand>();
			StubReloadCommand(reloadCommand);

			RaiseFileChangedEvent();

			taskManager.AssertWasCalled(tm => tm.QueueTask(reloadCommand));
		}

		[SyncTest]
		public void File_changes_trigger_a_reload_if_user_wants_to()
		{
			reloadPackage.Load();
			var reloadCommand = MockRepository.GenerateStub<ICommand>();
			StubReloadCommand(reloadCommand);
			windowManager.Stub(wm => wm.ShowDialog(Arg<ReloadDialog>.Is.Anything)).Return(DialogResult.OK);

			RaiseFileChangedEvent();

			taskManager.AssertWasCalled(tm => tm.QueueTask(reloadCommand));
		}

		[SyncTest]
		public void File_changes_do_not_trigger_a_reload_if_user_does_not_want_to()
		{
			reloadPackage.Load();
			windowManager.Stub(wm => wm.ShowDialog(Arg<ReloadDialog>.Is.Anything)).Return(DialogResult.Cancel);

			RaiseFileChangedEvent();

			taskManager.AssertWasNotCalled(tm => tm.QueueTask(Arg<ICommand>.Is.Anything));
		}

		private void StubReloadCommand(ICommand reloadCommand)
		{
			commandFactory.Stub(cf => cf.CreateReloadCommand()).Return(reloadCommand);
		}

		private void RaiseFileChangedEvent()
		{
			projectController.Raise(pc => pc.FileChanged += null, projectController, new FileChangedEventArgs("fileName"));
		}
	}
}