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
        private TreeNode[] testTreeCollection;

        public Main()
        {
            InitializeComponent();
                    
            // Set the application version in the window title.
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
            Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Text = String.Format(Text, appVersion.Major, appVersion.Minor);

            treeFilterCombo.SelectedIndex = 1;
            filterTestResultsCombo.SelectedIndex = 0;
            graphsFilterBox1.SelectedIndex = 0;

            testTree.Sort();
            testTree.TestStateImageList = stateImages;
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

        private void tlbStart_Click(object sender, EventArgs e)
        {
            testProgressStatusBar.Clear();
            testProgressStatusBar.Total = 50;
            for (int i = 0; i < testProgressStatusBar.Total; i++)
            {
                if (i == 12 || i == 20 || i == 28 || i == 29 || i == 38 || i == 40 || i == 45 || i == 46 || i == 47 ||
                    i == 18 || i == 25)
                    testProgressStatusBar.Failed += 1;
                else if (i == 30 || i == 42)
                    testProgressStatusBar.Ignored += 1;
                else
                    testProgressStatusBar.Passed += 1;

                Application.DoEvents();
                Thread.Sleep(50);
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
            if(openFile.ShowDialog() == DialogResult.OK)
            {
                GetTheTestTree(openFile.FileName);
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
            ((TestTreeNode) testTree.SelectedNode).TestState = TestStates.Failed;
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
            TestNodes(testTree.Nodes[0], TestStates.Failed);

            testTree.EndUpdate();
        }

        private void TestNodes(TreeNode node, TestStates state)
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
                    testNode.TestState = TestStates.Undefined;
            }
        }

        #endregion

        public event EventHandler<ProjectLoadEventArgs> GetTestTree;

        public void GetTheTestTree(string assembly)
        {
            ProjectLoadEventArgs loadeventargs = new ProjectLoadEventArgs(assembly);

            EventHandler<ProjectLoadEventArgs> getTestTree = GetTestTree;
            if (getTestTree != null)
            {
                getTestTree(this, loadeventargs);
                //bind test tree to control
            }
        }

        public TreeNode[] TestTreeCollection
        {
            set { testTreeCollection = value; }
        }

        public void DataBind()
        {
            testTree.Nodes.AddRange(testTreeCollection);
        }
    }
}