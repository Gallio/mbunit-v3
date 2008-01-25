using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    public partial class LogWindow : DockContent
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        public LogWindow(string text) : this()
        {
            Text = text;
        }

        public string LogBody
        {
            get { return logBody.Text; }
            set { logBody.Text = value; }
        }

        private void clearAllToolStripButton_Click(object sender, EventArgs e)
        {
            logBody.Clear();
        }
    }
}