// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    ///<summary>
    /// A textual progress monitor provides a little assistance with building
    /// primarily text-based progress monitors.
    ///</summary>
    public abstract class TextualProgressMonitor : TrackingProgressMonitor
    {
        /// <inheritdoc />
        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            base.OnBeginTask(taskName, totalWorkUnits);
            UpdateDisplay();
        }

        /// <inheritdoc />
        protected override void OnWorked(double workUnits)
        {
            base.OnWorked(workUnits);
            UpdateDisplay();
        }

        /// <inheritdoc />
        protected override void OnDone()
        {
            base.OnDone();
            UpdateDisplay();
        }

        /// <summary>
        /// Updates the display.
        /// </summary>
        protected abstract void UpdateDisplay();
    }
}