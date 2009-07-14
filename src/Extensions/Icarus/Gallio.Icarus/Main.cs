// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Text;
using System.Windows.Forms;
using Gallio.Common.Concurrency;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Icarus.Commands;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Runner.Projects.Schema;
using Gallio.Runtime;
using Gallio.UI.Common.Synchronization;
using Gallio.UI.Progress;
using WeifenLuo.WinFormsUI.Docking;
using UnhandledExceptionPolicy = Gallio.Common.Policies.UnhandledExceptionPolicy;
using Gallio.Icarus.Utilities;
using Gallio.UI.ControlPanel;
using Gallio.Runner.Projects;

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

        private WindowManager windowManager;

        // dock panel windows
        private readonly TestExplorer testExplorer;
        private readonly ProjectExplorer projectExplorer;
        private readonly TestResults testResults;
        private readonly RuntimeLogWindow runtimeLogWindow;
        private readonly FiltersWindow filtersWindow;
        private readonly ExecutionLogWindow executionLogWindow;
        private readonly AnnotationsWindow annotationsWindow;

        private readonly string projectFileFilter = string.Format("Gallio Projects (*{0})|*{0}", 
            TestProject.Extension);

        internal Main(IApplicationController applicationController)
        {
            this.applicationController = applicationController;

            applicationController.FileChanged += FileChanged;

            testController = RuntimeAccessor.ServiceLocator.Resolve<ITestController>();
            testController.RunStarted += (sender, e) => Sync.Invoke(this, delegate
            {
                // enable/disable buttons
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
                runTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = false;
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;
            });
            testController.RunFinished += (sender, e) => Sync.Invoke(this, delegate
            {
                // enable/disable buttons & menu items appropriately
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                runTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = true;

                // notify the user if tests have failed!
                if (applicationController.FailedTests)
                    Activate();
            });
            testController.ExploreFinished += (sender, e) => Sync.Invoke(this, delegate
            {
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                runTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = true;
            });

            projectController = RuntimeAccessor.ServiceLocator.Resolve<IProjectController>();

            taskManager = RuntimeAccessor.ServiceLocator.Resolve<ITaskManager>();
            optionsController = RuntimeAccessor.ServiceLocator.Resolve<IOptionsController>();
            reportController = RuntimeAccessor.ServiceLocator.Resolve<IReportController>();
            var testResultsController = RuntimeAccessor.ServiceLocator.Resolve<ITestResultsController>();
            var runtimeLogController = RuntimeAccessor.ServiceLocator.Resolve<IRuntimeLogController>();
            var executionLogController = RuntimeAccessor.ServiceLocator.Resolve<IExecutionLogController>();
            var annotationsController = RuntimeAccessor.ServiceLocator.Resolve<IAnnotationsController>();

            InitializeComponent();

            UnhandledExceptionPolicy.ReportUnhandledException += ReportUnhandledException;

            var sourceCodeController = RuntimeAccessor.ServiceLocator.Resolve<ISourceCodeController>();

            testExplorer = new TestExplorer(optionsController, projectController, testController, 
                sourceCodeController, taskManager);
            projectExplorer = new ProjectExplorer(projectController, testController, reportController, taskManager);
            testResults = new TestResults(testResultsController);
            runtimeLogWindow = new RuntimeLogWindow(runtimeLogController);
            filtersWindow = new FiltersWindow(new FilterController(taskManager, testController, 
                projectController));
            executionLogWindow = new ExecutionLogWindow(executionLogController);
            annotationsWindow = new AnnotationsWindow(annotationsController, sourceCodeController);

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

            progressController = RuntimeAccessor.Instance.ServiceLocator.Resolve<IProgressController>();
            taskManager.ProgressUpdate += (sender, e) => Sync.Invoke(this, ProgressUpdate);
            taskManager.TaskCanceled += (sender, e) => Sync.Invoke(this, TaskCanceled);
            taskManager.TaskCompleted += (sender, e) => Sync.Invoke(this, TaskCompleted);
            progressController.DisplayProgressDialog += (sender, e) => Sync.Invoke(this, () =>
            {
                var dialog = new ProgressMonitor(e.ProgressMonitor);
                dialog.Show(this);
            });
        }

        private void SetupReportMenus()
        {
            // add a menu item for each report type (Report -> View As)
            var reportTypes = new List<string>();
            reportTypes.AddRange(reportController.ReportTypes);
            reportTypes.Sort();
            foreach (string reportType in reportTypes)
            {
                var menuItem = new Controls.ToolStripMenuItem { Text = reportType };
                menuItem.Click += delegate 
                {
                    var command = new ShowReportCommand(testController, reportController, new FileSystem())
                    {
                        ReportFormat = menuItem.Text
                    };
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
            if (persistString == typeof(TestExplorer).ToString())
                return testExplorer;
            if (persistString == typeof(ProjectExplorer).ToString())
                return projectExplorer;
            if (persistString == typeof(TestResults).ToString())
                return testResults;
            if (persistString == typeof(FiltersWindow).ToString())
                return filtersWindow;
            if (persistString == typeof(ExecutionLogWindow).ToString())
                return executionLogWindow;
            if (persistString == typeof(RuntimeLogWindow).ToString())
                return runtimeLogWindow;
            if (persistString == typeof(AnnotationsWindow).ToString())
                return annotationsWindow;
            return windowManager.Get(persistString);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            Text = applicationController.Title;

            // setup window manager
            windowManager = (WindowManager)RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.Icarus.WindowManager");
            windowManager.DockPanel = dockPanel;
            windowManager.StatusStrip = statusStrip.Items;
            windowManager.ToolStrip = toolStripContainer;
            windowManager.Menu = menuStrip.Items;

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

            // provide WindowsFormsSynchronizationContext for cross-thread databinding
            SynchronizationContext.Instance = new SynchronizationContext(System.Threading.SynchronizationContext.Current);

            // restore window size & location
            if (!applicationController.Size.Equals(Size.Empty))
                Size = applicationController.Size;

            if (!applicationController.Location.Equals(Point.Empty))
                Location = applicationController.Location;
        }

        private void DefaultDockState()
        {
            testResults.Show(dockPanel, DockState.Document);
            executionLogWindow.Show(dockPanel, DockState.Document);
            runtimeLogWindow.Show(dockPanel, DockState.DockBottomAutoHide);
            annotationsWindow.Show(dockPanel, DockState.DockBottomAutoHide);
            testExplorer.Show(dockPanel, DockState.DockLeft);
            projectExplorer.Show(dockPanel, DockState.DockLeftAutoHide);
            filtersWindow.DockPanel = dockPanel;
        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            var testFrameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();
            using (var aboutDialog = new AboutDialog(new AboutController(testFrameworkManager)))
                aboutDialog.ShowDialog(this);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            StartTests(false);
        }

        private void StartTests(bool attachDebugger)
        {
            var command = new RunTestsCommand(testController, projectController, optionsController, reportController) 
                { AttachDebugger = attachDebugger };
            taskManager.QueueTask(command);
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            Reload(false);
        }

        private void Reload(bool runTests)
        {
            testExplorer.SaveState();

            var command = new ReloadCommand(testController, projectController);
            taskManager.QueueTask(command);

            if (runTests)
                StartTests(false);
        }

        private void openProject_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Title = "Open Project...";
                openFile.Filter = projectFileFilter;

                if (openFile.ShowDialog() != DialogResult.OK)
                    return;

                applicationController.OpenProject(openFile.FileName);
            }
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Save Project As...",
                OverwritePrompt = true,
                AddExtension = true,
                DefaultExt = projectFileFilter,
                Filter = projectFileFilter
            };
            if (saveFile.ShowDialog() != DialogResult.OK) 
                return;

            applicationController.Title = saveFile.FileName;
            SaveProject();
        }

        private void SaveProject()
        {
            applicationController.SaveProject(true);
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = Dialogs.CreateAddFilesDialog())
            {
                if (openFileDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var command = new AddFilesCommand(projectController, testController) 
                    { Files = openFileDialog.FileNames };
                taskManager.QueueTask(command);
            }
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            //using (var options = new Options.Options(optionsController))
            //    options.ShowDialog(this);

            var presenter = RuntimeAccessor.ServiceLocator.Resolve<IControlPanelPresenter>();
            if (presenter.Show(this) == DialogResult.OK)
                optionsController.Save();
        }

        private void removeAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllFiles();
        }

        private void RemoveAllFiles()
        {
            var cmd = new RemoveAllFilesCommand(testController, projectController);
            taskManager.QueueTask(cmd);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            progressController.Cancel();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProject();
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

        private void helpToolbarButton_Click(object sender, EventArgs e)
        {
            ShowOnlineHelp();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cmd = new ResetTestsCommand(testController);
            taskManager.QueueTask(cmd);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            // we'll close once we've tidied up
            e.Cancel = true;

            try
            {
                // shut down any running operations
                progressController.Cancel();

                // save the current state of the test tree
                testExplorer.SaveState();

                applicationController.SaveProject(false);

                // save window size & location for when we restore
                if (WindowState != FormWindowState.Minimized)
                {
                    applicationController.Size = Size;
                    applicationController.Location = Location;
                }
                optionsController.Save();

                // save dock panel config
                dockPanel.SaveAsXml(Paths.DockConfigFile);

                UnhandledExceptionPolicy.ReportUnhandledException -= ReportUnhandledException;
            }
            catch
            { }

            Application.Exit();
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ShowWindow(item.Name);
        }

        private void ShowWindow(string window)
        {
            Sync.Invoke(this, delegate
            {
                // TODO: is there a better way to do this, rather than relying on the menu item?
                switch (window)
                {
                    case "testResultsToolStripMenuItem":
                        testResults.Show(dockPanel);
                        break;
                    case "projectExplorerToolStripMenuItem":
                        projectExplorer.Show(dockPanel);
                        break;
                    case "testExplorerToolStripMenuItem":
                        testExplorer.Show(dockPanel);
                        break;
                    case "runtimeLogToolStripMenuItem":
                        runtimeLogWindow.Show(dockPanel);
                        break;
                    case "testFiltersToolStripMenuItem":
                        filtersWindow.Show(dockPanel);
                        break;
                    case "executionLogToolStripMenuItem":
                        executionLogWindow.Show(dockPanel);
                        break;
                    case "annotationsToolStripMenuItem":
                        annotationsWindow.Show(dockPanel);
                        break;
                }
            });
        }

        private void FileChanged(object sender, FileChangedEventArgs e)
        {
            // Do this asynchronously when called from another thread.
            BeginInvoke(new MethodInvoker(() => HandleFileChanged(e.FileName)));
        }

        private void HandleFileChanged(string fileName)
        {
            if (!optionsController.AlwaysReloadFiles)
            {
                var reloadDialog = new ReloadDialog(fileName, optionsController);

                if (reloadDialog.ShowDialog(this) != DialogResult.OK)
                    return;
            }
            Reload(optionsController.RunTestsAfterReload);
        }

        private void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e)
        {
            Sync.Invoke(this, () => MessageBox.Show(this, e.GetDescription(), e.Message, 
                MessageBoxButtons.OK, MessageBoxIcon.Error));
        }

        private void ProgressUpdate()
        {
            var progressMonitor = taskManager.ProgressMonitor;

            if (double.IsNaN(progressMonitor.TotalWorkUnits))
            {
                toolStripProgressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                toolStripProgressBar.Style = ProgressBarStyle.Continuous;
                toolStripProgressBar.Maximum = Convert.ToInt32(progressMonitor.TotalWorkUnits);
                toolStripProgressBar.Value = Convert.ToInt32(progressMonitor.CompletedWorkUnits);
            }

            var sb = new StringBuilder();
            sb.Append(progressMonitor.TaskName);
            if (!string.IsNullOrEmpty(progressMonitor.LeafSubTaskName))
            {
                sb.Append(" - ");
                sb.Append(progressMonitor.LeafSubTaskName);
            }
            if (progressMonitor.TotalWorkUnits > 0)
                sb.Append(String.Format(" ({0:P0})", (progressMonitor.CompletedWorkUnits / progressMonitor.TotalWorkUnits)));
            toolStripStatusLabel.Text = sb.ToString();
        }

        private void TaskCanceled()
        {
            toolStripProgressBar.Value = 0;
            toolStripStatusLabel.Text = "Canceled";
        }

        private void TaskCompleted()
        {
            toolStripProgressBar.Value = 0;
            toolStripStatusLabel.Text = string.Empty;
        }

        private void startWithDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTests(true);
        }

        private void runTestsWithDebuggerButton_Click(object sender, EventArgs e)
        {
            StartTests(true);
        }
    }
}
