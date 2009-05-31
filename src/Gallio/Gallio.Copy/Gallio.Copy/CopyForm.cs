using System.Collections.Generic;
using System.Windows.Forms;
using Gallio.Common.Policies;

namespace Gallio.Copy
{
    internal partial class CopyForm : Form
    {
        private readonly ICopyController copyController;

        public CopyForm(ICopyController copyController)
        {
            this.copyController = copyController;

            InitializeComponent();

            foreach (var plugin in copyController.Plugins)
                pluginsListView.Items.Add(plugin);

            UnhandledExceptionPolicy.ReportUnhandledException += (sender, e) => MessageBox.Show(this, 
                e.GetDescription(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void closeButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void copyButton_Click(object sender, System.EventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var list = new List<string>();
                foreach (ListViewItem item in pluginsListView.CheckedItems)
                    list.Add(item.Text);

                copyController.CopyTo(folderBrowserDialog.SelectedPath, list);
            }
        }
    }
}
