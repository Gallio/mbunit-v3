// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Windows.Forms;
using Gallio.Icarus.Controls;
using Gallio.Icarus.Controls.Enums;
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Interfaces;
using Gallio.Icarus.Properties;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using ZedGraph;
using Timer=System.Timers.Timer;

namespace Gallio.Icarus
{
    public partial class Main : Form, IProjectAdapterView
    {
        private TreeNode[] testTreeCollection;
        private ListViewItem[] assemblies;
        private Thread workerThread = null;
        // status bar
        private string statusText = string.Empty;
        private int totalWorkUnits, completedWorkUnits;
        private Timer statusBarTimer;
        private string projectFileName = String.Empty;
        
        public TreeNode[] TestTreeCollection
        {
            set { testTreeCollection = value; }
        }

        public ListViewItem[] Assemblies
        {
            set { assemblies = value; }
        }

        public string StatusText
        {
            set { statusText = value; }
        }

        public int TotalWorkUnits
        {
            set { totalWorkUnits = value; }
        }

        public int CompletedWorkUnits
        {
            set { completedWorkUnits = value; }
        }

        public string LogBody
        {
            set { logBody.Text = value; }
        }

        public string ReportPath
        {
            set
            {
                Invoke(new MethodInvoker(delegate()
                {
                    if (value != "")
                    {
                        reportViewer.Url = new Uri(value);
                    }
                }));
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
                    foreach (string reportType in value)
                    {
                        reportTypes.Items.Add(reportType);
                    }
                    if (value.Count > 0)
                        reportTypes.SelectedIndex = 0;
                }
            }
        }

        public IList<string> AvailableLogStreams
        {
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate()
                    {
                        AvailableLogStreams = value;
                    });
                }
                else
                {
                    logStream.Items.Clear();
                    foreach (string log in value)
                    {
                        logStream.Items.Add(log);
                    }
                    if (logStream.Items.Count > 0)
                        logStream.SelectedIndex = 0;
                    
                    UpdateLogBody();
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

        public event EventHandler<SingleStringEventArgs> GetTestTree;
        public event EventHandler<AddAssembliesEventArgs> AddAssemblies;
        public event EventHandler<EventArgs> RemoveAssemblies;
        public event EventHandler<SingleStringEventArgs> RemoveAssembly;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<GetLogStreamEventArgs> GetLogStream;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        public event EventHandler<SingleStringEventArgs> SaveProject;
        public event EventHandler<OpenProjectEventArgs> OpenProject;
        public event EventHandler<EventArgs> NewProject;
        public event EventHandler<SingleStringEventArgs> GetAvailableLogStreams;

        public Main()
        {
            InitializeComponent();

            // status bar
            statusBarTimer = new Timer(50);
            statusBarTimer.AutoReset = true;
            statusBarTimer.Enabled = true;
            statusBarTimer.Elapsed += new ElapsedEventHandler(statusBarTimer_Elapsed);
            statusBarTimer.Start();
        }

        private void UpdateGraph()
        {
            GraphPane graphPane = zedGraphControl1.GraphPane;
            graphPane.Title.Text = "Total Test Results";
            graphPane.XAxis.Title.Text = "Number of Tests";
            graphPane.YAxis.Title.Text = "Test Suites";

            // Make up some data points
            string[] labels = { "Class 1", "Class 2" };
            double[] x = { 1, 2 };
            double[] x2 = { 1, 5 };
            double[] x3 = { 4, 10 };

            // Generate a red bar with "Curve 1" in the legend
            BarItem myCurve = graphPane.AddBar("Fail", x, null, Color.Red);
            // Fill the bar with a red-white-red color gradient for a 3d look
            myCurve.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red, 90f);

            // Generate a blue bar with "Curve 2" in the legend
            myCurve = graphPane.AddBar("Ignore", x2, null, Color.Yellow);
            // Fill the bar with a Blue-white-Blue color gradient for a 3d look
            myCurve.Bar.Fill = new Fill(Color.Blue, Color.White, Color.Blue, 90f);

            // Generate a green bar with "Curve 3" in the legend
            myCurve = graphPane.AddBar("Pass", x3, null, Color.Green);
            // Fill the bar with a Green-white-Green color gradient for a 3d look
            myCurve.Bar.Fill = new Fill(Color.Green, Color.White, Color.Green, 90f);

            // Draw the Y tics between the labels instead of at the labels
            graphPane.YAxis.MajorTic.IsBetweenLabels = true;

            // Set the YAxis labels
            graphPane.YAxis.Scale.TextLabels = labels;
            // Set the YAxis to Text type
            graphPane.YAxis.Type = AxisType.Text;

            // Set the bar type to stack, which stacks the bars by automatically accumulating the values
            graphPane.BarSettings.Type = BarType.Stack;

            // Make the bars horizontal by setting the BarBase to "Y"
            graphPane.BarSettings.Base = BarBase.Y;

            // Fill the chart background with a color gradient
            graphPane.Chart.Fill = new Fill(Color.White,
                                            Color.FromArgb(255, 255, 166), 45.0F);

            zedGraphControl1.AxisChange();
        }

        private void Form_Load(object sender, EventArgs e)
        {
            // Set the application version in the window title
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Text = String.Format(Text, appVersion.Major, appVersion.Minor);

            treeFilterCombo.SelectedIndex = 0;
            filterTestResultsCombo.SelectedIndex = 0;
            graphsFilterBox1.SelectedIndex = 0;

            testTree.TestStateImageList = stateImages;

            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                if (GetReportTypes != null)
                    GetReportTypes(this, EventArgs.Empty);

                ThreadedRefreshTree();
            });
            workerThread.Start();
        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();

            //if (aboutForm != null)
            aboutForm.Dispose();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            // reset progress monitors
            testProgressStatusBar.Clear();
            testTree.BeginUpdate();
            testTree.Reset(testTree.Nodes);
            testTree.EndUpdate();
            
            statusText = "Running tests...";
            startButton.Enabled = false;
            stopButton.Enabled = true;

            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                // run tests
                if (RunTests != null)
                {
                    RunTests(this, new EventArgs());
                }

                // enable/disable buttons
                toolStripContainer.Invoke((MethodInvoker)delegate()
                {
                    stopButton.Enabled = false;
                    startButton.Enabled = true;
                });
            });
            workerThread.Start();
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Hide();
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                Hide();
            }
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            ////trayIcon.Icon = Resources.FailMb;
            ////trayIcon.ShowBalloonTip(5, "Gallio Test Notice", "Recent changes have caused 5 of your unit tests to fail.",
            ////                        ToolTipIcon.Error);
            //List<TaskButton> taskButtons = new List<TaskButton>();

            //TaskButton button1 = new TaskButton();
            //button1.Text = "Button 1";
            //button1.Icon = Resources.tick;
            //button1.Description = "This is the first button, it should explain what the option does.";
            //taskButtons.Add(button1);

            //TaskButton button2 = new TaskButton();
            //button2.Text = "Button 2";
            //button2.Icon = Resources.help_browser;
            //button2.Description =
            //    "This is the second button, much the same as the first button but this one demonstrates that the text will wrap onto the next line.";
            //taskButtons.Add(button2);

            //TaskButton button3 = new TaskButton();
            //button3.Text = "Close Window";
            //button3.Icon = Resources.cross;
            //button3.Description = "Saves all changes and exits the application.";
            //taskButtons.Add(button3);

            //TaskButton res = TaskDialog.Show("Title Text",
            //                                 "Description about the problem and what you need to do to resolve it. Each button can have its own description too.",
            //                                 taskButtons);
            //if (res == button2)
            //    MessageBox.Show("Button 2 was clicked.");
            //else if (res == button1)
            //    MessageBox.Show("Button 1 was clicked.");

            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                StatusText = "Reloading...";
                ThreadedRefreshTree();
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
                //AbortWorkerThread();
                //workerThread = new Thread(delegate()
                //{
                //    StatusText = "Opening project";
                    if (OpenProject != null)
                        OpenProject(this, new OpenProjectEventArgs(openFile.FileName, GetTreeFilter()));
                //});
                //workerThread.Start();
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
                if (SaveProject != null)
                {
                    SaveProject(this, new SingleStringEventArgs(projectFileName));
                }
            });
            workerThread.Start();
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*";
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                AbortWorkerThread();
                workerThread = new Thread(delegate()
                {
                    StatusText = "Adding assemblies...";
                    // Add assemblies
                    if (AddAssemblies != null)
                    {
                        AddAssemblies(this, new AddAssembliesEventArgs(openFile.FileNames));
                    }
                    ThreadedRefreshTree();
                });
                workerThread.Start();
            }
        }

        private void ThreadedRefreshTree()
        {
            // Load test tree
            if (GetTestTree != null)
            {
                GetTestTree(this, new SingleStringEventArgs(GetTreeFilter()));
            }
        }

        private string GetTreeFilter()
        {
            if (treeFilterCombo.InvokeRequired)
            {
                string treeFilter = "";
                treeFilterCombo.Invoke((MethodInvoker)delegate()
                {
                    treeFilter = (string)treeFilterCombo.SelectedItem;
                });
                return treeFilter;
            }
            else
            {
                return (string)treeFilterCombo.SelectedItem;
            }
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            Options options = new Options();
            options.ShowDialog();

            if (!options.IsDisposed)
                options.Dispose();
        }

        private void helpToolbarButton_Click(object sender, EventArgs e)
        {
            ((TestTreeNode) testTree.SelectedNode).TestState = TestState.Failed;
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Test Tree Context Menu

        private void expandAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.BeginUpdate();
            testTree.ExpandAll();
            testTree.EndUpdate();
        }

        private void collapseAllMenuItem_Click(object sender, EventArgs e)
        {
            testTree.BeginUpdate();
            testTree.CollapseAll();
            testTree.EndUpdate();
        }

        private void expandFailedMenuItem_Click(object sender, EventArgs e)
        {
            testTree.BeginUpdate();

            testTree.CollapseAll();
            TestNodes(testTree.Nodes[0], TestState.Failed);

            testTree.EndUpdate();
        }

        private void TestNodes(TreeNode node, TestState state)
        {
            if (node is TestTreeNode)
            {
                if (((TestTreeNode) node).TestState == state)
                    ExpandNode(node);
            }

            // Loop though all the child nodes and expand them if they
            // meet the test state.
            foreach (TreeNode tNode in node.Nodes)
                TestNodes(tNode, state);
        }

        private void ExpandNode(TreeNode node)
        {
            // Loop through all parent nodes that are not already
            // expanded and expand them.
            if (node.Parent != null && !node.Parent.IsExpanded)
                ExpandNode(node.Parent);

            node.Expand();
        }

        private void resetTestsMenuItem_Click(object sender, EventArgs e)
        {
            if (testTree.Nodes.Count > 0)
            {
                testTree.BeginUpdate();
                ClearResults(testTree.Nodes[0]);
                testTree.EndUpdate();

                testProgressStatusBar.Clear();
                testProgressStatusBar.Total = 50;

                testResultsList.Items.Clear();
            }
        }

        private void ClearResults(TreeNode node)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode child in node.Nodes)
                    ClearResults(child);
            }
            else
            {
                TestTreeNode testNode = node as TestTreeNode;
                if (testNode != null)
                    testNode.TestState = TestState.Undefined;
            }
        }

        #endregion

        public void DataBind()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate()
                {
                    DataBind();
                });
            }
            else
            {
                // populate tree
                testTree.Nodes.Clear();
                testTree.Nodes.AddRange(testTreeCollection);

                // populate assembly list
                assemblyList.Items.Clear();
                assemblyList.Items.AddRange(assemblies);

                // clear test results
                testResultsList.Items.Clear();
            }
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                // remove assemblies
                StatusText = "Removing assemblies...";
                if (RemoveAssemblies != null)
                {
                    RemoveAssemblies(this, new EventArgs());
                }
                ThreadedRefreshTree();
            });
            workerThread.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (workerThread != null)
            {
                workerThread.Abort();
            }
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

        private void statusBarTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (statusStrip.InvokeRequired)
                {
                    statusStrip.Invoke(new MethodInvoker(UpdateStatusBar));
                }
                else
                {
                    UpdateStatusBar();
                }
            }
            catch { }
        }

        private void UpdateStatusBar()
        {
            // status bar
            toolStripStatusLabel.Text = statusText;
            toolStripProgressBar.Maximum = totalWorkUnits;
            toolStripProgressBar.Value = completedWorkUnits;
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
                Color foreColor = testResultsList.ForeColor;
                switch (testStepRun.Result.Outcome)
                {
                    case TestOutcome.Passed:
                        testProgressStatusBar.Passed++;
                        testTree.UpdateTestState(testData.Id, TestState.Success);
                        foreColor = Color.Green;
                        break;
                    case TestOutcome.Failed:
                        testProgressStatusBar.Failed++;
                        testTree.UpdateTestState(testData.Id, TestState.Failed);
                        foreColor = Color.Red;
                        break;
                    case TestOutcome.Inconclusive:
                        testProgressStatusBar.Ignored++;
                        testTree.UpdateTestState(testData.Id, TestState.Ignored);
                        foreColor = Color.Yellow;
                        break;
                }

                // update test results list
                testResultsList.UpdateTestResults(testData.Name, testStepRun.Result.Outcome.ToString(), foreColor, 
                    (testStepRun.EndTime - testStepRun.StartTime).TotalMilliseconds.ToString(), testData.CodeReference.TypeName, 
                    testData.CodeReference.NamespaceName, testData.CodeReference.AssemblyName);
            }
        }

        public void TotalTests(int totalTests)
        {
            if (testProgressStatusBar.InvokeRequired)
            {
                testTree.Invoke((MethodInvoker)delegate()
                {
                    TotalTests(totalTests);
                });
            }
            else
            {
                testProgressStatusBar.Total = totalTests;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(delegate()
            {
                if (StopTests != null)
                {
                    StopTests(this, new EventArgs());
                }
                // enable/disable buttons
                toolStripContainer.Invoke((MethodInvoker)delegate()
                {
                    stopButton.Enabled = false;
                    startButton.Enabled = true;
                });
            });
            workerThread.Start();
        }

        private void assemblyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assemblyList.SelectedItems.Count > 0)
            {
                removeAssemblyToolStripMenuItem1.Enabled = true;
            }
            else
            {
                removeAssemblyToolStripMenuItem1.Enabled = false;
            }
        }

        private void removeAssemblyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(new ParameterizedThreadStart(ThreadedRemoveAssembly));
            workerThread.Start(assemblyList.SelectedItems[0].SubItems[2].Text);
        }

        private void ThreadedRemoveAssembly(object o)
        {
            string assembly = o as string;
            // remove assemblies
            StatusText = "Removing assembly...";
            if (RemoveAssembly != null)
            {
                RemoveAssembly(this, new SingleStringEventArgs(assembly));
            }
            ThreadedRefreshTree();
        }

        private void testTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TestTreeNode node = testTree.SelectedNode as TestTreeNode;
            if (node != null)
            {
                // if "Assembly" node
                if (node.SelectedImageIndex == 2)
                {
                    // enable "Remove assembly" context item
                    removeAssemblyToolStripMenuItem2.Enabled = true;
                }
                else
                {
                    // disable it
                    removeAssemblyToolStripMenuItem2.Enabled = false;
                }

                if (GetAvailableLogStreams != null)
                    GetAvailableLogStreams(this, new SingleStringEventArgs(node.Name));
            }
        }

        private void removeAssemblyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(new ParameterizedThreadStart(ThreadedRemoveAssembly));
            TestTreeNode node = testTree.SelectedNode as TestTreeNode;
            workerThread.Start(node.CodeBase);
        }

        private void testTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (SetFilter != null)
                {
                    SetFilter(this, new SetFilterEventArgs("Latest", testTree.Nodes));
                }
                testTree.Reset(testTree.Nodes);
                testProgressStatusBar.Clear();
                testProgressStatusBar.Total = testTree.CountTests(testTree.Nodes, new List<string>());
            }
        }

        private void treeFilterCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(new ThreadStart(ThreadedRefreshTree));
            workerThread.Start();
        }

        private void logStream_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLogBody();
        }

        private void UpdateLogBody()
        {
            TestTreeNode node = testTree.SelectedNode as TestTreeNode;
            if (node != null && node.SelectedImageIndex == 4 && node.TestState != TestState.Undefined &&
                GetLogStream != null && logStream.SelectedItem != null)
                // display log stream (if available)
                GetLogStream(this, new GetLogStreamEventArgs(logStream.SelectedItem.ToString(), node.Name));
            else
                logBody.Clear();
        }

        private void btnSaveReportAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.OverwritePrompt = true;
            saveFile.AddExtension = true;
            string ext = "All files (*.*)|*.*";
            switch ((string)reportTypes.SelectedItem)
            {
                case "Xml":
                case "Xml-Inline":
                    ext = "XML files (*.xml)|*.xml";
                    break;
                case "Html":
                case "Html-Inline":
                    ext = "HTML files (*.html)|*.html";
                    break;
                case "Xhtml":
                case "Xhtml-Inline":
                    ext = "XHTML files (*.xhtml)|*.xhtml";
                    break;
                case "Text":
                    ext = "Text files (*.txt)|*.txt";
                    break;
            }
            saveFile.DefaultExt = ext;
            saveFile.Filter = ext;
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                if (SaveReportAs != null)
                {
                    SaveReportAs(this, new SaveReportAsEventArgs(saveFile.FileName, (string)reportTypes.SelectedItem));
                }
            }
        }

        private void reportTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSaveReportAs.Enabled = ((string)reportTypes.SelectedItem != "");
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
                if (NewProject != null)
                    NewProject(this, EventArgs.Empty);
                ThreadedRefreshTree();
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
                {
                    ApplyFilter(childFilter);
                }
            }
            else if (filter is IdFilter<ITest>)
            {
                IdFilter<ITest> idFilter = (IdFilter<ITest>)filter;
                TreeNode[] nodes = testTree.Nodes.Find(idFilter.ToString().Substring(13, 16), true);
                foreach (TestTreeNode n in nodes)
                    n.Toggle();
            }
        }

        private void logStreamsTabPage_Click(object sender, EventArgs e)
        {

        }
    }	
}
