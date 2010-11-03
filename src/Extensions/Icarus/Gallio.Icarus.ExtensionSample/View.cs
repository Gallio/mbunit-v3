using System;
using System.Windows.Forms;
using Gallio.Common.Concurrency;

namespace Gallio.Icarus.ExtensionSample
{
    public partial class View : UserControl
    {
        public View(IController controller)
        {
            InitializeComponent();

            controller.Update += (s, e) => AppendText(e.Text);
        }

        private void AppendText(string text)
        {
            Sync.Invoke(this, () => richTextBox1.AppendText(text + Environment.NewLine));
        }
    }
}
