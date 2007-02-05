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
            Version appVersion = System.Reflection.Assembly.GetCallingAssembly().GetName().Version;
            this.Text = String.Format(this.Text, appVersion.Major, appVersion.Minor, appVersion.Build);

        }

        private void Form_Load(object sender, EventArgs e)
        {
            ddlTreeFilter.SelectedIndex = 1;
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

  
    }
}