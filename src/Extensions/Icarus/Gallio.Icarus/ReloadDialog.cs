using System.Windows.Forms;

namespace Gallio.Icarus
{
    public partial class ReloadDialog : Form
    {
        public bool AlwaysReloadTests
        {
            get { return alwaysReload.Checked; }
        }

        public ReloadDialog(string assembly)
        {
            InitializeComponent();
                                                 
            assemblyLabel.Text = string.Format("The assembly {0} has been modified.", assembly);
        }
    }
}
