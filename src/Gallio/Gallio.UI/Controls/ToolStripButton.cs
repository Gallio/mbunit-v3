// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System.ComponentModel;
using System.Windows.Forms;

namespace Gallio.UI.Controls
{
    /// <summary>
    /// Sub-class of <see cref="System.Windows.Forms.ToolStripButton">ToolStripButton</see>, 
    /// making databinding easier.
    /// </summary>
    public class ToolStripButton : System.Windows.Forms.ToolStripButton, IBindableComponent
    {
        private BindingContext bindingContext;
        private ControlBindingsCollection dataBindings;

        /// <summary>
        /// Gets or sets the collection of currency managers for the IBindableComponent.
        /// </summary>
        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                    bindingContext = new BindingContext();
                return bindingContext;
            }
            set { bindingContext = value; }
        }

        /// <summary>
        /// Gets the collection of data-binding objects for this IBindableComponent.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (dataBindings == null)
                    dataBindings = new ControlBindingsCollection(this);
                return dataBindings;
            }
        }
    }
}