using System.Windows.Forms;

namespace Gallio.Icarus.Search
{
    public partial class View : UserControl
    {
        private readonly IController controller;

        public View(IController controller)
        {
            this.controller = controller;

            InitializeComponent();
        }

        private void searchTextBox_TextChanged(object sender, System.EventArgs e)
        {
            controller.Search(searchTextBox.Text);
        }
    }
}
