using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class AssembliesOptions : OptionsPanel
    {
        public AssembliesOptions(IOptionsController optionsController)
        {
            InitializeComponent();

            alwaysReloadAssemblies.DataBindings.Add("Checked", optionsController, "AlwaysReloadAssemblies");
        }
    }
}
