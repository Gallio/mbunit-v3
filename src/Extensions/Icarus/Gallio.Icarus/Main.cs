// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.ProgressMonitoring;
using Gallio.Icarus.Projects;
using Gallio.Icarus.TestExplorer;
using Gallio.Icarus.Utilities;
using Gallio.Icarus.WindowManager;
using Gallio.Model;
using Gallio.Runtime;
using Gallio.UI.Common.Synchronization;
using Gallio.UI.ControlPanel;
using Gallio.UI.ProgressMonitoring;
using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class Main : Form
    {
        private readonly IApplicationController applicationController;
        private readonly IProgressController progressController;
        private readonly IReportController reportController;
        private readonly ITestController testController;
        private readonly ITaskManager taskManager;
        private readonly IProjectController projectController;
        private readonly IOptionsController optionsController;
        private readonly IWindowManager windowManager;

        // dock panel windows
        private readonly TestExplorer.View testExplorer;
        private readonly ProjectExplorer projectExplorer;

        private readonly ITestTreeModel testTreeModel;
        private readonly ITestStatistics testStatistics;
        private readonly ICommandFactory commandFactory;
        private readonly ITestFrameworkManager testFrameworkManager;

        internal Main(IApplicationController applicationController)
        {
            this.applicationController = applicationController;

            testController = RuntimeAccessor.ServiceLocator.Resolve<ITestController>();
            applicationController.RunStarted += (sender, e) => BeginInvoke((MethodInvoker) delegate
            {
                // enable/disable buttons
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
                startTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = false;
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;
            });
            applicationController.RunFinished += (sender, e) => BeginInvoke((MethodInvoker) delegate
            {
                // enable/disable buttons & menu items appropriately
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                startTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = true;
            });
            applicationController.ExploreFinished += (sender, e) => BeginInvoke((MethodInvoker) delegate
            {
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                startTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = true;
            });
            applicationController.TestsFailed += (s, e) => BeginInvoke((MethodInvoker) Activate);

            projectController = RuntimeAccessor.ServiceLocator.Resolve<IProjectController>();

            projectController.FileChanged += OnFileChanged;
            projectController.ProjectChanged += OnProjectChanged;

            taskManager = RuntimeAccessor.ServiceLocator.Resolve<ITaskManager>();
            optionsController = RuntimeAccessor.ServiceLocator.Resolve<IOptionsController>();
            reportController = RuntimeAccessor.ServiceLocator.Resolve<IReportController>();

            testTreeModel = RuntimeAccessor.ServiceLocator.Resolve<ITestTreeModel>();
            testStatistics = RuntimeAccessor.ServiceLocator.Resolve<ITestStatistics>();

            var testExplorerModel = RuntimeAccessor.ServiceLocator.Resolve<IModel>();
            var testExplorerController = RuntimeAccessor.ServiceLocator.Resolve<IController>();
            testExplorer = new TestExplorer.View(testExplorerController, testExplorerModel);

            commandFactory = RuntimeAccessor.ServiceLocator.Resolve<ICommandFactory>();
            testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            windowManager = RuntimeAccessor.ServiceLocator.Resolve<IWindowManager>();    

            projectExplorer = new ProjectExplorer(projectController, testController, reportController, taskManager, 
                commandFactory, windowManager);

            // moved this below the service locator calls as the optionsController was being used _before_ it was initialised :(
            // TODO: remove as many dependencies from the shell as possible
            InitializeComponent();

            SetupReportMenus();

            SetupRecentProjects();

            applicationController.PropertyChanged += (sender, e) => 
            {
                switch (e.PropertyName)
                {
                    case "ProjectFileName":
                        Text = applicationController.Title;
                        break;

                    case "RecentProjects":
                        SetupRecentProjects();
                        break;
                }
            };

            progressController = RuntimeAccessor.ServiceLocator.Resolve<IProgressController>();
            progressController.Status.PropertyChanged += (s, e) =>
            {
                toolStripStatusLabel.Text = progressController.Status;
            };
            progressController.TotalWork.PropertyChanged += (s, e) =>
            {
                toolStripProgressBar.TotalWork = progressController.TotalWork;
            };
            progressController.CompletedWork.PropertyChanged += (s, e) =>
            {
                toolStripProgressBar.CompletedWork = progressController.CompletedWork;
            };
            progressController.DisplayProgressDialog += (s, e) => BeginInvoke((MethodInvoker) (() => 
                new ProgressMonitorDialog(e.ProgressMonitor).Show(this)));
        }

        private static bool RunningOnWin7()
        {
            return (Environment.OSVersion.Version.Major > 6) ||
                (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1);
        }

        private void SetupReportMenus()
        {
            // add a menu item for each report type (Report -> View As)
            var reportTypes = new List<string>();
            reportTypes.AddRange(reportController.ReportTypes);
            reportTypes.Sort();
            foreach (string reportType in reportTypes)
            {
                var menuItem = new ToolStripMenuItem { Text = reportType };
                menuItem.Click += delegate 
                {
                    var command = commandFactory.CreateShowReportCommand(menuItem.Text);
                    taskManager.QueueTask(command);
                };
                viewAsToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void SetupRecentProjects()
        {            
            recentProjectsToolStripMenuItem.DropDownItems.Clear();
            recentProjectsToolStripMenuItem.DropDownItems.AddRange(applicationController.RecentProjects);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            // TODO: once these have moved to packages this will no longer be req
            if (persistString == typeof(TestExplorer.View).ToString())
                return testExplorer;
            if (persistString == typeof(ProjectExplorer).ToString())
                return projectExplorer;

            return windowManager.Get(persistString);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // provide WindowsFormsSynchronizationContext for cross-thread databinding
            SynchronizationContext.Current = System.Threading.SynchronizationContext.Current;

            Text = applicationController.Title;

            // setup window manager
            var manager = (WindowManager.WindowManager) windowManager;
            manager.SetDockPanel(dockPanel);
            var menuManager = (MenuManager)manager.MenuManager;
            menuManager.SetToolstrip(menuStrip.Items);

            // deal with arguments
            applicationController.Load();

            // try to load the dock state, if the file does not exist
            // or loading fails then use defaults.
            try
            {
                dockPanel.LoadFromXml(Paths.DockConfigFile, GetContentFromPersistString);
            }
            catch
            {
                DefaultDockState();
            }

            RestoreWindowSizeAndLocation();

            if (RunningOnWin7())
                new Win7TaskBar(Handle, (ITaskbarList4)new CTaskbarList(), testTreeModel, testStatistics);
        }

        private void RestoreWindowSizeAndLocation()
        {
            if (!optionsController.Size.Equals(Size.Empty))
                Size = optionsController.Size;

            if (optionsController.WindowState == FormWindowState.Minimized)
                return;

            WindowState = optionsController.WindowState;

            if (optionsController.WindowState == FormWindowState.Maximized)
                return;

            var desktop = new Rectangle();

            foreach (var screen in Screen.AllScreens)
                desktop = Rectangle.Union(desktop, screen.WorkingArea);

            if (desktop.Contains(optionsController.Location))
                Location = optionsController.Location;
        }

        private void DefaultDockState()
        {
            // We show the test results, execution log, project explorer and annotations
            // by default in order to draw the user's attention to these elements.
            // I've seen users get lost trying to add/remove files when only presented
            // with the test explorer.  Likewise I've seen them confused when tests
            // won't run due to an error that could be diagnosed in the annotation window
            // or in the runtime log.  Auto-hidden panels are less likely to be
            // looked at than regular tabs.
            // -- Jeff.
            projectExplorer.Show(dockPanel, DockState.DockLeft);
            testExplorer.Show(dockPanel, DockState.DockLeft);

            windowManager.ShowDefaults();
        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            using (var aboutDialog = new AboutDialog(new AboutController(testFrameworkManager)))
                aboutDialog.ShowDialog(this);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            StartTests(false);
        }

        private void StartTests(bool attachDebugger)
        {
            var command = commandFactory.CreateRunTestsCommand(attachDebugger);
            taskManager.QueueTask(command);
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void Reload()
        {
            var command = commandFactory.CreateReloadCommand();
            taskManager.QueueTask(command);
        }

        private void openProject_Click(object sender, EventArgs e)
        {
            using (var openProjectDialog = Dialogs.CreateOpenProjectDialog())
            {
                if (openProjectDialog.ShowDialog() != DialogResult.OK)
                    return;

                applicationController.OpenProject(openProjectDialog.FileName);
            }
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Dialogs.CreateSaveProjectDialog())
            {
                SaveProject(true);
            }
        }

        private void SaveProject(Boolean saveAs)
        {
            if (saveAs || applicationController.DefaultProject )
            {
                using (var saveProjectDialog = Dialogs.CreateSaveProjectDialog())
                {
                    if (saveProjectDialog.ShowDialog() != DialogResult.OK)
                        return;

                    applicationController.Title = saveProjectDialog.FileName;
                }                
            }
            applicationController.SaveProject(true);
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFiles();
        }

        private void AddFiles()
        {
            using (var openFileDialog = Dialogs.CreateAddFilesDialog())
            {
                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var command = commandFactory.CreateAddFilesCommand(openFileDialog.FileNames);
                taskManager.QueueTask(command);
            }
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            var presenter = RuntimeAccessor.ServiceLocator.Resolve<IControlPanelPresenter>();
            presenter.Show(this);
        }

        private void removeAllFiles_Click(object sender, EventArgs e)
        {
            RemoveAllFiles();
        }

        private void RemoveAllFiles()
        {
            var command = commandFactory.CreateRemoveAllFilesCommand();
            taskManager.QueueTask(command);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            progressController.Cancel();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject(false);
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void CreateNewProject()
        {
            applicationController.NewProject();
        }

        private void newProjectToolStripButton_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressController.Cancel();
        }

        private void startTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTests(false);
        }

        private void showOnlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOnlineHelp();
        }

        private static void ShowOnlineHelp()
        {
            Process.Start(ConfigurationManager.AppSettings["OnlineHelpURL"]);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var command = commandFactory.CreateResetTestsCommand();
            taskManager.QueueTask(command);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            try
            {
                optionsController.Location = Location;
                optionsController.WindowState = WindowState;

                // shut down any running operations
                progressController.Cancel();

                applicationController.Shutdown();

                // save dock panel config
                dockPanel.SaveAsXml(Paths.DockConfigFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            ShowWindow(item.Name);
        }

        private void ShowWindow(string window)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                switch (window)
                {
                    case "projectExplorerToolStripMenuItem":
                        projectExplorer.Show(dockPanel);
                        break;
                    case "testExplorerToolStripMenuItem":
                        testExplorer.Show(dockPanel);
                        break;
                }
            });
        }

        private void OnFileChanged(object sender, FileChangedEventArgs e)
        {
            // Do this asynchronously when called from another thread.
            BeginInvoke(new MethodInvoker(() => HandleFileChanged(e.FileName)));
        }

        private void HandleFileChanged(string fileName)
        {
            if (!optionsController.AlwaysReloadFiles)
            {
                var reloadDialog = new ReloadDialog(fileName, optionsController);
                if (reloadDialog.ShowDialogIfNotVisible(this) != DialogResult.OK)
                    return;
            }
            Reload();
        }

        private void startWithDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTests(true);
        }

        private void saveProjectToolStripButton_Click(object sender, EventArgs e)
        {
            SaveProject(false);
        }

        private void addFilesToolStripButton_Click(object sender, EventArgs e)
        {
            AddFiles();
        }

        private void OnProjectChanged(object sender, ProjectChangedEventArgs e)
        {
            BeginInvoke(new MethodInvoker(() => HandleProjectChanged(e.ProjectLocation)));
        }

        private void HandleProjectChanged(string projectLocation)
        {
            var projectName = Path.GetFileNameWithoutExtension(projectLocation);
            using (var projectReloadDialog = new ProjectReloadDialog(projectName))
            {
                if (projectReloadDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var command = commandFactory.CreateOpenProjectCommand(projectLocation);
                taskManager.QueueTask(command);
            }
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            optionsController.Size = Size;
        }
    }
}
