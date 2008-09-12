// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.VisualStudio.Shell.Actions
{
    /// <summary>
    /// Represents an action that is associated with a button on a Visual Studio command bar.
    /// </summary>
    public abstract class Action
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="button">The button that was clicked to invoke the action</param>
        public abstract void Execute(ActionButton button);

        /// <summary>
        /// Provides an opportunity for the action to update the status, text and other
        /// properties of a button.
        /// </summary>
        /// <param name="button">The button to update</param>
        public virtual void Update(ActionButton button)
        {
        }
    }
}
