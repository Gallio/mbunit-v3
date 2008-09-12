using System.Collections.Generic;

namespace Gallio.VisualStudio.Shell.Actions
{
    /// <summary>
    /// Provides mechanisms for installing and removing action buttons.
    /// </summary>
    public interface IActionManager
    {
        /// <summary>
        /// Installs an action button.
        /// </summary>
        /// <param name="descriptor">The action button descriptor</param>
        void InstallActionButton(ActionButtonDescriptor descriptor);
    }
}