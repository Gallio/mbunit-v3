using Gallio.Icarus.Controllers.Interfaces;
using Gallio.UI.ControlPanel.Preferences;

namespace Gallio.Icarus.ControlPanel
{
    public class TreeViewCategoryPaneProvider : IPreferencePaneProvider
    {
        private readonly IOptionsController optionsController;

        public TreeViewCategoryPaneProvider(IOptionsController optionsController)
        {
            this.optionsController = optionsController;
        }

        public PreferencePane CreatePreferencePane()
        {
            return new TreeViewCategoryPane(optionsController);
        }
    }
}
