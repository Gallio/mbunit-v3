using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    public class TestStatusPaneProvider : IPreferencePaneProvider
    {
        private readonly IOptionsController optionsController;

        public TestStatusPaneProvider(IOptionsController optionsController)
        {
            this.optionsController = optionsController;
        }

        public PreferencePane CreatePreferencePane()
        {
            return new TestStatusPane(optionsController);
        }
    }
}
