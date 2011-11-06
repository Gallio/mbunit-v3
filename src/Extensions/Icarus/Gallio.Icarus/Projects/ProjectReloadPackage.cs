using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Common.Synchronization;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.Icarus.Projects
{
	public class ProjectReloadPackage : IPackage
	{
		private readonly IProjectController projectController;
		private readonly IWindowManager windowManager;
		private readonly ICommandFactory commandFactory;
		private readonly ITaskManager taskManager;

		public ProjectReloadPackage(IProjectController projectController, IWindowManager windowManager, 
			ICommandFactory commandFactory, ITaskManager taskManager)
		{
			this.projectController = projectController;
			this.windowManager = windowManager;
			this.commandFactory = commandFactory;
			this.taskManager = taskManager;
		}

		public void Load()
		{
			projectController.ProjectChanged += (s, e) => HandleProjectChanged(e.ProjectLocation);
		}

		private void HandleProjectChanged(string projectLocation)
		{
			SyncContext.Post(cb =>
			{
				var projectName = Path.GetFileNameWithoutExtension(projectLocation);
				using (var projectReloadDialog = new ProjectReloadDialog(projectName))
				{
					if (windowManager.ShowDialog(projectReloadDialog) != DialogResult.OK)
						return;

					var command = commandFactory.CreateOpenProjectCommand(projectLocation);
					taskManager.QueueTask(command);
				}
			}, null);
		}

		public void Dispose() { }
	}
}