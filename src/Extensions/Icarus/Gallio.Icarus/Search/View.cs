using System.Windows.Forms;

namespace Gallio.Icarus.Search
{
    public partial class View : UserControl
    {
        private readonly IController controller;
        private readonly IModel model;

        public View(IController controller, IModel model)
        {
            this.controller = controller;
            this.model = model;

            InitializeComponent();

            SetMetadataOptions();
            model.Metadata.PropertyChanged += (s, e) => SetMetadataOptions();
        }

        private void SetMetadataOptions()
        {
            metadataComboBox.Items.Clear();
            foreach (var metadata in model.Metadata.Value)
            {
                metadataComboBox.Items.Add(metadata);
            }

            if (metadataComboBox.Items.Count > 0)
                metadataComboBox.SelectedIndex = 0;
        }

        private void searchTextBox_TextChanged(object sender, System.EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            controller.Search(searchTextBox.Text, (string)metadataComboBox.SelectedItem);
        }

        private void metadataComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Search();
        }
    }
}
