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
using MbUnit.Framework.Model.Metadata;
using MbUnit.Framework.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// The template that is the ancestor of all templates declared
    /// by the MbUnit test framework.
    /// </summary>
    public class MbUnitTestFrameworkTemplate : MbUnitTestTemplate
    {
        private Version version;
        private List<MbUnitTestAssemblyTemplate> assemblyTemplates;

        /// <summary>
        /// Initializes the MbUnit framework template model object.
        /// </summary>
        /// <param name="version">The MbUnit framework version</param>
        public MbUnitTestFrameworkTemplate(Version version)
            : base("MbUnit Gallio v" + version, CodeReference.Unknown)
        {
            this.version = version;

            assemblyTemplates = new List<MbUnitTestAssemblyTemplate>();
            Kind = TemplateKind.Framework;
        }

        /// <inheritdoc />
        public override IEnumerable<ITestTemplate> Children
        {
            get
            {
                foreach (MbUnitTestAssemblyTemplate assemblyTemplate in assemblyTemplates)
                    yield return assemblyTemplate;
            }
        }

        /// <summary>
        /// Gets the MbUnit framework version.
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>
        /// Gets the list of assembly templates.
        /// </summary>
        /// <returns>The assembly templates</returns>
        public IList<MbUnitTestAssemblyTemplate> AssemblyTemplates
        {
            get { return assemblyTemplates; }
        }

        /// <summary>
        /// Adds an assembly template to the framework.
        /// </summary>
        /// <param name="assemblyTemplate">The assembly template</param>
        public void AddAssemblyTemplate(MbUnitTestAssemblyTemplate assemblyTemplate)
        {
            ModelUtils.LinkTemplate(this, assemblyTemplates, assemblyTemplate);
        }
    }
}
