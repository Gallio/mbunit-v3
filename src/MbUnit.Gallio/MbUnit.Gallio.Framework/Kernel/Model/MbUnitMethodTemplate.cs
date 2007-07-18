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
using System.Reflection;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit test method.
    /// </summary>
    public class MbUnitMethodTemplate : MbUnitTemplate
    {
        private MbUnitFixtureTemplate fixtureTemplate;
        private MethodInfo method;

        /// <summary>
        /// Initializes an MbUnit test method template model object.
        /// </summary>
        /// <param name="fixtureTemplate">The containing fixture template</param>
        /// <param name="method">The test method</param>
        public MbUnitMethodTemplate(MbUnitFixtureTemplate fixtureTemplate, MethodInfo method)
            : base(method.Name, CodeReference.CreateFromMember(method))
        {
            this.fixtureTemplate = fixtureTemplate;
            this.method = method;

            Kind = TemplateKind.Test;
        }

        /// <summary>
        /// Gets the containing fixture template.
        /// </summary>
        public MbUnitFixtureTemplate FixtureTemplate
        {
            get { return fixtureTemplate; }
        }

        /// <summary>
        /// Gets the test method.
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }

        /// <summary>
        /// Adds a test method parameter.
        /// </summary>
        /// <param name="parameter">The parameter to add</param>
        public void AddParameter(MbUnitTemplateParameter parameter)
        {
        }
    }
}
