using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gallio.Common.Splash.Demo
{
    public partial class DemoForm : Form
    {
        public DemoForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            splashView1.AppendText("Text...\nText 2");
            splashView1.AppendText("...");
            splashView1.AppendText("\nMore \tText...\n");
            splashView1.AppendText("Why Hello مرحبا العالمي World?  How are you?");
            splashView1.AppendLine();
            splashView1.AppendText("Tab1\tTab2\tTab3\tTab4\tTab5\tTab6\tTab7\tTab8\n");
            splashView1.AppendText("Tab.1\tTab.2\tTab.3\tTab.4\tTab.5\tTab.6\tTab.7\tTab.8\n");
        }
    }
}
