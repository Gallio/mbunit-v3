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
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit test fixture.
    /// </summary>
    public class MbUnitFixtureTemplate : MbUnitTemplate
    {
        private readonly MbUnitAssemblyTemplate assemblyTemplate;
        private readonly Type fixtureType;

        /// <summary>
        /// Initializes an MbUnit test fixture template model object.
        /// </summary>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="fixtureType">The test fixture type</param>
        public MbUnitFixtureTemplate(MbUnitAssemblyTemplate assemblyTemplate, Type fixtureType)
            : base(fixtureType.Name, CodeReference.CreateFromType(fixtureType))
        {
            this.assemblyTemplate = assemblyTemplate;
            this.fixtureType = fixtureType;

            Kind = ComponentKind.Fixture;
        }

        /// <summary>
        /// Gets the containing assembly template.
        /// </summary>
        public MbUnitAssemblyTemplate AssemblyTemplate
        {
            get { return assemblyTemplate; }
        }

        /// <summary>
        /// Gets the test fixture type.
        /// </summary>
        public Type FixtureType
        {
            get { return fixtureType; }
        }

        /// <summary>
        /// Gets the list of method templates.
        /// </summary>
        public IList<MbUnitMethodTemplate> MethodTemplates
        {
            get { return ModelUtils.FilterChildrenByType<ITemplate, MbUnitMethodTemplate>(this); }
        }

        /// <summary>
        /// Adds a test method template as a child of the fixture.
        /// </summary>
        /// <param name="methodTemplate">The test method model</param>
        public void AddMethodTemplate(MbUnitMethodTemplate methodTemplate)
        {
            AddChild(methodTemplate);
        }
    }
}
