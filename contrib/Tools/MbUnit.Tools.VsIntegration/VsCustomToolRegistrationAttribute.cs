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

using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Tools.VsIntegration
{
    /// <summary>
    /// Specifies registration information for a <see cref="VsCustomTool" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class VsCustomToolRegistrationAttribute : Attribute
    {
        private readonly string name;
        private readonly string description;
        private readonly string[] vsCategoryGuids;
        private readonly string[] vsVersions;

        /// <summary>
        /// Specifies registration information for a <see cref="VsCustomTool" />.
        /// </summary>
        /// <param name="name">The name of the custom tool as specified in the Visual Studio "Custom Tool" property</param>
        /// <param name="description">The custom tool description</param>
        /// <param name="vsCategoryGuids">The array of Visual Studio category guids to support</param>
        /// <param name="vsVersions">The array of Visual Studio versions to support, such as "8.0"</param>
        /// <seealso cref="VsCategoryGuid"/>
        public VsCustomToolRegistrationAttribute(string name, string description, string[] vsCategoryGuids, string[] vsVersions)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");
            if (description == null)
                throw new ArgumentNullException(@"description");
            if (vsCategoryGuids == null || Array.IndexOf(vsCategoryGuids, null) != 0)
                throw new ArgumentNullException(@"vsCategoryGuids");
            if (vsVersions == null || Array.IndexOf(vsVersions, null) != 0)
                throw new ArgumentNullException(@"vsVersions");

            this.name = name;
            this.description = description;
            this.vsCategoryGuids = vsCategoryGuids;
            this.vsVersions = vsVersions;
        }

        /// <summary>
        /// Gets the custom tool name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the custom tool description.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets the array of Visual Studio category guids to support.
        /// </summary>
        /// <seealso cref="VsCategoryGuid"/>
        public string[] VsCategoryGuids
        {
            get { return vsCategoryGuids; }
        }

        /// <summary>
        /// Gets the array of Visual Studio versions to support.
        /// </summary>
        public string[] VsVersions
        {
            get { return vsVersions; }
        }
    }
}
