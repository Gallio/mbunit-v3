using Gallio.Icarus.Interfaces;

namespace Gallio.Icarus.Options
{
    internal partial class StartupOptions : OptionsPanel
    {
        public StartupOptions(IOptionsController optionsController)
        {
            InitializeComponent();

            restorePreviousSession.DataBindings.Add("Checked", optionsController, "RestorePreviousSettings");

            testRunnerFactories.Items.AddRange(optionsController.TestRunnerFactories);
            testRunnerFactories.DataBindings.Add("Text", optionsController, "TestRunnerFactory");
        }
    }
}
