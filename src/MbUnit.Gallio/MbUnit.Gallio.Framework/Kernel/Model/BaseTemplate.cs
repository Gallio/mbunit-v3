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
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITemplate" />.
    /// </summary>
    /// <remarks>
    /// The base template implementation acts as a simple container for templates.
    /// Accordingly its kind is set to <see cref="ComponentKind.Group" /> by default.
    /// </remarks>
    public class BaseTemplate : BaseTemplateComponent, ITemplate
    {
        private ITemplate parent;
        private List<ITemplate> children;
        private List<ITemplateParameterSet> parameterSets;

        /// <summary>
        /// Initializes a template initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public BaseTemplate(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            children = new List<ITemplate>();
            parameterSets = new List<ITemplateParameterSet>();

            Kind = ComponentKind.Group;
        }

        /// <inheritdoc />
        public ITemplate Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <inheritdoc />
        public virtual IList<ITemplate> Children
        {
            get { return children; }
        }

        /// <inheritdoc />
        public IList<ITemplateParameterSet> ParameterSets
        {
            get { return parameterSets; }
        }

        /// <inheritdoc />
        public virtual ITemplateBinding Bind(TestScope scope, IDictionary<ITemplateParameter, object> arguments)
        {
            return new BaseTemplateBinding(this, scope, arguments);
        }

        /// <inheritdoc />
        public virtual void AddChild(ITemplate template)
        {
            ModelUtils.Link<ITemplate>(this, template);
        }

        /// <summary>
        /// Gets the parameter set with the specified name, or null if none.
        /// Always returns null if the parameter set name is empty (anonymous).
        /// </summary>
        /// <param name="parameterSetName">The parameter set name</param>
        /// <returns>The parameter set</returns>
        public ITemplateParameterSet GetParameterSetByName(string parameterSetName)
        {
            if (parameterSetName.Length != 0)
            {
                foreach (ITemplateParameterSet parameterSet in parameterSets)
                    if (parameterSet.Name == parameterSetName)
                        return parameterSet;
            }

            return null;
        }
    }
}
