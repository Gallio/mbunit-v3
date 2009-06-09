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
using System.Drawing;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Describes the traits of a <see cref="IPlugin" /> component.
    /// </summary>
    public class PluginTraits : Traits
    {
        /// <summary>
        /// Creates a plugin traits object.
        /// </summary>
        /// <param name="name">The localized display name of the plugin.</param>
        public PluginTraits(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
        }

        /// <summary>
        /// Gets the localized display name of the plugin.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the plugin version number, or null if none.
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// Gets or sets the plugin icon, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The image should be 32x32 pixels in size.
        /// </para>
        /// </remarks>
        public Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets the localized plugin description, or null if none.
        /// </summary>
        public string Description { get; set; }
    }
}
