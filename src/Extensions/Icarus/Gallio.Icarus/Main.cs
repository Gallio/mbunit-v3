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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Interfaces;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class Main : Form, IProjectAdapterView
    {
        private readonly TaskManager primaryTaskManager = new TaskManager();
        private readonly TaskManager executionLogTaskManager = new TaskManager();

        private string projectFileName = String.Empty;
        private readonly Arguments arguments;
        
        // dock panel windows
        private readonly DeserializeDockContent deserializeDockContent;
        private readonly TestExplorer testExplorer;
        private readonly ProjectExplorer projectExplorer;
        private readonly TestResults testResults;
        private readonly ReportWindow reportWindow;
        private readonly RuntimeLogWindow runtimeLogWindow;
        private readonly About aboutDialog;
        private readonly PropertiesWindow propertiesWindow;
        private readonly FiltersWindow filtersWindow;
        private readonly ExecutionLogWindow executionLogWindow;
        private readonly AnnotationsWindow annotationsWindow;
        
        // progress monitoring
        private readonly ProgressMonitor progressMonitor;
        private readonly System.Timers.Timer progressMonitorTimer;
        private bool showProgressMonitor = true;
        private string taskName, subTaskName;
        private double totalWorkUnits, completedWorkUnits;

        public bool ShowProgressMonitor
        {
            set { showProgressMonitor = value; }
        }

        public ITreeModel TestTreeModel
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    testExplorer.TreeModel = value;
                    testResults.TreeModel = value;
                    ((TestTreeModel)value).TestCountChanged += delegate { TotalTests = ((TestTreeModel)value).TestCount; };
                });
            }
        }

        public ITreeModel ProjectTreeModel
        {
            set { Sync.Invoke(this, delegate { projectExplorer.TreeModel = value; }); }
        }

        public string TaskName
        {
            set
            {
                taskName = value;
                // TODO: Tidy this up
                if (value == "Running the tests.")
                    showProgressMonitor = false;
                Sync.Invoke(this, UpdateProgress);
            }
        }

        public string SubTaskName
        {
            set
            {
                subTaskName = value;
                Sync.Invoke(this, UpdateProgress);
            }
        }

        public double TotalWorkUnits
        {
            set
            {
                totalWorkUnits = value;
                Sync.Invoke(this, delegate
                {
                    UpdateProgress();
                    if (value != 0)
                        return;
                    // task is complete, hide progress monitor
                    progressMonitorTimer.Enabled = false;
                    progressMonitor.Hide();
                    Cursor = Cursors.Default;
                    showProgressMonitor = true;
                });
            }
        }

        public double CompletedWorkUnits
        {
            set
            {
                completedWorkUnits = value;
                Sync.Invoke(this, delegate
                {
                    UpdateProgress();
                    if (value > 0 && !progressMonitor.Visible && showProgressMonitor && OptionsController.Instance.ShowProgressDialogs)
                        progressMonitorTimer.Enabled = true;
                    Cursor = Cursors.WaitCursor;
                });
            }
        }

        public int TotalTests
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    testResults.TotalTests = value;
                });
            }
        }

        public string ReportPath
        {
            set
            {
                if (value != "")
                {
                    Sync.Invoke(this, delegate
                    {
                        reportWindow.ReportPath = value;
                    });
                }
            }
        }

        public IList<string> ReportTypes
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    reportWindow.ReportTypes = value;
                });
            }
        }

        public IList<string> TestFrameworks
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    aboutDialog.TestFrameworks = value;
                });
            }
        }

        public Stream ExecutionLog
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    executionLogWindow.Log = value;
                });
            }
        }

        public CodeLocation SourceCodeLocation
        {
            set
            {
                foreach (DockPane dockPane in dockPanel.Panes)
                {
                    foreach (IDockContent dockContent in dockPane.Contents)
                    {
                        if (dockContent.ToString() == value.Path)
                        {
                            ((CodeWindow)dockContent).JumpTo(value.Line, value.Column);
                            dockContent.DockHandler.Show();
                            return;
                        }
                    }
                }
                CodeWindow codeWindow = new CodeWindow(value);
                codeWindow.Show(dockPanel, DockState.Document);
            }
        }

        public IList<string> HintDirectories
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    propertiesWindow.HintDirectories = value;
                });
            }
        }

        public string ApplicationBaseDirectory
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    propertiesWindow.ApplicationBaseDirectory = value;
                });
            }
        }

        public string WorkingDirectory
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    propertiesWindow.WorkingDirectory = value;
                });
            }
        }

        public bool ShadowCopy
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    propertiesWindow.ShadowCopy = value;
                });
            }
        }

        private string ProjectFileName
        {
            set
            {
                projectFileName = value;
                Text = value != string.Empty ? String.Format("{0} - Gallio Icarus", value) : "Gallio Icarus";
            }
        }

        public IList<string> TestFilters
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    filtersWindow.Filters = value;
                });
            }
        }

        public List<AnnotationData> Annotations
        {
            set
            {
                Sync.Invoke(this, delegate
                {
                    annotationsWindow.Annotations = value;
                    if (value.Count > 0)
                        annotationsWindow.Show();
                });
            }
        }

        public bool EditEnabled
        {
            set { testExplorer.EditEnabled = value; }
        }

        public event EventHandler<GetTestTreeEventArgs> GetTestTree;
        public event EventHandler<SingleEventArgs<IList<string>>> AddAssemblies;
        public event EventHandler<EventArgs> RemoveAssemblies;
        public event EventHandler<SingleEventArgs<string>> RemoveAssembly;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> GenerateReport;
        public event EventHandler<EventArgs> CancelOperation;
        public event EventHandler<SingleEventArgs<string>> SaveFilter;
        public event EventHandler<SingleEventArgs<string>> ApplyFilter;
        public event EventHandler<SingleEventArgs<string>> DeleteFilter;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<EventArgs> GetTestFrameworks;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        public event EventHandler<SingleEventArgs<string>> SaveProject;
        public event EventHandler<OpenProjectEventArgs> OpenProject;
        public event EventHandler<EventArgs> NewProject;
        public event EventHandler<SingleEventArgs<string>> GetSourceLocation;
        public event EventHandler<SingleEventArgs<IList<string>>> UpdateHintDirectoriesEvent;
        public event EventHandler<SingleEventArgs<string>> UpdateApplicationBaseDirectoryEvent;
        public event EventHandler<SingleEventArgs<string>> UpdateWorkingDirectoryEvent;
        public event EventHandler<SingleEventArgs<bool>> UpdateShadowCopyEvent;
        public event EventHandler<EventArgs> ResetTestStatus;
        public event EventHandler<SingleEventArgs<IList<string>>> GetExecutionLog;
        public event EventHandler<EventArgs> UnloadTestPackage;
        public event EventHandler<EventArgs> CleanUp;

        public Main(Arguments arguments)
        {
            this.arguments = arguments;

            InitializeComponent();

            UnhandledExceptionPolicy.ReportUnhandledException += ReportUnhandledException;

            testExplorer = new TestExplorer(this);
            projectExplorer = new ProjectExplorer(this);
            testResults = new TestResults(OptionsController.Instance);
            reportWindow = new ReportWindow(this);
            runtimeLogWindow = new RuntimeLogWindow();
            aboutDialog = new About();
            propertiesWindow = new PropertiesWindow(this);
            filtersWindow = new FiltersWindow(this);
            executionLogWindow = new ExecutionLogWindow();
            annotationsWindow = new AnnotationsWindow(this);
            progressMonitor = new ProgressMonitor();

            deserializeDockContent = GetContentFromPersistString;

            progressMonitorTimer = new System.Timers.Timer {Interval = 1000, AutoReset = false};
            progressMonitorTimer.Elapsed += delegate
            {
                Sync.Invoke(this, delegate
                {
                    if (progressMonitor != null)
                        progressMonitor.Show(this);
                });
            };
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(TestExplorer).ToString())
                return testExplorer;
            if (persistString == typeof(ProjectExplorer).ToString())
                return projectExplorer;
            if (persistString == typeof(TestResults).ToString())
                return testResults;
            if (persistString == typeof(ReportWindow).ToString())
                return reportWindow;
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
            Text = String.Format(Text, appVersion.Major, appVersion.Minor);

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
                assemblyFiles.AddRange(arguments.Assemblies);
            else if (OptionsController.Instance.RestorePreviousSettings && File.Exists(Paths.DefaultProject))
                assemblyFiles.Add(Paths.DefaultProject);

            primaryTaskManager.StartTask(delegate
            {
                if (GetReportTypes != null)
                    GetReportTypes(this, EventArgs.Empty);
                if (GetTestFrameworks != null)
                    GetTestFrameworks(this, EventArgs.Empty);
                if (assemblyFiles.Count > 0 && AddAssemblies != null)
                    AddAssemblies(this, new SingleEventArgs<IList<string>>(assemblyFiles));
                ThreadedReloadTree(true);
            });
        }

        private void DefaultDockState()
        {
            testResults.Show(dockPanel, DockState.Document);
            executionLogWindow.Show(dockPanel, DockState.Document);
            runtimeLogWindow.DockPanel = dockPanel;
            annotationsWindow.Show(dockPanel, DockState.Document);
            testExplorer.Show(dockPanel, DockState.DockLeft);
            projectExplorer.Show(dockPanel, DockState.DockLeftAutoHide);
            reportWindow.DockPanel = dockPanel;
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
                // reset progress monitors
                ResetTests();

                // enable/disable buttons
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;

                // disable checkboxes (filter) on test tree
                testExplorer.EditEnabled = false;

                primaryTaskManager.StartTask(delegate
                {
                    // save test filter
                    OnSaveFilter("LastRun");
                    
                    // run tests
                    if (RunTests != null)
                        RunTests(this, new EventArgs());

                    // enable/disable buttons
                    Invoke((MethodInvoker)delegate
                    {
                        stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                        startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                        testExplorer.EditEnabled = true;
                    });
                });
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while starting the tests.", ex);
            }
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            primaryTaskManager.StartTask(() => ThreadedReloadTree(true));
        }

        private void openProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog {Filter = "Gallio Projects (*.gallio)|*.gallio"};
            if (openFile.ShowDialog() != DialogResult.OK)
                return;
            ProjectFileName = openFile.FileName;
            primaryTaskManager.StartTask(OpenProjectFromFile);
        }

        private void OpenProjectFromFile()
        {
            try
            {
                if (OpenProject != null)
                    OpenProject(this, new OpenProjectEventArgs(projectFileName, testExplorer.TreeFilter));
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while opening a project.", ex);
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

            primaryTaskManager.StartTask(delegate
            {
                if (SaveProject != null)
                    SaveProject(this, new SingleEventArgs<string>(projectFileName));
            });
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAssembliesToTree();
        }

        public void AddAssembliesToTree()
        {
            OpenFileDialog openFile = new OpenFileDialog
                                          {
                                              Filter =
                                                  "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*",
                                              Multiselect = true
                                          };
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                primaryTaskManager.StartTask(delegate
                {
                    if (AddAssemblies != null)
                        AddAssemblies(this, new SingleEventArgs<IList<string>>(openFile.FileNames));
                    ThreadedReloadTree(true);
                });
            }
        }

        public void ReloadTree()
        {
            primaryTaskManager.StartTask(() => ThreadedReloadTree(false));
        }

        private void ThreadedReloadTree(bool reloadTestModelData)
        {
            if (GetTestTree != null)
                GetTestTree(this, new GetTestTreeEventArgs(testExplorer.TreeFilter, reloadTestModelData));
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            using (Options.Options options = new Options.Options(OptionsController.Instance))
            {
                options.ShowDialog();
            }
        }

        public void ResetTests()
        {
            testResults.Reset();
            if (ResetTestStatus != null)
                ResetTestStatus(this, EventArgs.Empty);
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAssembliesFromTree();
        }

        public void RemoveAssembliesFromTree()
        {
            primaryTaskManager.StartTask(delegate
            {
                // remove assemblies
                if (RemoveAssemblies != null)
                    RemoveAssemblies(this, new EventArgs());
                ThreadedReloadTree(true);
            });
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            CancelTests();
        }

        private void CancelTests()
        {
            CancelRunningOperation();

            // reset buttons
            stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
            startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
        }

        public void CancelRunningOperation()
        {
            if (CancelOperation != null)
                CancelOperation(this, new EventArgs());
        }

        public void ThreadedRemoveAssembly(string assembly)
        {
            primaryTaskManager.StartTask(delegate
            {
                if (RemoveAssembly != null)
                    RemoveAssembly(this, new SingleEventArgs<string>(assembly));
                ThreadedReloadTree(true);
            });
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
            primaryTaskManager.StartTask(delegate
            {
                if (NewProject != null)
                    NewProject(this, EventArgs.Empty);
                ThreadedReloadTree(true);
            });
        }

        private void newProjectToolStripButton_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CancelTests();
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

        public void SaveReport(string fileName, string reportType)
        {
            if (SaveReportAs != null)
                SaveReportAs(this, new SaveReportAsEventArgs(fileName, reportType));
        }

        public void WriteToLog(LogSeverity severity, string message, Exception exception)
        {
            Sync.Invoke(this, delegate
            {
                Color color = Color.Black;
                switch (severity)
                {
                    case LogSeverity.Error:
                        color = Color.Red;
                        break;

                    case LogSeverity.Warning:
                        color = Color.Gold;
                        break;

                    case LogSeverity.Info:
                        color = Color.Gray;
                        break;

                    case LogSeverity.Debug:
                        color = Color.DarkGray;
                        break;
                }

                runtimeLogWindow.AppendTextLine(message, color);
                if (exception != null)
                    runtimeLogWindow.AppendTextLine(ExceptionUtils.SafeToString(exception), color);
            });
        }

        public void CreateReport()
        {
            primaryTaskManager.StartTask(ThreadedCreateReport);
        }

        private void ThreadedCreateReport()
        {
            if (GenerateReport != null)
                GenerateReport(this, EventArgs.Empty);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetTests();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                e.Cancel = true;

                primaryTaskManager.StartTask(delegate
                {
                    try
                    {
                        if (UnloadTestPackage != null)
                            UnloadTestPackage(this, EventArgs.Empty);

                        CleanUpOnClose();
                    }
                    finally
                    {
                        Application.Exit();
                    }
                });
            }
        }

        private void CleanUpOnClose()
        {
            // FIXME: Improve error handling
            try
            {
                // save test filter
                OnSaveFilter("AutoSave");
            }
            catch
            { }

            try
            {
                // save project
                if (SaveProject != null)
                    SaveProject(this, new SingleEventArgs<string>(string.Empty));

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

            primaryTaskManager.AbortTask();
            executionLogTaskManager.AbortTask();

            UnhandledExceptionPolicy.ReportUnhandledException -= ReportUnhandledException;
        }

        public void ViewSourceCode(string testId)
        {
            if (GetSourceLocation != null)
                GetSourceLocation(this, new SingleEventArgs<string>(testId));
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            switch (item.Name)
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
                case "reportToolStripMenuItem":
                    CreateReport();
                    reportWindow.Show(dockPanel);
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

        public void AssemblyChanged(string filePath)
        {
            List<TaskButton> taskButtons = new List<TaskButton>();
            TaskButton yes = new TaskButton
                                 {
                                     Text = "Yes",
                                     Icon = Properties.Resources.tick,
                                     Description = "Reload the test model."
                                 };
            taskButtons.Add(yes);
            TaskButton no = new TaskButton
                                {
                                    Text = "No",
                                    Icon = Properties.Resources.cross,
                                    Description = "Don't reload."
                                };
            taskButtons.Add(no);

            if (OptionsController.Instance.AlwaysReloadAssemblies || TaskDialog.Show("Assembly changed", filePath + 
                " has changed, would you like to reload the test model?", taskButtons) == yes)
            {
                primaryTaskManager.StartTask(() => ThreadedReloadTree(true));
            }
        }

        public void UpdateHintDirectories(IList<string> hintDirectories)
        {
            if (UpdateHintDirectoriesEvent != null)
                UpdateHintDirectoriesEvent(this, new SingleEventArgs<IList<string>>(hintDirectories));
        }

        public void UpdateApplicationBaseDirectory(string applicationBaseDirectory)
        {
            if (UpdateApplicationBaseDirectoryEvent != null)
                UpdateApplicationBaseDirectoryEvent(this, new SingleEventArgs<string>(applicationBaseDirectory));
        }

        public void UpdateWorkingDirectory(string workingDirectory)
        {
            if (UpdateWorkingDirectoryEvent != null)
                UpdateWorkingDirectoryEvent(this, new SingleEventArgs<string>(workingDirectory));
        }

        public void UpdateShadowCopy(bool shadowCopy)
        {
            if (UpdateShadowCopyEvent != null)
                UpdateShadowCopyEvent(this, new SingleEventArgs<bool>(shadowCopy));
        }

        public void OnSaveFilter(string filterName)
        {
            if (SaveFilter != null)
                SaveFilter(this, new SingleEventArgs<string>(filterName));
        }

        public void OnApplyFilter(string filterName)
        {
            if (ApplyFilter != null)
                ApplyFilter(this, new SingleEventArgs<string>(filterName));
        }

        public void OnDeleteFilter(string filterName)
        {
            if (DeleteFilter != null)
                DeleteFilter(this, new SingleEventArgs<string>(filterName));
        }

        public void LoadComplete()
        {
            Sync.Invoke(this, delegate
            {
                testExplorer.ExpandAll();
                startButton.Enabled = true;
                startTestsToolStripMenuItem.Enabled = true;
            });
        }

        public void UpdateSelectedNode(IList<string> testIds)
        {
            if (testIds.Count > 0)
            {
                executionLogTaskManager.StartTask(delegate
                {
                    if (GetExecutionLog != null)
                        GetExecutionLog(this, new SingleEventArgs<IList<string>>(testIds));
                });
            }
            testResults.SelectedNodeIds = testIds;
        }

        private static void ReportUnhandledException(object sender, CorrelatedExceptionEventArgs e)
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
            //Sync.Invoke(this, delegate
            //{
            //    MessageBox.Show(this, e.GetDescription(), e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //});
        }

        private void UpdateProgress()
        {
            toolStripProgressBar.Maximum = Convert.ToInt32(totalWorkUnits);
            toolStripProgressBar.Value = Convert.ToInt32(completedWorkUnits);

            progressMonitor.TotalWorkUnits = Convert.ToInt32(totalWorkUnits);
            progressMonitor.CompletedWorkUnits = Convert.ToInt32(completedWorkUnits);

            progressMonitor.TaskName = taskName;
            progressMonitor.SubTaskName = subTaskName;
            progressMonitor.Progress = totalWorkUnits > 0 ? String.Format("{0:P}", (completedWorkUnits / totalWorkUnits)) : String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.Append(taskName);
            if (subTaskName != String.Empty)
            {
                sb.Append(" - ");
                sb.Append(subTaskName);
            }
            if (totalWorkUnits > 0)
                sb.Append(String.Format(" ({0:P})", (completedWorkUnits / totalWorkUnits)));
            toolStripStatusLabel.Text = sb.ToString();
        }
    }
}
