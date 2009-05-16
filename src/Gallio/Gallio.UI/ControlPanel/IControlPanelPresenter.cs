using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Presents the control panel dialog.
    /// </summary>
    public interface IControlPanelPresenter
    {
        /// <summary>
        /// Shows the control panel dialog.
        /// </summary>
        /// <param name="owner">The dialog owner control</param>
        /// <returns>The dialog result, either <see cref="DialogResult.OK" />
        /// or <see cref="DialogResult.Cancel"/> depending on how the dialog
        /// was closed</returns>
        DialogResult Show(IWin32Window owner);
    }
}
