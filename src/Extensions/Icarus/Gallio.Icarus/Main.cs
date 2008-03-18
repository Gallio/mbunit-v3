// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Windows.Forms;

using Aga.Controls.Tree;

using Castle.Core.Logging;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Interfaces;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Utilities;

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
        private LogWindow runtimeWindow;
        private PerformanceMonitor performanceMonitor;
        private About aboutDialog;
        private PropertiesWindow propertiesWindow;
        private FiltersWindow filtersWindow;
        
        public ITreeModel TreeModel
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                        {
                            TreeModel = value;
                        }));
                }
                else
                {
                    testExplorer.TreeModel = value;
                    ((TestTreeModel)((SortedTreeModel)value).InnerModel).TestCountChanged += delegate
                    {
                        TotalTests = ((TestTreeModel)((SortedTreeModel)value).InnerModel).TestCount;
                    };
                    ((TestTreeModel)((SortedTreeModel)value).InnerModel).TestResult += delegate(object sender, TestResultEventArgs e)
                    {
                        testResults.UpdateTestResults(e.TestName, e.TestOutcome, e.Duration, e.TypeName, e.NamespaceName, e.AssemblyName);
                        performanceMonitor.UpdateTestResults(e.TestOutcome, e.TypeName, e.NamespaceName, e.AssemblyName);
                    };
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
                {
                    testResults.TotalTests = value;
                }
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
                    settings = LoadSettings();
                    if (settings == null)
                        settings = new Settings();
                }
                return settings;
            }
            set
            {
                if (settings == null)
                    throw new ArgumentNullException("value");
                settings = value;
            }
        }

        private Settings LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFile))
                    return SerializationUtils.LoadFromXml<Settings>(settingsFile);
            }
            catch
            { }
            return null;
        }

        public IList<string> HintDirectories
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                        {
                            HintDirectories = value;
                        }));
                }
                else
                    propertiesWindow.HintDirectories = value;
            }
        }

        public string ApplicationBaseDirectory
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        ApplicationBaseDirectory = value;
                    }));
                }
                else
                    propertiesWindow.ApplicationBaseDirectory = value;
            }
        }

        public string WorkingDirectory
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        WorkingDirectory = value;
                    }));
                }
                else
                    propertiesWindow.WorkingDirectory = value;
            }
        }

        public bool ShadowCopy
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        ShadowCopy = value;
                    }));
                }
                else
                    propertiesWindow.ShadowCopy = value;
            }
        }

        public string ProjectFileName
        {
            set { projectFileName = value; }
        }

        public IList<string> TestFilters
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate()
                    {
                        TestFilters = value;
                    }));
                }
                else
                    filtersWindow.Filters = value;
            }
        }

        public event EventHandler<GetTestTreeEventArgs> GetTestTree;
        public event EventHandler<SingleEventArgs<IList<string>>> AddAssemblies;
        public event EventHandler<EventArgs> RemoveAssemblies;
        public event EventHandler<SingleEventArgs<string>> RemoveAssembly;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> GenerateReport;
        public event EventHandler<EventArgs> StopTests;
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

        public Main()
        {
            InitializeComponent();

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
            runtimeWindow = new LogWindow("Runtime");
            performanceMonitor = new PerformanceMonitor();
            aboutDialog = new About();
            propertiesWindow = new PropertiesWindow(this);
            filtersWindow = new FiltersWindow(this);

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
            else if (persistString == typeof(PropertiesWindow).ToString())
                return propertiesWindow;
            else if (persistString == typeof(FiltersWindow).ToString())
                return filtersWindow;
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
                    case "Runtime":
                        return runtimeWindow;
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
                runtimeWindow.Show(dockPanel, DockState.DockBottomAutoHide);
                testExplorer.Show(dockPanel, DockState.DockLeft);
            }

            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                if (GetReportTypes != null)
                    GetReportTypes(this, EventArgs.Empty);
                if (GetTestFrameworks != null)
                    GetTestFrameworks(this, EventArgs.Empty);
                if (projectFileName != string.Empty)
                    OpenProjectFromFile(projectFileName);
                else
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
                StatusText = "Running tests";

                // reset progress monitors
                Reset();

                // enable/disable buttons
                startButton.Enabled = startTestsToolStripMenuItem.Enabled = false;
                stopButton.Enabled = stopTestsToolStripMenuItem.Enabled = true;

                AbortWorkerThread();
                workerThread = new Thread(delegate()
                {
                    // save test filter
                    OnSaveFilter("LastRun");
                    
                    // run tests
                    if (RunTests != null)
                        RunTests(this, new EventArgs());

                    // create report (if necessary)
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

        private void openProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Gallio Projects (*.gallio)|*.gallio";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                AbortWorkerThread();
                workerThread = new Thread(delegate()
                {
                    OpenProjectFromFile(openFile.FileName);
                });
                workerThread.Start();
            }
        }

        private void OpenProjectFromFile(string fileName)
        {
            StatusText = "Opening project";
            try
            {
                if (OpenProject != null)
                    OpenProject(this, new OpenProjectEventArgs(fileName, testExplorer.TreeFilter));
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            StatusText = string.Empty;
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
                        SaveProject(this, new SingleEventArgs<string>(projectFileName));
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
                            AddAssemblies(this, new SingleEventArgs<IList<string>>(openFile.FileNames));
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
                    RemoveAssembly(this, new SingleEventArgs<string>(assembly));
                ThreadedReloadTree(true);
                StatusText = string.Empty;
            });
            workerThread.Start();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveProjectToFile();
        }

        private void openProjectToolStripButton_Click(object sender, EventArgs e)
        {

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
                logBody = Environment.NewLine + logBody;
                switch (logName)
                {
                    case LogStreamNames.ConsoleError:
                        consoleErrorWindow.AppendText(logBody);
                        break;
                    case LogStreamNames.ConsoleInput:
                        consoleInputWindow.AppendText(logBody);
                        break;
                    case LogStreamNames.ConsoleOutput:
                        consoleOutputWindow.AppendText(logBody);
                        break;
                    case LogStreamNames.DebugTrace:
                        debugTraceWindow.AppendText(logBody);
                        break;
                    case LogStreamNames.Default:
                        logWindow.AppendText(logBody);
                        break;
                    case LogStreamNames.Failures:
                        failuresWindow.AppendText(logBody);
                        break;
                    case LogStreamNames.Warnings:
                        warningsWindow.AppendText(logBody);
                        break;
                }
            }
        }

        public void WriteToLog(LoggerLevel level, string name, string message, Exception exception)
        {
            Color color = Color.Black;
            switch (level)
            {
                case LoggerLevel.Fatal:
                case LoggerLevel.Error:
                    color = Color.Red;
                    break;

                case LoggerLevel.Warn:
                    color = Color.Yellow;
                    break;

                case LoggerLevel.Info:
                    color = Color.Gray;
                    break;

                case LoggerLevel.Debug:
                    color = Color.DarkGray;
                    break;
            }
            runtimeWindow.AppendText(message, color);
            runtimeWindow.AppendText(ExceptionUtils.SafeToString(exception), color);
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
                // create folder (if necessary)
                string gallioDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gallio/Icarus");
                if (!Directory.Exists(gallioDir))
                    Directory.CreateDirectory(gallioDir);

                // save test filter
                OnSaveFilter("AutoSave");

                // save project
                if (SaveProject != null)
                    SaveProject(this, new SingleEventArgs<string>(Path.Combine(gallioDir, "Icarus.gallio")));

                // save dock panel config
                dockPanel.SaveAsXml(dockConfigFile);
            }
            catch
            {
                // eat any exceptions
            }
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
                case "runtimeToolStripMenuItem":
                    runtimeWindow.Show(dockPanel);
                    break;
                case "propertiesToolStripMenuItem":
                    propertiesWindow.Show(dockPanel);
                    break;
                case "testFiltersToolStripMenuItem":
                    filtersWindow.Show(dockPanel);
                    break;
            }
        }

        public void AssemblyChanged(string filePath)
        {
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
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { LoadComplete(); });
            }
            else
            {
                testExplorer.ExpandAll();
                startButton.Enabled = true;
                startTestsToolStripMenuItem.Enabled = true;
            }
        }
    }
}
