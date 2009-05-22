using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    public class TestExplorerPaneProvider : IPreferencePaneProvider
    {
        private readonly IOptionsController optionsController;

        public TestExplorerPaneProvider(IOptionsController optionsController)
        {
            this.optionsController = optionsController;
        }

        public PreferencePane CreatePreferencePane()
        {
            return new TestExplorerPane(optionsController);
        }
    }
}
