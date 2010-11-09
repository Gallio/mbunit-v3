using System;
using System.Windows.Forms;
using Gallio.Common.Concurrency;

namespace Gallio.Icarus.ExtensionSample
{
    public partial class View : UserControl
    {
        private readonly IController controller;
        private readonly EventHandler<UpdateEventArgs> update;

        public View(IController controller)
        {
            this.controller = controller;

            InitializeComponent();

            update = (s, e) => AppendText(e.Text);
            controller.Update += update;
        }

        private void AppendText(string text)
        {
            Sync.Invoke(this, () => richTextBox1.AppendText(text + Environment.NewLine));
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            controller.Update -= update;
        }
    }
}
