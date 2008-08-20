using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class AppearanceOptions : OptionsPanel
    {
        public AppearanceOptions(IOptionsController optionsController)
        {
            InitializeComponent();

            testProgressBarStyle.DataBindings.Add("Text", optionsController, "TestProgressBarStyle");
            showProgressDialogs.DataBindings.Add("Checked", optionsController, "ShowProgressDialogs");
        }
    }
}
