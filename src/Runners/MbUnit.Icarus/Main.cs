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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

using MbUnit.Icarus.Controls;
using MbUnit.Icarus.Controls.Enums;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Interfaces;
using MbUnit.Icarus.Properties;

using ZedGraph;

namespace MbUnit.Icarus
{
    public partial class Main : Form, IProjectAdapterView
    {
        #region Variables

        private TreeNode[] testTreeCollection;
        private ListViewItem[] assemblies;
        private Thread workerThread = null;
        
        // status bar
        private string statusText = "Ready...";
        private int totalWorkUnits, completedWorkUnits;
        private System.Timers.Timer statusBarTimer;
        
        #endregion

        #region Properties

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
            get { return statusText; }
            set { statusText = value; }
        }

        public int TotalWorkUnits
        {
            get { return totalWorkUnits; }
            set { totalWorkUnits = value; }
        }

        public int CompletedWorkUnits
        {
            get { return completedWorkUnits; }
            set { completedWorkUnits = value; }
        }
        
        #endregion

        #region Events

        public event EventHandler<EventArgs> GetTestTree;
        public event EventHandler<AddAssembliesEventArgs> AddAssemblies;
        public event EventHandler<EventArgs> RemoveAssemblies;
        public event EventHandler<RemoveAssemblyEventArgs> RemoveAssembly;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;

        #endregion

        #region Delegates

        public delegate void SingleParameterMethodDelegate(object o);
        public delegate void ClearTreeDelegate();
        public delegate void AddRangeToTreeDelegate(TreeNode[] nodes);
        public delegate void ClearListDelegate();
        public delegate void AddRangeToListDelegate(ListViewItem[] items);

        #endregion

        public Main()
        {
            InitializeComponent();

            // status bar
            statusBarTimer = new System.Timers.Timer(50);
            statusBarTimer.AutoReset = true;
            statusBarTimer.Enabled = true;
            statusBarTimer.Elapsed += new ElapsedEventHandler(statusBarTimer_Elapsed);
            statusBarTimer.Start();

            GraphPane graphPane = zedGraphControl1.GraphPane;
            graphPane.Title.Text = "Total Test Results";
            graphPane.XAxis.Title.Text = "Number of Tests";
            graphPane.YAxis.Title.Text = "Test Suites";

            // Make up some data points
            string[] labels = {"Class 1", "Class 2"};
            double[] x = {1, 2};
            double[] x2 = {1, 5};
            double[] x3 = {4, 10};

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

            treeFilterCombo.SelectedIndex = 1;
            filterTestResultsCombo.SelectedIndex = 0;
            graphsFilterBox1.SelectedIndex = 0;

            testTree.TestStateImageList = stateImages;

            AbortWorkerThread();
            workerThread = new Thread(new ThreadStart(ThreadedRefreshTree));
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
            testTree.Reset();
            
            statusText = "Running tests...";
            startButton.Enabled = false;
            stopButton.Enabled = true;

            AbortWorkerThread();
            workerThread = new Thread(new ThreadStart(ThreadedRunTests));
            workerThread.Start();
        }

        private void ThreadedRunTests()
        {
            // run tests
            if (RunTests != null)
            {
                RunTests(this, new EventArgs());
            }
            // enable/disable buttons
            toolStripContainer.Invoke(new SingleParameterMethodDelegate(EnableButtons), new object[] { "RunTestsFinished" });
        }

        private void EnableButtons(object o)
        {
            string mode = o as string;
            if (mode != null)
            {
                switch (mode)
                {
                    case "RunTestsFinished":
                        {
                            stopButton.Enabled = false;
                            startButton.Enabled = true;
                            break;
                        }
                }
            }
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
            //trayIcon.Icon = Resources.FailMb;
            //trayIcon.ShowBalloonTip(5, "MbUnit Test Notice", "Recent changes have caused 5 of your unit tests to fail.",
            //                        ToolTipIcon.Error);
            List<TaskButton> taskButtons = new List<TaskButton>();

            TaskButton button1 = new TaskButton();
            button1.Text = "Button 1";
            button1.Icon = Resources.tick;
            button1.Description = "This is the first button, it should explain what the option does.";
            taskButtons.Add(button1);

            TaskButton button2 = new TaskButton();
            button2.Text = "Button 2";
            button2.Icon = Resources.help_browser;
            button2.Description =
                "This is the second button, much the same as the first button but this one demonstrates that the text will wrap onto the next line.";
            taskButtons.Add(button2);

            TaskButton button3 = new TaskButton();
            button3.Text = "Close Window";
            button3.Icon = Resources.cross;
            button3.Description = "Saves all changes and exits the application.";
            taskButtons.Add(button3);

            TaskButton res = TaskDialog.Show("Title Text",
                                             "Description about the problem and what you need to do to resolve it. Each button can have its own description too.",
                                             taskButtons);
            if (res == button2)
                MessageBox.Show("Button 2 was clicked.");
            else if (res == button1)
                MessageBox.Show("Button 1 was clicked.");
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "MbUnit Projects (*.mbunit)|*.mbunit";
            DialogResult res = openFile.ShowDialog();

            if (res == DialogResult.OK)
                MessageBox.Show(Path.GetFileName(openFile.FileName), Path.GetDirectoryName(openFile.FileName));
        }

        private void saveProjectAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.OverwritePrompt = true;
            saveFile.AddExtension = true;
            saveFile.DefaultExt = "MbUnit Projects (*.mbunit)|*.mbunit";
            saveFile.Filter = "MbUnit Projects (*.mbunit)|*.mbunit";
            saveFile.ShowDialog();
        }

        private void addAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Assemblies or Executables (*.dll, *.exe)|*.dll;*.exe|All Files (*.*)|*.*";
            openFile.Multiselect = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                AbortWorkerThread();
                workerThread = new Thread(new ParameterizedThreadStart(ThreadedAddAssemblies));
                workerThread.Start(openFile.FileNames);
            }
        }

        private void ThreadedAddAssemblies(object o)
        {
            StatusText = "Adding assemblies...";
            string[] filenames = (string[]) o;
            if (filenames != null)
            {
                // Add assemblies
                if (AddAssemblies != null)
                {
                    AddAssemblies(this, new AddAssembliesEventArgs(filenames));
                }
                ThreadedRefreshTree();
            }
        }

        private void ThreadedRefreshTree()
        {
            // Load test tree
            if (GetTestTree != null)
            {
                GetTestTree(this, new EventArgs());
            }
            testTree.Invoke(new MethodInvoker(testTree.Sort));
            StatusText = "Ready...";
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
            // populate tree
            testTree.Invoke(new ClearTreeDelegate(testTree.Nodes.Clear));
            testTree.Invoke(new AddRangeToTreeDelegate(testTree.Nodes.AddRange), new object[] { testTreeCollection });

            // populate assembly list
            assemblyList.Invoke(new ClearListDelegate(assemblyList.Items.Clear));
            assemblyList.Invoke(new AddRangeToListDelegate(assemblyList.Items.AddRange), new object[] { assemblies });
        }

        private void removeAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(new ThreadStart(ThreadedRemoveAssemblies));
            workerThread.Start();
        }

        private void ThreadedRemoveAssemblies()
        {
            // remove assemblies
            StatusText = "Removing assemblies...";
            if (RemoveAssemblies != null)
            {
                RemoveAssemblies(this, new EventArgs());
            }
            ThreadedRefreshTree();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (workerThread != null)
            {
                workerThread.Abort();
            }
            base.OnClosing(e);
        }

        public void AbortWorkerThread()
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
                    StatusText = "Ready...";
                }
                catch (Exception ex)
                {
                    if (ex is System.Threading.ThreadAbortException)
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

        public void Passed(object o)
        {
            if (testTree.InvokeRequired)
            {
                testTree.Invoke(new SingleParameterMethodDelegate(Passed), new object[] { o });
            }
            else
            {
                string testId = o as string;
                if (testId != null)
                {
                    testProgressStatusBar.Passed++;
                    testTree.Passed(testId);
                }
            }
        }

        public void Failed(object o)
        {
            if (testTree.InvokeRequired)
            {
                testTree.Invoke(new SingleParameterMethodDelegate(Failed), new object[] { o });
            }
            else
            {
                string testId = o as string;
                if (testId != null)
                {
                    testProgressStatusBar.Failed++;
                    testTree.Failed(testId);
                }
            }
        }

        public void Ignored(object o)
        {
            if (testTree.InvokeRequired)
            {
                testTree.Invoke(new SingleParameterMethodDelegate(Ignored), new object[] { o });
            }
            else
            {
                string testId = o as string;
                if (testId != null)
                {
                    testProgressStatusBar.Ignored++;
                    testTree.Ignored(testId);
                }
            }
        }

        public void Skipped(object o)
        {
            if (testTree.InvokeRequired)
            {
                testTree.Invoke(new SingleParameterMethodDelegate(Skipped), new object[] { o });
            }
            else
            {
                string testId = o as string;
                if (testId != null)
                {
                    testProgressStatusBar.Skipped++;
                    testTree.Skipped(testId);
                }
            }
        }

        public void TotalTests(object o)
        {
            if (testProgressStatusBar.InvokeRequired)
            {
                testProgressStatusBar.Invoke(new SingleParameterMethodDelegate(TotalTests), new object[] { o });
            }
            else
            {
                int? totalTests = o as int?;
                if (totalTests != null)
                {
                    testProgressStatusBar.Total = (int) totalTests;
                }
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            AbortWorkerThread();
            workerThread = new Thread(new ThreadStart(ThreadedStopTests));
            workerThread.Start();
        }

        private void ThreadedStopTests()
        {
            if (StopTests != null)
            {
                StopTests(this, new EventArgs());
            }
            // enable/disable buttons
            toolStripContainer.Invoke(new SingleParameterMethodDelegate(EnableButtons), new object[] { "RunTestsFinished" });
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
                RemoveAssembly(this, new RemoveAssemblyEventArgs(assembly));
            }
            ThreadedRefreshTree();
        }

        private void testTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TestTreeNode node = testTree.SelectedNode as TestTreeNode;
            if (node.SelectedImageIndex == 2)
            {
                removeAssemblyToolStripMenuItem2.Enabled = true;
            }
            else
            {
                removeAssemblyToolStripMenuItem2.Enabled = false;
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
                    SetFilter(this, new SetFilterEventArgs(testTree.Nodes));
                }
            }
        }

    }
}