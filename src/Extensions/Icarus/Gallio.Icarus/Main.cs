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
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Model;
using Gallio.Runner.Projects;
using Gallio.Runtime;
using WeifenLuo.WinFormsUI.Docking;
using SynchronizationContext = System.Threading.SynchronizationContext;
using UnhandledExceptionPolicy = Gallio.Common.Policies.UnhandledExceptionPolicy;
using Gallio.Icarus.Runtime;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Commands;

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
        private readonly PropertiesWindow propertiesWindow;
        private readonly FiltersWindow filtersWindow;
        private readonly ExecutionLogWindow executionLogWindow;
        private readonly AnnotationsWindow annotationsWindow;

        private readonly string projectFileFilter = string.Format("Gallio Projects (*{0})|*{0}", 
            Project.Extension);

        internal Main(IApplicationController applicationController)
        {
            this.applicationController = applicationController;

            applicationController.AssemblyChanged += AssemblyChanged;

            testController = RuntimeAccessor.ServiceLocator.Resolve<ITestController>();
            testController.RunFinished += TestControllerRunFinished;
            testController.ExploreFinished += TestControllerLoadFinished;

            projectController = RuntimeAccessor.ServiceLocator.Resolve<IProjectController>();

            var sourceCodeController = RuntimeAccessor.ServiceLocator.Resolve<ISourceCodeController>();
            sourceCodeController.ShowSourceCode += ((sender, e) => ShowSourceCode(e.CodeLocation));

            taskManager = RuntimeAccessor.ServiceLocator.Resolve<ITaskManager>();
            optionsController = RuntimeAccessor.ServiceLocator.Resolve<IOptionsController>();
            reportController = RuntimeAccessor.ServiceLocator.Resolve<IReportController>();
            var testResultsController = RuntimeAccessor.ServiceLocator.Resolve<ITestResultsController>();
            var runtimeLogController = RuntimeAccessor.ServiceLocator.Resolve<IRuntimeLogController>();
            var executionLogController = RuntimeAccessor.ServiceLocator.Resolve<IExecutionLogController>();
            var annotationsController = RuntimeAccessor.ServiceLocator.Resolve<IAnnotationsController>();

            InitializeComponent();

            UnhandledExceptionPolicy.ReportUnhandledException += ReportUnhandledException;

            testExplorer = new TestExplorer(optionsController, projectController, testController, 
                sourceCodeController, taskManager);
            projectExplorer = new ProjectExplorer(projectController, testController, reportController, taskManager);
            testResults = new TestResults(testResultsController);
            runtimeLogWindow = new RuntimeLogWindow(runtimeLogController);
            propertiesWindow = new PropertiesWindow(projectController);
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
            progressController.ProgressUpdate += ProgressUpdate;
            progressController.DisplayProgressDialog += (sender, e) => Sync.Invoke(this, () =>
            {
                using (var dialog = new ProgressMonitor(e.ProgressMonitor, e.ProgressUpdateEventArgs))
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
                var menuItem = new ToolStripMenuItem { Text = reportType };
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

        private void ShowSourceCode(CodeLocation codeLocation)
        {
            foreach (var dockPane in dockPanel.Panes)
            {
                foreach (var dockContent in dockPane.Contents)
                {
                    if (!(dockContent is CodeWindow) || dockContent.ToString() != codeLocation.Path)
                        continue;

                    ((CodeWindow)dockContent).JumpTo(codeLocation.Line, codeLocation.Column);
                    dockContent.DockHandler.Show();
                    return;
                }
            }
            var codeWindow = new CodeWindow(codeLocation);
            codeWindow.Show(dockPanel, DockState.Document);
        }

        private void TestControllerRunFinished(object sender, EventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                // enable/disable buttons & menu items appropriately
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                runTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = true;

                // notify the user if tests have failed!
                if (applicationController.FailedTests)
                    Activate();
            });
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(TestExplorer).ToString())
                return testExplorer;
            if (persistString == typeof(ProjectExplorer).ToString())
                return projectExplorer;
            if (persistString == typeof(TestResults).ToString())
                return testResults;
            if (persistString == typeof(PropertiesWindow).ToString())
                return propertiesWindow;
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

            // prepare window manager
            windowManager = new WindowManager(dockPanel, statusStrip.Items, toolStripContainer, menuStrip.Items);
            var runtime = (IcarusRuntime)RuntimeAccessor.Instance;
            runtime.RegisterComponent("Gallio.Icarus.WindowManager", typeof(IWindowManager),
                windowManager);

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
            Utilities.SynchronizationContext.Instance = new Utilities.SynchronizationContext(SynchronizationContext.Current);

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
            propertiesWindow.DockPanel = dockPanel;
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
            // enable/disable buttons
            startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
            runTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = false;
            stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;

            var command = new RunTestsCommand(testController, projectController, optionsController, reportController);
            command.AttachDebugger = attachDebugger;
            taskManager.QueueTask(command);
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void Reload()
        {
            testExplorer.SaveState();

            var command = new ReloadCommand(testController, projectController);
            taskManager.QueueTask(command);
        }

        private void openProject_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
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
            applicationController.SaveProject();
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var addAssembliesCommand = new AddAssembliesCommand(projectController, testController);
            taskManager.QueueTask(addAssembliesCommand);
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (var options = new Options.Options(optionsController))
                options.ShowDialog(this);
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllAssemblies();
        }

        private void RemoveAllAssemblies()
        {
            var cmd = new RemoveAllAssembliesCommand(testController, projectController);
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
            
            // shut down any running operations
            progressController.Cancel();

            // save the current state of the test tree
            testExplorer.SaveState();

            applicationController.SaveProject();

            // save window size & location for when we restore
            if (WindowState != FormWindowState.Minimized)
            {
                applicationController.Size = Size;
                applicationController.Location = Location;
            }
            optionsController.Save();

            // save dock panel config
            dockPanel.SaveAsXml(Paths.DockConfigFile);

            // dispose of the test runner service
            UnhandledExceptionPolicy.ReportUnhandledException -= ReportUnhandledException;

            Application.Exit();
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            ShowWindow(item.Name);
        }

        public void ShowWindow(string window)
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
                                          case "propertiesToolStripMenuItem":
                                              propertiesWindow.Show(dockPanel);
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

        private void AssemblyChanged(object sender, AssemblyChangedEventArgs e)
        {
            // Do this asynchronously when called from another thread.
            BeginInvoke(new MethodInvoker(() => HandleAssemblyChanged(e.AssemblyName)));
        }

        private void HandleAssemblyChanged(string assemblyName)
        {
            if (!optionsController.AlwaysReloadAssemblies)
            {
                using (var reloadDialog = new ReloadDialog(assemblyName))
                {
                    if (reloadDialog.ShowDialog(this) != DialogResult.OK)
                        return;

                    optionsController.AlwaysReloadAssemblies = reloadDialog.AlwaysReloadTests;
                }
            }

            Reload();

            if (optionsController.RunTestsAfterReload)
                StartTests(false);
        }

        private void TestControllerLoadFinished(object sender, EventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                runTestsWithDebuggerButton.Enabled = startWithDebuggerToolStripMenuItem.Enabled = true;
            });
        }

        private void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e)
        {
            Sync.Invoke(this, () => MessageBox.Show(this, e.GetDescription(), e.Message, 
                MessageBoxButtons.OK, MessageBoxIcon.Error));
        }

        private void ProgressUpdate(object sender, ProgressUpdateEventArgs e)
        {
            Sync.Invoke(this, () => 
            {
                toolStripProgressBar.Maximum = Convert.ToInt32(e.TotalWorkUnits);
                toolStripProgressBar.Value = Convert.ToInt32(e.CompletedWorkUnits);

                var sb = new StringBuilder();
                sb.Append(e.TaskName);
                if (!string.IsNullOrEmpty(e.SubTaskName))
                {
                    sb.Append(" - ");
                    sb.Append(e.SubTaskName);
                }
                if (e.TotalWorkUnits > 0)
                    sb.Append(String.Format(" ({0:P0})", (e.CompletedWorkUnits/e.TotalWorkUnits)));
                toolStripStatusLabel.Text = sb.ToString();
            });
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
