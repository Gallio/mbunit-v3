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
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.ProgressMonitoring.EventArgs;
using Gallio.Reflection;
using Gallio.Runner.Projects;
using Gallio.Runtime;
using Gallio.Utilities;
using WeifenLuo.WinFormsUI.Docking;
using Timer = System.Timers.Timer;

namespace Gallio.Icarus
{
    public partial class Main : Form
    {
        private readonly IProjectController projectController;
        private readonly ITestController testController;
        private readonly IOptionsController optionsController;
        private readonly IReportController reportController;
        
        private readonly ProgressMonitor progressMonitor;
        private readonly Timer timer = new Timer();
        private bool showProgressMonitor = true;

        private string projectFileName = String.Empty;
        private readonly Arguments arguments;
        
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

        public Main(IProjectController projectController, ITestController testController, IRuntimeLogController runtimeLogController, 
            IExecutionLogController executionLogController, IReportController reportController, Arguments arguments)
        {
            this.projectController = projectController;
            projectController.AssemblyChanged += AssemblyChanged;
            this.testController = testController;
            testController.RunFinished += testController_RunFinished;
            testController.LoadFinished += testController_LoadFinished;
            testController.ProgressUpdate += ProgressUpdate;
            testController.ShowSourceCode += delegate(object sender, ShowSourceCodeEventArgs e) { ShowSourceCode(e.CodeLocation); };
            this.reportController = reportController;
            reportController.ProgressUpdate += ProgressUpdate;
            this.arguments = arguments;

            optionsController = OptionsController.Instance;

            progressMonitor = new ProgressMonitor(testController, optionsController);

            InitializeComponent();

            UnhandledExceptionPolicy.ReportUnhandledException += ReportUnhandledException;

            testExplorer = new TestExplorer(projectController, testController, optionsController);
            projectExplorer = new ProjectExplorer(projectController);
            testResults = new TestResults(testController, optionsController);
            runtimeLogWindow = new RuntimeLogWindow(runtimeLogController);
            aboutDialog = new AboutDialog(testController);
            propertiesWindow = new PropertiesWindow(projectController);
            filtersWindow = new FiltersWindow(projectController, testController);
            executionLogWindow = new ExecutionLogWindow(executionLogController);
            annotationsWindow = new AnnotationsWindow(testController);

            // used by dock window framework to re-assemble layout
            deserializeDockContent = GetContentFromPersistString;

            // set up delay timer for progress monitor
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Elapsed += delegate { Sync.Invoke(this, delegate { progressMonitor.Show(this); }); };

            // add a menu item for each report type (Report -> View As)
            List<string> reportTypes = new List<string>();
            reportTypes.AddRange(reportController.ReportTypes);
            reportTypes.Sort();
            foreach (string reportType in reportTypes)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = reportType;
                menuItem.Click += delegate
                {
                    testController.Report.Read(report => reportController.ShowReport(report, menuItem.Text));
                };
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
            CodeWindow codeWindow = new CodeWindow(codeLocation);
            codeWindow.Show(dockPanel, DockState.Document);
        }

        void testController_RunFinished(object sender, EventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;

                string reportFolder = Path.Combine(Path.GetDirectoryName(projectController.ProjectFileName), "Reports");

                testController.Report.Read(report => reportController.GenerateReport(report, reportFolder));
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

            List<string> assemblyFiles = new List<string>();
            if (arguments != null && arguments.Assemblies.Length > 0)
            {
                foreach (string assembly in assemblyFiles)
                {
                    if (!File.Exists(assembly))
                        continue;
                    if (Path.GetExtension(assembly) == "'gallio")
                    {
                        OpenProject(assembly);
                        continue;
                    }
                    projectController.AddAssemblies(assemblyFiles);
                }
            }
            else if (OptionsController.Instance.RestorePreviousSettings && File.Exists(Paths.DefaultProject))
                OpenProject(Paths.DefaultProject);
        }

        void OpenProject(string fileName)
        {
            projectController.OpenProject(fileName);
            testController.Reload(projectController.TestPackageConfig);
            testController.LoadFinished += delegate
            {
                foreach (FilterInfo filterInfo in projectController.TestFilters)
                {
                    if (filterInfo.FilterName != "AutoSave")
                        continue;
                    testController.ApplyFilter(filterInfo.Filter);
                    return;
                }
            };
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
            StartTests();
        }

        private void StartTests()
        {
            try
            {
                // enable/disable buttons
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;

                projectController.SaveFilter("LastRun", testController.GetCurrentFilter());
                testController.RunTests();
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while starting the tests.", ex);
            }
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            testController.Reload(projectController.TestPackageConfig);
        }

        private void openProject_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Filter = "Gallio Projects (*.gallio)|*.gallio";
                if (openFile.ShowDialog() != DialogResult.OK)
                    return;
                ProjectFileName = openFile.FileName;
                OpenProject(projectFileName);
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
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.OverwritePrompt = true;
                saveFile.AddExtension = true;
                saveFile.DefaultExt = "Gallio Projects (*.gallio)|*.gallio";
                saveFile.Filter = "Gallio Projects (*.gallio)|*.gallio";
                if (saveFile.ShowDialog() == DialogResult.OK)
                    ProjectFileName = saveFile.FileName;
            }
            projectController.SaveProject(projectFileName);
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAssembliesToTree();
        }

        public void AddAssembliesToTree()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;
            using (openFileDialog)
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                projectController.AddAssemblies(openFileDialog.FileNames);
                testController.Reload(projectController.TestPackageConfig);
            }
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (Options.Options options = new Options.Options(OptionsController.Instance))
            {
                options.ShowDialog();
            }
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAssembliesFromTree();
        }

        public void RemoveAssembliesFromTree()
        {
            projectController.RemoveAllAssemblies();
            testController.Reload(projectController.TestPackageConfig);
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        public void Cancel()
        {
            testController.Cancel();
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
            projectController.NewProject();
            testController.Reload(projectController.TestPackageConfig);
        }

        private void newProjectToolStripButton_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void startTestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTests();
        }

        private void showOnlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowOnlineHelp();
        }

        private static void ShowOnlineHelp()
        {
            System.Diagnostics.Process.Start("http://docs.mbunit.com");
        }

        private void helpToolbarButton_Click(object sender, EventArgs e)
        {
            ShowOnlineHelp();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testController.ResetTests();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.ApplicationExitCall)
                return;

            e.Cancel = true;
            testController.UnloadFinished += CleanUpOnClose;
            testController.UnloadTestPackage();
        }

        private void CleanUpOnClose(object sender, EventArgs e)
        {
            // FIXME: Improve error handling
            try
            {
                // save test filter
                projectController.SaveFilter("AutoSave", testController.GetCurrentFilter());
            }
            catch
            { }

            try
            {
                // save project
                projectController.SaveProject(string.Empty);
            }
            catch
            { }

            try
            {
                // save dock panel config
                dockPanel.SaveAsXml(Paths.DockConfigFile);
            }
            catch
            { }

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
        }

        private void AssemblyChanged(object sender, AssemblyChangedEventArgs e)
        {
            bool reload = false;
            if (optionsController.AlwaysReloadAssemblies)
                reload = true;
            else
            {
                using (ReloadDialog reloadDialog = new ReloadDialog(e.AssemblyName))
                {
                    if (reloadDialog.ShowDialog() == DialogResult.OK)
                    {
                        reload = true;
                        OptionsController.Instance.AlwaysReloadAssemblies = reloadDialog.AlwaysReloadTests;
                    }
                }
            }
            if (reload)
                testController.Reload(projectController.TestPackageConfig);
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
            Sync.Invoke(this, delegate
            {
                MessageBox.Show(this, e.GetDescription(), e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private void ProgressUpdate(object sender, ProgressUpdateEventArgs e)
        {
            Sync.Invoke(this, delegate
            {
                if (e.TaskName == "Running the tests.")
                    showProgressMonitor = false;

                if (e.TotalWorkUnits > 0 && !progressMonitor.Visible && showProgressMonitor && optionsController.ShowProgressDialogs)
                {
                    timer.Enabled = true;
                    progressMonitor.Cursor = Cursors.WaitCursor;
                }
                else
                {
                    timer.Enabled = false;
                    progressMonitor.Hide();
                    progressMonitor.Cursor = Cursors.Default;
                    showProgressMonitor = true;
                }

                toolStripProgressBar.Maximum = Convert.ToInt32(e.TotalWorkUnits);
                toolStripProgressBar.Value = Convert.ToInt32(e.CompletedWorkUnits);

                StringBuilder sb = new StringBuilder();
                sb.Append(e.TaskName);
                if (e.SubTaskName != String.Empty)
                {
                    sb.Append(" - ");
                    sb.Append(e.SubTaskName);
                }
                if (e.TotalWorkUnits > 0)
                    sb.Append(String.Format(" ({0:P})", (e.CompletedWorkUnits/e.TotalWorkUnits)));
                toolStripStatusLabel.Text = sb.ToString();
            });
        }
    }
}
