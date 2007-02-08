using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

using MbUnit.GUI.Controls;
using MbUnit.GUI.Controls.Enums;

namespace MbUnit.GUI
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            // Set the application version in the window title.
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            this.Text = String.Format(this.Text, appVersion.Major, appVersion.Minor);

        }

        private void Form_Load(object sender, EventArgs e)
        {
            treeFilterCombo.SelectedIndex = 1;
            filterTestResultsCombo.SelectedIndex = 0;

            TestTreeNode project = new TestTreeNode("Test Project 1.0", 0, 0);
            projectTree.Nodes.Add(project);

            TestTreeNode namespaces = new TestTreeNode("Namespaces", 0, 0);
            project.Nodes.Add(namespaces);

            TestTreeNode ns = new TestTreeNode("TestNamespace", 1, 1);
            namespaces.Nodes.Add(ns);

            TestTreeNode cl = new TestTreeNode("Class1", 2, 2);
            ns.Nodes.Add(cl);

            TestTreeNode m1 = new TestTreeNode("TestMethod", 3, 3);
            m1.TestState = TestState.Success;
            cl.Nodes.Add(m1);

            TestTreeNode m2 = new TestTreeNode("AnotherMethod", 3, 3);
            m2.TestState = TestState.Failure;
            cl.Nodes.Add(m2);

            TestTreeNode cl2 = new TestTreeNode("Class2", 2, 2);
            ns.Nodes.Add(cl2);

            TestTreeNode m3 = new TestTreeNode("MethodThatWorks", 3, 3);
            m3.TestState = TestState.Success;
            cl2.Nodes.Add(m3);

            TestTreeNode m4 = new TestTreeNode("DoesntWork", 3, 3);
            m4.TestState = TestState.Failure;
            cl2.Nodes.Add(m4);

            TestTreeNode m5 = new TestTreeNode("DoGetProgress", 3, 3);
            m5.TestState = TestState.Success;
            cl2.Nodes.Add(m5);

            TestTreeNode m6 = new TestTreeNode("BuildTree", 3, 3);
            m6.TestState = TestState.Success;
            cl2.Nodes.Add(m6);

            project.ExpandAll();

        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //private void linkSummary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        //{
        //    if (panelResults.Height >= 149)
        //    {
        //        linkSummary.Text = "Show test summary...";

        //        while (panelResults.Height >= 68)
        //        {
        //            panelResults.Height -= 4;
        //            Application.DoEvents();
        //        }
        //    }
        //    else
        //    {
        //        linkSummary.Text = "Hide test summary...";

        //        while (panelResults.Height <= 149)
        //        {
        //            panelResults.Height += 4;
        //            Application.DoEvents();
        //        }
        //    }
        //}

        private void tlbStart_Click(object sender, EventArgs e)
        {
            testProgressStatusBar.Clear();
            testProgressStatusBar.Total = 50;
            for (int i = 0; i < testProgressStatusBar.Total; i++)
            {
                if (i == 12 || i == 20 || i == 28 || i == 29 || i == 38 || i == 40 || i == 45 || i == 46 || i == 47 || i == 18 || i == 25)
                    testProgressStatusBar.Failed += 1;
                else if (i == 30 || i == 42)
                    testProgressStatusBar.Ignored += 1;
                else
                    testProgressStatusBar.Passed += 1;

                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();

            if (aboutForm != null)
                aboutForm.Dispose();
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        private void reloadToolbarButton_Click(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(5, "MbUnit Test Notice", "Recent changes have caused 5 of your unit tests to fail.", ToolTipIcon.Error);
        }
    }
}