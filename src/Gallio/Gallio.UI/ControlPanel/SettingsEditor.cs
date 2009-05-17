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

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Base class for user controls for editing settings with deferred application.
    /// </summary>
    public class SettingsEditor : UserControl
    {
        /// <summary>
        /// Event raised when the settings have changed.
        /// </summary>
        public event EventHandler SettingsChanged;

        /// <summary>
        /// Gets or sets whether the settings have changed.
        /// </summary>
        public bool HasSettingsChanges { get; protected set; }

        /// <summary>
        /// Applies settings changes.
        /// </summary>
        /// <remarks>
        /// The default implementation simply sets <see cref="HasSettingsChanges" /> to false.
        /// </remarks>
        public virtual void ApplySettingsChanges()
        {
            HasSettingsChanges = false;
        }

        /// <summary>
        /// Sets <see cref="HasSettingsChanges" /> to true and raises the <see cref="SettingsChanged" /> event
        /// on the first change.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnSettingsChanged(EventArgs e)
        {
            if (!HasSettingsChanges)
            {
                HasSettingsChanges = true;

                if (SettingsChanged != null)
                    SettingsChanged(this, e);
            }
        }
    }
}
