// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Reflection;
using Gallio.Runtime;
using Gallio.Utilities;
using WeifenLuo.WinFormsUI.Docking;
using Timer = System.Timers.Timer;

namespace Gallio.Icarus
{
    public partial class Main : Form
    {
        private readonly IMediator mediator;

        private readonly Timer timer = new Timer();
        private bool showProgressMonitor = true;

        private string projectFileName = String.Empty;
        
        // dock panel windows
        private readonly DeserializeDockContent deserializeDockContent;
        private readonly TestExplorer testExplorer;
        private readonly ProjectExplorer projectExplorer;
        private readonly TestResults testResults;
        private readonly RuntimeLogWindow runtimeLogWindow;
        private readonly AboutDialog aboutDialog;
        private readonly PropertiesWindow propertiesWindow;
        private readonly FiltersWindow filtersWindow;
        private readonly ExecutionLogWindow executionLogWindow;
        private readonly AnnotationsWindow annotationsWindow;

        private readonly ProgressMonitor progressMonitor;

        private string ProjectFileName
        {
            set
            {
                projectFileName = value;
                Text = value != string.Empty ? String.Format("{0} - Gallio Icarus", value) : "Gallio Icarus";
            }
        }

        public bool ShowProgressMonitor
        {
            get { return showProgressMonitor; }
            set { showProgressMonitor = value; }
        }

        public event EventHandler<EventArgs> CleanUp;

        public Main(IMediator mediator)
        {
            this.mediator = mediator;

            mediator.ProjectController.AssemblyChanged += AssemblyChanged;

            mediator.TestController.RunFinished += testController_RunFinished;
            mediator.TestController.LoadFinished += testController_LoadFinished;
            mediator.TestController.ShowSourceCode += ((sender, e) => ShowSourceCode(e.CodeLocation));
            
            mediator.ProgressMonitorProvider.ProgressUpdate += ProgressUpdate;
            progressMonitor = new ProgressMonitor(mediator);

            InitializeComponent();

            UnhandledExceptionPolicy.ReportUnhandledException += ReportUnhandledException;

            testExplorer = new TestExplorer(mediator);
            projectExplorer = new ProjectExplorer(mediator);
            testResults = new TestResults(mediator);
            runtimeLogWindow = new RuntimeLogWindow(mediator.RuntimeLogController);
            aboutDialog = new AboutDialog(mediator.TestController);
            propertiesWindow = new PropertiesWindow(mediator.ProjectController);
            filtersWindow = new FiltersWindow(mediator);
            executionLogWindow = new ExecutionLogWindow(mediator.ExecutionLogController);
            annotationsWindow = new AnnotationsWindow(mediator.AnnotationsController);

            // used by dock window framework to re-assemble layout
            deserializeDockContent = GetContentFromPersistString;

            // set up delay timer for progress monitor
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Elapsed += delegate { Sync.Invoke(this, () => progressMonitor.Show(this)); };

            SetupReportMenus();
        }

        private void SetupReportMenus()
        {
            // add a menu item for each report type (Report -> View As)
            var reportTypes = new List<string>();
            reportTypes.AddRange(mediator.ReportController.ReportTypes);
            reportTypes.Sort();
            foreach (string reportType in reportTypes)
            {
                var menuItem = new ToolStripMenuItem { Text = reportType };
                menuItem.Click += delegate { mediator.ShowReport(menuItem.Text); };
                viewAsToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        public void ShowSourceCode(CodeLocation codeLocation)
        {
            foreach (DockPane dockPane in dockPanel.Panes)
            {
                foreach (IDockContent dockContent in dockPane.Contents)
                {
                    if (dockContent.ToString() != codeLocation.Path)
                        continue;

                    ((CodeWindow)dockContent).JumpTo(codeLocation.Line, codeLocation.Column);
                    dockContent.DockHandler.Show();
                    return;
                }
            }
            var codeWindow = new CodeWindow(codeLocation);
            codeWindow.Show(dockPanel, DockState.Document);
        }

        private void testController_RunFinished(object sender, EventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;

                mediator.GenerateReport();
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
            return persistString == typeof(AnnotationsWindow).ToString() ? annotationsWindow : null;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // Set the application version in the window title
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Text = String.Format(Text, appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision);

            // try to load the dock state, if the file does not exist
            // or loading fails then use defaults.
            try
            {
                dockPanel.LoadFromXml(Paths.DockConfigFile, deserializeDockContent);
            }
            catch
            {
                DefaultDockState();
            }

            // provide WindowsFormsSynchronizationContext to controllers for cross-thread databinding
            mediator.ProjectController.SynchronizationContext = mediator.TestController.Model.SynchronizationContext = SynchronizationContext.Current;
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
            aboutDialog.ShowDialog();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            StartTests(false);
        }

        private void StartTests(bool attachDebugger)
        {
            // enable/disable buttons
            startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
            stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;

            // no need for progress dialog
            showProgressMonitor = false;

            mediator.RunTests(attachDebugger);
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            Reload();
        }

        public void Reload()
        {
            testExplorer.SaveState();
            mediator.Reload();
        }

        private void openProject_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = "Gallio Projects (*.gallio)|*.gallio";
                if (openFile.ShowDialog() != DialogResult.OK)
                    return;
                ProjectFileName = openFile.FileName;
                mediator.OpenProject(projectFileName);
            }
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectFileName = string.Empty;
            SaveProjectToFile();
        }

        private void SaveProjectToFile()
        {
            if (projectFileName == String.Empty)
            {
                SaveFileDialog saveFile = new SaveFileDialog
                                              {
                                                  OverwritePrompt = true,
                                                  AddExtension = true,
                                                  DefaultExt = "Gallio Projects (*.gallio)|*.gallio",
                                                  Filter = "Gallio Projects (*.gallio)|*.gallio"
                                              };
                if (saveFile.ShowDialog() == DialogResult.OK)
                    ProjectFileName = saveFile.FileName;
            }
            mediator.SaveProject(projectFileName);
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAssembliesToTree();
        }

        public void AddAssembliesToTree()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
                                                {
                                                    Filter =
                                                        "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*",
                                                    Multiselect = true
                                                };
            using (openFileDialog)
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                mediator.AddAssemblies(openFileDialog.FileNames);
            }
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (Options.Options options = new Options.Options(mediator.OptionsController))
            {
                options.ShowDialog();
            }
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAllAssemblies();
        }

        public void RemoveAllAssemblies()
        {
            mediator.RemoveAllAssemblies();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            mediator.Cancel();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectToFile();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void CreateNewProject()
        {
            ProjectFileName = string.Empty;
            mediator.NewProject();
        }

        private void newProjectToolStripButton_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediator.Cancel();
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
            ResetTests();
        }

        public void ResetTests()
        {
            mediator.ResetTests();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            // we'll close once we've tidied up
            e.Cancel = true;
            // shut down any running operations
            mediator.Cancel();
            // save the current state of the test tree
            testExplorer.SaveState();
            mediator.TestController.UnloadFinished += CleanUpOnClose;
            // unload the current test package
            mediator.Unload();
        }

        private void CleanUpOnClose(object sender, EventArgs e)
        {
            mediator.SaveProject(string.Empty);

            // save dock panel config
            dockPanel.SaveAsXml(Paths.DockConfigFile);

            // dispose of the test runner service
            EventHandlerUtils.SafeInvoke(CleanUp, this, EventArgs.Empty);
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
            bool reload = false;
            if (mediator.OptionsController.AlwaysReloadAssemblies)
                reload = true;
            else
            {
                using (ReloadDialog reloadDialog = new ReloadDialog(e.AssemblyName))
                {
                    if (reloadDialog.ShowDialog() == DialogResult.OK)
                    {
                        reload = true;
                        mediator.OptionsController.AlwaysReloadAssemblies = reloadDialog.AlwaysReloadTests;
                    }
                }
            }
            if (reload)
                Reload();
        }

        void testController_LoadFinished(object sender, EventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                startButton.Enabled = true;
                startTestsToolStripMenuItem.Enabled = true;
            });
        }

        void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e)
        {
            if (e.Exception is ThreadAbortException || e.IsRecursive)
                return;

            // We already print the errors to the log and most of them are harmless.
            // Ideally we should display errors more unobtrusively.  Say by flashing
            // a little icon in the status area to indicate that some new error has been
            // logged.  I really don't like the fact that Icarus is using Thread Aborts all
            // over the place.  That's the cause of most of these errors anyways.
            // Better if we introduced a real abstraction for background task management
            // and displayed progress monitor dialogs for long-running operations. -- Jeff.
            Sync.Invoke(this,
                () => MessageBox.Show(this, e.GetDescription(), e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error));
        }

        private void ProgressUpdate(object sender, ProgressUpdateEventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                if (e.TotalWorkUnits > 0 && !progressMonitor.Visible && showProgressMonitor && mediator.OptionsController.ShowProgressDialogs)
                {
                    timer.Enabled = true;
                    progressMonitor.Cursor = Cursors.WaitCursor;
                }
                else if (e.TotalWorkUnits == 0)
                {
                    timer.Enabled = false;
                    progressMonitor.Hide();
                    progressMonitor.Cursor = Cursors.Default;
                    showProgressMonitor = true;
                }

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
    }
}
