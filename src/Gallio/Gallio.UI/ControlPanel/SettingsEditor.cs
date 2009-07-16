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
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Runtime.Security;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Base class for user controls for editing settings with deferred application.
    /// </summary>
    public class SettingsEditor : UserControl
    {
        private bool pendingSettingsChanges;
        private bool requiresElevation;

        /// <summary>
        /// Event raised when the value of <see cref="PendingSettingsChanges" /> changes.
        /// </summary>
        public event EventHandler PendingSettingsChangesChanged;

        /// <summary>
        /// Event raised when the value of <see cref="RequiresElevation" /> changes.
        /// </summary>
        public event EventHandler RequiresElevationChanged;

        /// <summary>
        /// Gets or sets whether there are pending settings changes yet to be applied.
        /// </summary>
        public bool PendingSettingsChanges
        {
            get { return pendingSettingsChanges; }
            set
            {
                if (pendingSettingsChanges != value)
                {
                    pendingSettingsChanges = value;
                    OnPendingSettingsChangesChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether elevation will be required to apply pending modifications.
        /// </summary>
        public bool RequiresElevation
        {
            get { return requiresElevation; }
            set
            {
                if (requiresElevation != value)
                {
                    requiresElevation = value;
                    OnRequiresElevationChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Applies pending settings changes.
        /// </summary>
        /// <param name="elevationContext">An elevation context when <see cref="RequiresElevation" />
        /// is true, otherwise null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        /// <remarks>
        /// The default implementation simply sets <see cref="PendingSettingsChanges" /> to false.
        /// </remarks>
        public virtual void ApplyPendingSettingsChanges(IElevationContext elevationContext, IProgressMonitor progressMonitor)
        {
            PendingSettingsChanges = false;
        }

        /// <summary>
        /// Raises the <see cref="PendingSettingsChanges" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnPendingSettingsChangesChanged(EventArgs e)
        {
            if (PendingSettingsChangesChanged != null)
                PendingSettingsChangesChanged(this, e);
        }

        /// <summary>
        /// Raises the <see cref="RequiresElevationChanged" /> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnRequiresElevationChanged(EventArgs e)
        {
            if (RequiresElevationChanged != null)
                RequiresElevationChanged(this, e);
        }
    }
}
