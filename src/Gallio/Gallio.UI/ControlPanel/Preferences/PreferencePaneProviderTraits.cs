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
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Describes the traits of a <see cref="IPreferencePaneProvider" />.
    /// </summary>
    public class PreferencePaneProviderTraits : Traits
    {
        private readonly string path;

        /// <summary>
        /// Creates a traits object for a preference pane provider.
        /// </summary>
        /// <param name="path">The preference pane path consisting of slash-delimited name segments
        /// specifying tree nodes</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is empty</exception>
        public PreferencePaneProviderTraits(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("The preference pane path must not be empty.", "path");

            this.path = path;
            Scope = PreferencePaneScope.User;
        }

        /// <summary>
        /// Gets the preference pane path consisting of slash-delimited name segments
        /// specifying tree nodes.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Gets or sets an icon (16x16) for the preference pane, or null if none.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets an integer index used to sort control panel tabs in ascending order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets a value that describes the scope of the changes effected by a preference pane.
        /// </summary>
        /// <value>The scope.  <see cref="PreferencePaneScope.User" /> by default.</value>
        public PreferencePaneScope Scope { get; set; }
    }
}
