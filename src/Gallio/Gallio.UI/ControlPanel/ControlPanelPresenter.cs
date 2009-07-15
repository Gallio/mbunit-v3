// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Gallio.Common;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;
using Gallio.UI.ErrorReporting;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Presents the control panel dialog.
    /// </summary>
    public class ControlPanelPresenter : IControlPanelPresenter
    {
        private readonly ComponentHandle<IControlPanelTabProvider, ControlPanelTabProviderTraits>[] controlPanelTabProviderHandles;
        private readonly IElevationManager elevationManager;

        /// <summary>
        /// Creates a control panel presenter.
        /// </summary>
        /// <param name="controlPanelTabProviderHandles">The preference page provider handles, not null</param>
        /// <param name="elevationManager">The elevation manager, not null</param>
        public ControlPanelPresenter(ComponentHandle<IControlPanelTabProvider, ControlPanelTabProviderTraits>[] controlPanelTabProviderHandles,
            IElevationManager elevationManager)
        {
            this.controlPanelTabProviderHandles = controlPanelTabProviderHandles;
            this.elevationManager = elevationManager;
        }

        /// <inheritdoc />
        public DialogResult Show(IWin32Window owner)
        {
            var dialog = CreateControlPanelDialog();

            if (Application.MessageLoop)
            {
                var dr = dialog.ShowDialog(owner);
                return dr;
            }
            else
            {
                ErrorDialogUnhandledExceptionHandler.RunApplicationWithHandler(dialog);
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

            controlPanelDialog.ElevationManager = elevationManager;
            controlPanelDialog.ProgressMonitorProvider = NullProgressMonitorProvider.Instance; // FIXME: Use a proper dialog.
            return controlPanelDialog;
        }

        private static Func<ControlPanelTab> GetControlPanelTabFactory(ComponentHandle<IControlPanelTabProvider, ControlPanelTabProviderTraits> controlPanelTabProviderHandle)
        {
            return () => controlPanelTabProviderHandle.GetComponent().CreateControlPanelTab();
        }
    }
}
