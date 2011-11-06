using System.Windows.Forms;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Common.Synchronization;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Reload
{
	public class ReloadPackage : IPackage
	{
		private readonly IProjectController projectController;
		private readonly IOptionsController optionsController;
		private readonly ICommandFactory commandFactory;
		private readonly ITaskManager taskManager;
		private readonly IWindowManager windowManager;
		private ReloadDialog reloadDialog;

		public ReloadPackage(IProjectController projectController, IOptionsController optionsController, ICommandFactory commandFactory, 
			ITaskManager taskManager, IWindowManager windowManager)
		{
			this.projectController = projectController;
			this.optionsController = optionsController;
			this.commandFactory = commandFactory;
			this.taskManager = taskManager;
			this.windowManager = windowManager;
		}

		public void Load()
		{
			projectController.FileChanged += (s, e) => FileChanged(e.FileName);
			reloadDialog = new ReloadDialog(optionsController);
		}

		private void FileChanged(string fileName)
		{
			SyncContext.Post(cb => 
			{
				if (reloadDialog.Visible)
					return;

				if (!optionsController.AlwaysReloadFiles)
				{
					reloadDialog.SetFilename(fileName);
					if (windowManager.ShowDialog(reloadDialog) != DialogResult.OK)
						return;
				}
				var command = commandFactory.CreateReloadCommand();
				taskManager.QueueTask(command);
			}, null);
		}

		public void Dispose() { }
	}
}