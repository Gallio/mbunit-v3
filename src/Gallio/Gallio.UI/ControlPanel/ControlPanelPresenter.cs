using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Gallio.Common;
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Presents the control panel dialog.
    /// </summary>
    public class ControlPanelPresenter : IControlPanelPresenter
    {
        private readonly ComponentHandle<IControlPanelTabProvider, ControlPanelTabProviderTraits>[] controlPanelTabProviderHandles;

        /// <summary>
        /// Creates a control panel presenter.
        /// </summary>
        /// <param name="controlPanelTabProviderHandles">The preference page provider handles, not null</param>
        public ControlPanelPresenter(ComponentHandle<IControlPanelTabProvider, ControlPanelTabProviderTraits>[] controlPanelTabProviderHandles)
        {
            this.controlPanelTabProviderHandles = controlPanelTabProviderHandles;
        }

        /// <inheritdoc />
        public DialogResult Show(IWin32Window owner)
        {
            var dialog = CreateControlPanelDialog();

            if (Application.MessageLoop)
            {
                return dialog.ShowDialog(owner);
            }
            else
            {
                Application.Run(dialog);
                return dialog.DialogResult;
            }
        }

        private ControlPanelDialog CreateControlPanelDialog()
        {
            var controlPanelDialog = new ControlPanelDialog();

            Array.Sort(controlPanelTabProviderHandles, (x, y) => x.GetTraits().Order.CompareTo(y.GetTraits().Order));

            foreach (var controlPanelTabProviderHandle in controlPanelTabProviderHandles)
            {
                ControlPanelTabProviderTraits traits = controlPanelTabProviderHandle.GetTraits();

                controlPanelDialog.AddTab(traits.Name,
                    GetControlPanelTabFactory(controlPanelTabProviderHandle));
            }

            return controlPanelDialog;

        }

        private static Func<ControlPanelTab> GetControlPanelTabFactory(ComponentHandle<IControlPanelTabProvider, ControlPanelTabProviderTraits> controlPanelTabProviderHandle)
        {
            return () => controlPanelTabProviderHandle.GetComponent().CreateControlPanelTab();
        }
    }
}
