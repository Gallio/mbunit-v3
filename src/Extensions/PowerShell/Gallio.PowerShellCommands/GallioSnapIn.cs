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
using System.Management.Automation;

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    /// <summary>
    /// A PowerShell SnapIn that registers the Gallio Cmdlets.
    /// </summary>
    [RunInstaller(true)]
    public class GallioSnapIn : PSSnapIn
    {
        /// <summary>
        /// Gets the description of the snap-in.
        /// </summary>
        public override string Description
        {
            get { return "Gallio Commands."; }
        }

        /// <summary>
        /// Gets the name of the snap-in.
        /// </summary>
        public override string Name
        {
            get { return "Gallio"; }
        }

        /// <summary>
        /// Gets the vendor of the snap-in.
        /// </summary>
        public override string Vendor
        {
            get { return "Gallio"; }
        }
    }
}
