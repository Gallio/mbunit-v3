using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    public class StartupPaneProvider : IPreferencePaneProvider
    {
        private readonly IOptionsController optionsController;

        public StartupPaneProvider(IOptionsController optionsController)
        {
            this.optionsController = optionsController;
        }

        public PreferencePane CreatePreferencePane()
        {
            return new StartupPane(optionsController);
        }
    }
}
