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
using Gallio.Model.Reflection;
using MbUnit.Model;
using Gallio.Model;
using MbUnit.Properties;

namespace MbUnit.Model
{
    /// <summary>
    /// The template that is the ancestor of all templates declared
    /// by the MbUnit test framework.
    /// </summary>
    public class MbUnitFrameworkTemplate : MbUnitTemplate
    {
        private readonly Version version;

        /// <summary>
        /// Initializes the MbUnit framework template model object.
        /// </summary>
        /// <param name="version">The MbUnit framework version</param>
        public MbUnitFrameworkTemplate(Version version)
            : base(String.Format(Resources.MbUnitFrameworkTemplate_MbUnitGallioFrameworkVersionFormat, version), null)
        {
            this.version = version;

            Kind = ComponentKind.Framework;
            IsGenerator = true;
        }

        /// <summary>
        /// Gets the MbUnit framework version.
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>
        /// Gets the list of assembly templates that are children of this template.
        /// </summary>
        public IList<MbUnitAssemblyTemplate> AssemblyTemplates
        {
            get { return ModelUtils.FilterChildrenByType<ITemplate, MbUnitAssemblyTemplate>(this); }
        }

        /// <summary>
        /// Adds an assembly template as a child of this template.
        /// </summary>
        /// <param name="assemblyTemplate">The assembly template</param>
        public void AddAssemblyTemplate(MbUnitAssemblyTemplate assemblyTemplate)
        {
            AddChild(assemblyTemplate);
        }
    }
}