using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace MbUnit.Icarus
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            // Set the application version.
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            versionLabel.Text = String.Format(versionLabel.Text, appVersion.Major, appVersion.Minor, appVersion.Build);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}