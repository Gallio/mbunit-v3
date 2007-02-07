using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

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

            TreeNode project = new TreeNode("Test Project 1.0", 0, 0);
            projectTree.Nodes.Add(project);

            TreeNode namespaces = new TreeNode("Namespaces", 0, 0);
            project.Nodes.Add(namespaces);

            TreeNode ns = new TreeNode("TestNamespace", 1, 1);
            namespaces.Nodes.Add(ns);

            TreeNode cl = new TreeNode("Class1", 2, 2);
            ns.Nodes.Add(cl);

            TreeNode m1 = new TreeNode("TestMethod", 3, 3);
            m1.ForeColor = Color.Green;
            cl.Nodes.Add(m1);

            TreeNode m2 = new TreeNode("AnotherMethod", 3, 3);
            m2.ForeColor = Color.Red;
            cl.Nodes.Add(m2);

            TreeNode cl2 = new TreeNode("Class2", 2, 2);
            ns.Nodes.Add(cl2);

            TreeNode m3 = new TreeNode("MethodThatWorks", 3, 3);
            m3.ForeColor = Color.Green;
            cl2.Nodes.Add(m3);

            TreeNode m4 = new TreeNode("DoesntWork", 3, 3);
            m4.ForeColor = Color.Red;
            cl2.Nodes.Add(m4);

            TreeNode m5 = new TreeNode("DoGetProgress", 3, 3);
            m5.ForeColor = Color.Green;
            cl2.Nodes.Add(m5);

            TreeNode m6 = new TreeNode("BuildTree", 3, 3);
            m6.ForeColor = Color.Green;
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
    }
}