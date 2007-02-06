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
            this.Text = String.Format(this.Text, appVersion.Major, appVersion.Minor, appVersion.Build);

        }

        private void Form_Load(object sender, EventArgs e)
        {
            ddlTreeFilter.SelectedIndex = 1;

            TreeNode project = new TreeNode("Test Project 1.0", 0, 0);
            treeFunctions.Nodes.Add(project);

            TreeNode namespaces = new TreeNode("Namespaces", 0, 0);
            project.Nodes.Add(namespaces);

            TreeNode ns = new TreeNode("TestNamespace", 1, 1);
            namespaces.Nodes.Add(ns);

            TreeNode cl = new TreeNode("Class1", 2, 2);
            ns.Nodes.Add(cl);

            TreeNode m1 = new TreeNode("TestMethod", 3, 3);
            m1.Tag = Color.Green;
            cl.Nodes.Add(m1);

            TreeNode m2 = new TreeNode("AnotherMethod", 3, 3);
            m2.Tag = Color.Red;
            cl.Nodes.Add(m2);

            TreeNode cl2 = new TreeNode("Class2", 2, 2);
            ns.Nodes.Add(cl2);

            TreeNode m3 = new TreeNode("MethodThatWorks", 3, 3);
            m3.Tag = Color.Green;
            cl2.Nodes.Add(m3);

            TreeNode m4 = new TreeNode("DoesntWork", 3, 3);
            m4.Tag = Color.Red;
            cl2.Nodes.Add(m4);

            TreeNode m5 = new TreeNode("DoGetProgress", 3, 3);
            m5.Tag = Color.Green;
            cl2.Nodes.Add(m5);

            TreeNode m6 = new TreeNode("BuildTree", 3, 3);
            m6.Tag = Color.Green;
            cl2.Nodes.Add(m6);

            project.ExpandAll();

        }

        private void fileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void linkSummary_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (panelResults.Height >= 149)
            {
                linkSummary.Text = "Show test summary...";

                while (panelResults.Height >= 68)
                {
                    panelResults.Height -= 4;
                    Application.DoEvents();
                }
            }
            else
            {
                linkSummary.Text = "Hide test summary...";

                while (panelResults.Height <= 149)
                {
                    panelResults.Height += 4;
                    Application.DoEvents();
                }
            }
        }

        private void tlbStart_Click(object sender, EventArgs e)
        {
            testStatusBar1.Clear();
            testStatusBar1.Total = 50;
            for (int i = 0; i < testStatusBar1.Total; i++)
            {
                if (i == 12 || i == 20 || i == 28 || i == 29 || i == 38 || i == 40 || i == 45 || i == 46 || i == 47 || i == 18 || i == 25)
                    testStatusBar1.Failed += 1;
                else if (i == 30 || i == 42)
                    testStatusBar1.Ignored += 1;
                else
                    testStatusBar1.Passed += 1;

                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}