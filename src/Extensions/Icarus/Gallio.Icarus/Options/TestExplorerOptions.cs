using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class TestExplorerOptions : OptionsPanel
    {
        private readonly IOptionsController optionsController;

        public TestExplorerOptions(IOptionsController optionsController)
        {
            this.optionsController = optionsController;

            InitializeComponent();

            selectedTreeViewCategories.DataSource = optionsController.SelectedTreeViewCategories;
            unselectedTreeViewCategories.DataSource = optionsController.UnselectedTreeViewCategories;

            alwaysReloadAssemblies.DataBindings.Add("Checked", optionsController, "AlwaysReloadAssemblies");
        }

        private void addButton_Click(object sender, System.EventArgs e)
        {
            object category = unselectedTreeViewCategories.SelectedItem;
            optionsController.SelectedTreeViewCategories.Add((string) category);
            optionsController.UnselectedTreeViewCategories.Remove((string) category);
        }

        private void removeButton_Click(object sender, System.EventArgs e)
        {
            object category = selectedTreeViewCategories.SelectedItem;
            optionsController.UnselectedTreeViewCategories.Add((string) category);
            optionsController.SelectedTreeViewCategories.Remove((string) category);
        }
    }
}
