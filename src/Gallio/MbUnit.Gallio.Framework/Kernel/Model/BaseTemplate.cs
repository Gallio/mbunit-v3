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
using MbUnit.Framework.Kernel.DataBinding;
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
        private readonly List<ITemplate> children;
        private readonly List<ITemplateParameter> parameters;
        private ITemplate parent;

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
            parameters = new List<ITemplateParameter>();

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
        public IList<ITemplateParameter> Parameters
        {
            get { return parameters; }
        }

        /// <inheritdoc />
        public virtual ITemplateBinding Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            return new BaseTemplateBinding(this, scope, arguments);
        }

        /// <inheritdoc />
        public virtual void AddChild(ITemplate template)
        {
            ModelUtils.Link<ITemplate>(this, template);
        }

        /// <summary>
        /// Gets the parameter with the specified name, or null if none.
        /// </summary>
        /// <param name="parameterName">The parameter name</param>
        /// <returns>The parameter</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameterName"/> is null</exception>
        public ITemplateParameter GetParameterByName(string parameterName)
        {
            if (parameterName == null)
                throw new ArgumentNullException("parameterName");

            return parameters.Find(delegate(ITemplateParameter parameter)
            {
                return parameter.Name == parameterName;
            });
        }

        /// <summary>
        /// Adds a parameter to the list of parameters.
        /// </summary>
        /// <param name="parameter">The parameter to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a parameter with the same name already exists</exception>
        public void AddParameter(ITemplateParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (GetParameterByName(parameter.Name) != null)
                throw new InvalidOperationException("The parameter list already contains a parameter named: " + parameter.Name);

            parameters.Add(parameter);
        }
    }
}
