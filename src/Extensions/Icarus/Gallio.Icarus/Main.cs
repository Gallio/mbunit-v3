// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Windows.Forms;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Controls.Enums;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Runner.Reports;

using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class Main : Form, IProjectAdapterView
    {
        private Thread workerThread = null;
        
        private string projectFileName = String.Empty;
        private Settings settings;
        private string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Gallio/Icarus/Icarus.settings");
        
        // dock panel windows
        private DeserializeDockContent deserializeDockContent;
        private string dockConfigFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Gallio/Icarus/DockPanel.config");
        private TestExplorer testExplorer;
        private AssemblyList assemblyList;
        private TestResults testResults;
        private ReportWindow reportWindow;
        private LogWindow logWindow;
        private LogWindow consoleInputWindow;
        private LogWindow consoleOutputWindow;
        private LogWindow consoleErrorWindow;
        private LogWindow debugTraceWindow;
        private LogWindow warningsWindow;
        private LogWindow failuresWindow;
        private PerformanceMonitor performanceMonitor;
        private About aboutDialog;
        
        public TreeNode[] TestTreeCollection
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                        {
                            TestTreeCollection = value;
                        }));
                }
                else
                {
                    testExplorer.DataBind(value);
                    startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                }
            }
        }

        public ListViewItem[] Assemblies
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        Assemblies = value;
                    }));
                }
                else
                {
                    assemblyList.DataBind(value);
                }
            }
        }

        public string StatusText
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate()
                    {
                        StatusText = value;
                    });
                }
                else
                    toolStripStatusLabel.Text = value;
            }
        }

        public int TotalWorkUnits
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate()
                    {
                        TotalWorkUnits = value;
                    });
                }
                else
                    toolStripProgressBar.Maximum = value;
            }
        }

        public int CompletedWorkUnits
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate()
                    {
                        CompletedWorkUnits = value;
                    });
                }
                else
                    toolStripProgressBar.Value = value;
            }
        }

        public int TotalTests
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate()
                    {
                        TotalTests = value;
                    });
                }
                else
                    testResults.Total = value;
            }
        }

        public string ReportPath
        {
            set
            {
                if (value != "")
                {
                    if (InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate()
                        {
                            ReportPath = value;
                        }));
                    }
                    else
                    {
                        reportWindow.ReportPath = value;
                    }
                }
            }
        }

        public IList<string> ReportTypes
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        ReportTypes = value;
                    }));
                }
                else
                {
                    reportWindow.ReportTypes = value;
                }
            }
        }

        public IList<string> TestFrameworks
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        TestFrameworks = value;
                    }));
                }
                else
                {
                    aboutDialog.TestFrameworks = value;
                }
            }
        }

        public Exception Exception
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        Exception = value;
                    }));
                }
                else
                {
                    MessageBox.Show(String.Format("Message: {0}\nStack trace: {1}", value.Message, value.StackTrace), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public CodeLocation SourceCodeLocation
        {
            set
            {
                CodeWindow codeWindow = new CodeWindow(value);
                codeWindow.Show(dockPanel, DockState.Document);
            }
        }

        public Settings Settings
        {
            get
            {
                if (settings == null)
                {
                    try
                    {
                        if (File.Exists(settingsFile))
                            settings = SerializationUtils.LoadFromXml<Settings>(settingsFile);
                    }
                    catch
                    {
                        settings = new Settings();
                    }
                }
                return settings;
            }
            set
            {
                if (settings == null)
                    throw new ArgumentNullException("settings");
                settings = value;
            }
        }

        public event EventHandler<GetTestTreeEventArgs> GetTestTree;
        public event EventHandler<AddAssembliesEventArgs> AddAssemblies;
        public event EventHandler<EventArgs> RemoveAssemblies;
        public event EventHandler<SingleStringEventArgs> RemoveAssembly;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> GenerateReport;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<EventArgs> GetTestFrameworks;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        public event EventHandler<SingleStringEventArgs> SaveProject;
        public event EventHandler<OpenProjectEventArgs> OpenProject;
        public event EventHandler<EventArgs> NewProject;
        public event EventHandler<SingleStringEventArgs> GetSourceLocation;

        public Main()
        {
            InitializeComponent();

            settings = new Settings();

            testExplorer = new TestExplorer(this);
            assemblyList = new AssemblyList(this);
            testResults = new TestResults();
            reportWindow = new ReportWindow(this);
            logWindow = new LogWindow();
            consoleInputWindow = new LogWindow("Console input");
            consoleOutputWindow = new LogWindow("Console output");
            consoleErrorWindow = new LogWindow("Console error");
            debugTraceWindow = new LogWindow("Debug trace");
            warningsWindow = new LogWindow("Warnings");
            failuresWindow = new LogWindow("Failures");
            performanceMonitor = new PerformanceMonitor();
            aboutDialog = new About();

            deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            if (persistString == typeof(TestExplorer).ToString())
                return testExplorer;
            else if (persistString == typeof(AssemblyList).ToString())
                return assemblyList;
            else if (persistString == typeof(TestResults).ToString())
                return testResults;
            else if (persistString == typeof(ReportWindow).ToString())
                return reportWindow;
            else if (persistString == typeof(PerformanceMonitor).ToString())
                return performanceMonitor;
            else
            {
                string[] parsedStrings = persistString.Split(new char[] { ',' });
                if (parsedStrings.Length != 2)
                    return null;
                if (parsedStrings[0] != typeof(LogWindow).ToString())
                    return null;
                switch (parsedStrings[1])
                {
                    case "Log":
                        return logWindow;
                    case "Console input":
                        return consoleInputWindow;
                    case "Console output":
                        return consoleOutputWindow;
                    case "Console error":
                        return consoleErrorWindow;
                    case "Debug trace":
                        return debugTraceWindow;
                    case "Warnings":
                        return warningsWindow;
                    case "Failures":
                        return failuresWindow;
                    default:
                        return null;
                }
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // Set the application version in the window title
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Text = String.Format(Text, appVersion.Major, appVersion.Minor);

            if (File.Exists(dockConfigFile))
            {
                try
                {
                    dockPanel.LoadFromXml(dockConfigFile, deserializeDockContent);
                }
                catch
                { }
            }
            else
            {
                assemblyList.Show(dockPanel, DockState.DockLeftAutoHide);
                performanceMonitor.Show(dockPanel);
                testResults.Show(dockPanel);
                consoleInputWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                consoleOutputWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                consoleErrorWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                debugTraceWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                warningsWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                failuresWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                logWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                testExplorer.Show(dockPanel, DockState.DockLeft);
            }

            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                if (GetReportTypes != null)
                    GetReportTypes(this, EventArgs.Empty);
                if (GetTestFrameworks != null)
                    GetTestFrameworks(this, EventArgs.Empty);
                ThreadedReloadTree(true);
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                Reset();

                // enable/disable buttons
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;

                AbortWorkerThread();
                workerThread = new Thread(delegate()
                {
                    // run tests
                    StatusText = "Running tests";
                    if (RunTests != null)
                        RunTests(this, new EventArgs());

                    if (!reportWindow.IsHidden)
                        ThreadedCreateReport();

                    // enable/disable buttons
                    Invoke((MethodInvoker)delegate()
                    {
                        stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                        startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                    });
                    StatusText = string.Empty;
                });
                workerThread.Start();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        //private void ShowTaskDialog()
        //{
        //    //trayIcon.Icon = Resources.FailMb;
        //    //trayIcon.ShowBalloonTip(5, "Gallio Test Notice", "Recent changes have caused 5 of your unit tests to fail.",
        //    //                        ToolTipIcon.Error);
        //    List<TaskButton> taskButtons = new List<TaskButton>();

        //    TaskButton button1 = new TaskButton();
        //    button1.Text = "Button 1";
        //    button1.Icon = Resources.tick;
        //    button1.Description = "This is the first button, it should explain what the option does.";
        //    taskButtons.Add(button1);

        //    TaskButton button2 = new TaskButton();
        //    button2.Text = "Button 2";
        //    button2.Icon = Resources.help_browser;
        //    button2.Description =
        //        "This is the second button, much the same as the first button but this one demonstrates that the text will wrap onto the next line.";
        //    taskButtons.Add(button2);

        //    TaskButton button3 = new TaskButton();
        //    button3.Text = "Close Window";
        //    button3.Icon = Resources.cross;
        //    button3.Description = "Saves all changes and exits the application.";
        //    taskButtons.Add(button3);

        //    TaskButton res = TaskDialog.Show("Title Text",
        //                                     "Description about the problem and what you need to do to resolve it. Each button can have its own description too.",
        //                                     taskButtons);
        //    if (res == button2)
        //        MessageBox.Show("Button 2 was clicked.");
        //    else if (res == button1)
        //        MessageBox.Show("Button 1 was clicked.");
        //}

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                StatusText = "Reloading...";
                ThreadedReloadTree(true);
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenProjectFromFile();
        }

        private void OpenProjectFromFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Gallio Projects (*.gallio)|*.gallio";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                AbortWorkerThread();
                workerThread = new Thread(delegate()
                {
                    StatusText = "Opening project";
                    try
                    {
                        if (OpenProject != null)
                            OpenProject(this, new OpenProjectEventArgs(openFile.FileName, testExplorer.TreeFilter));
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                    }
                    StatusText = string.Empty;
                });
                workerThread.Start();
            }
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectFileName = string.Empty;
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
                {
                    projectFileName = saveFile.FileName;
                }
            }
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                StatusText = "Saving project";
                try
                {
                    if (SaveProject != null)
                        SaveProject(this, new SingleStringEventArgs(projectFileName));
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAssembliesToTree();
        }

        public void AddAssembliesToTree()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*";
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                AbortWorkerThread();
                workerThread = new Thread(delegate()
                {
                    StatusText = "Adding assemblies";
                    try
                    {
                        if (AddAssemblies != null)
                            AddAssemblies(this, new AddAssembliesEventArgs(openFile.FileNames));
                        ThreadedReloadTree(true);
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                    }
                    StatusText = string.Empty;
                });
                workerThread.Start();
            }
        }

        public void ReloadTree()
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                ThreadedReloadTree(false);
            });
            workerThread.Start();
        }

        private void ThreadedReloadTree(bool reloadTestModelData)
        {
            StatusText = "Reloading tree";
            if (GetTestTree != null)
                GetTestTree(this, new GetTestTreeEventArgs(testExplorer.TreeFilter, reloadTestModelData));
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options(this);
            if (options.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SerializationUtils.SaveToXml<Settings>(settings, settingsFile);
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }
            }
            if (!options.IsDisposed)
                options.Dispose();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void Reset()
        {
            testExplorer.Reset();
            testResults.Reset();
        }

        private void removeAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAssembliesFromTree();
        }

        public void RemoveAssembliesFromTree()
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                // remove assemblies
                StatusText = "Removing assemblies";
                if (RemoveAssemblies != null)
                    RemoveAssemblies(this, new EventArgs());
                ThreadedReloadTree(true);
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (workerThread != null)
                workerThread.Abort();
            base.OnClosing(e);
        }

        private void AbortWorkerThread()
        {
            if (workerThread != null)
            {
                try
                {
                    StatusText = "Aborting worker thread";
                    workerThread.Join(1000);
                    workerThread.Abort();
                    workerThread.Join(2000);
                    workerThread = null;
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException)
                        return;
                }
            }
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate()
                {
                    Update(testData, testStepRun);
                });
            }
            else
            {
                // update test tree
                Color foreColor = Color.SlateGray;
                switch (testStepRun.Result.Outcome)
                {
                    case TestOutcome.Passed:
                        testResults.Passed++;
                        testExplorer.UpdateTestState(testData.Id, TestStates.Success);
                        foreColor = Color.Green;
                        break;
                    case TestOutcome.Failed:
                        testResults.Failed++;
                        testExplorer.UpdateTestState(testData.Id, TestStates.Failed);
                        foreColor = Color.Red;
                        break;
                    case TestOutcome.Inconclusive:
                        testResults.Inconclusive++;
                        testExplorer.UpdateTestState(testData.Id, TestStates.Inconclusive);
                        foreColor = Color.Yellow;
                        break;
                }

                CodeReference codeReference = testData.CodeReference ?? CodeReference.Unknown;
                
                // update test results list
                testResults.UpdateTestResults(testData.Name, testStepRun.Result.Outcome.ToString(), foreColor, 
                    (testStepRun.EndTime - testStepRun.StartTime).TotalMilliseconds.ToString(), codeReference.TypeName, 
                    codeReference.NamespaceName, codeReference.AssemblyName);

                // update test results graph
                performanceMonitor.UpdateTestResults(testStepRun.Result.Outcome.ToString(), codeReference.TypeName,
                    codeReference.NamespaceName, codeReference.AssemblyName);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            CancelTests();
        }

        private void CancelTests()
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                StatusText = "Stopping tests";
                if (StopTests != null)
                    StopTests(this, new EventArgs());

                // enable/disable buttons
                toolStripContainer.Invoke((MethodInvoker)delegate()
                {
                    stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = false;
                    startButton.Enabled = startTestsToolStripMenuItem.Enabled = true;
                });
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        public void ThreadedRemoveAssembly(string assembly)
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                StatusText = "Removing assembly";
                if (RemoveAssembly != null)
                    RemoveAssembly(this, new SingleStringEventArgs(assembly));
                ThreadedReloadTree(true);
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        public void CreateFilter(TreeNodeCollection nodes)
        {
            if (SetFilter != null)
                SetFilter(this, new SetFilterEventArgs("Latest", nodes));
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectToFile();
        }

        private void openProjectToolStripButton_Click(object sender, EventArgs e)
        {
            OpenProjectFromFile();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        private void CreateNewProject()
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                StatusText = "Creating new project";
                if (NewProject != null)
                    NewProject(this, EventArgs.Empty);
                ThreadedReloadTree(true);
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        private void newProjectToolStripButton_Click(object sender, EventArgs e)
        {
            CreateNewProject();
        }

        public void ApplyFilter(Filter<ITest> filter)
        {
            if (filter is NoneFilter<ITest>)
                return;
            if (filter is OrFilter<ITest>)
            {
                OrFilter<ITest> orFilter = (OrFilter<ITest>)filter;
                foreach (Filter<ITest> childFilter in orFilter.Filters)
                    ApplyFilter(childFilter);
            }
            else if (filter is IdFilter<ITest>)
            {
                IdFilter<ITest> idFilter = (IdFilter<ITest>)filter;
                foreach (TestTreeNode n in testExplorer.FindNodes(idFilter.ToString().Substring(13, 16)))
                    n.Toggle();
            }
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

        private void ShowOnlineHelp()
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

        public void WriteToLog(string logName, string logBody)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate()
                {
                    WriteToLog(logName, logBody);
                });
            }
            else
            {
                switch (logName)
                {
                    case LogStreamNames.ConsoleError:
                        consoleErrorWindow.LogBody += logBody;
                        break;
                    case LogStreamNames.ConsoleInput:
                        consoleInputWindow.LogBody += logBody;
                        break;
                    case LogStreamNames.ConsoleOutput:
                        consoleOutputWindow.LogBody += logBody;
                        break;
                    case LogStreamNames.DebugTrace:
                        debugTraceWindow.LogBody += logBody;
                        break;
                    case LogStreamNames.Default:
                        logWindow.LogBody += logBody;
                        break;
                    case LogStreamNames.Failures:
                        failuresWindow.LogBody += logBody;
                        break;
                    case LogStreamNames.Warnings:
                        warningsWindow.LogBody += logBody;
                        break;
                }
            }
        }

        public void CreateReport()
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
                {
                    try
                    {
                        ThreadedCreateReport();
                        StatusText = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        Exception = ex;
                    }
                });
            workerThread.Start();
        }

        private void ThreadedCreateReport()
        {
            StatusText = "Generating report";
            if (GenerateReport != null)
                GenerateReport(this, EventArgs.Empty);
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string gallioDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gallio/Icarus");
                if (!Directory.Exists(gallioDir))
                    Directory.CreateDirectory(gallioDir);
                if (SaveProject != null)
                    SaveProject(this, new SingleStringEventArgs(Path.Combine(gallioDir, "Icarus.gallio")));
                dockPanel.SaveAsXml(dockConfigFile);
            }
            catch
            { }
        }

        public void ViewSourceCode(string testId)
        {
            if (GetSourceLocation != null)
                GetSourceLocation(this, new SingleStringEventArgs(testId));
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            switch (item.Name)
            {
                case "logToolStripMenuItem":
                    logWindow.Show(dockPanel);
                    break;
                case "consoleInputToolStripMenuItem":
                    consoleInputWindow.Show(dockPanel);
                    break;
                case "consoleOutputToolStripMenuItem":
                    consoleOutputWindow.Show(dockPanel);
                    break;
                case "consoleErrorToolStripMenuItem":
                    consoleErrorWindow.Show(dockPanel);
                    break;
                case "debugTraceToolStripMenuItem":
                    debugTraceWindow.Show(dockPanel);
                    break;
                case "warningsToolStripMenuItem":
                    warningsWindow.Show(dockPanel);
                    break;
                case "failuresToolStripMenuItem":
                    failuresWindow.Show(dockPanel);
                    break;
                case "performanceMonitorToolStripMenuItem":
                    performanceMonitor.Show(dockPanel);
                    break;
                case "testResultsToolStripMenuItem":
                    testResults.Show(dockPanel);
                    break;
                case "assemblyListToolStripMenuItem":
                    assemblyList.Show(dockPanel);
                    break;
                case "testExplorerToolStripMenuItem":
                    testExplorer.Show(dockPanel);
                    break;
                case "reportToolStripMenuItem":
                    CreateReport();
                    reportWindow.Show(dockPanel);
                    break;
            }
        }

        public void AssemblyChanged(string filePath)
        {
        }
    }
}
