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
    public class BaseTemplate : BaseTemplateComponent, ITemplate
    {
        private ITemplate parent;
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
            this.parameterSets = new List<ITemplateParameterSet>();
            Kind = TemplateKind.Custom;
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="MetadataConstants.TemplateKindKey" />
        /// metadata entry.  (This is a convenience method.)
        /// </summary>
        /// <value>
        /// One of the <see cref="TemplateKind" /> constants.
        /// </value>
        public string Kind
        {
            get { return (string) Metadata.GetValue(MetadataConstants.TemplateKindKey); }
            set { Metadata.SetValue(MetadataConstants.TemplateKindKey, value); }
        }

        /// <inheritdoc />
        public ITemplate Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <inheritdoc />
        public virtual IEnumerable<ITemplate> Children
        {
            get { yield break; }
        }

        /// <inheritdoc />
        public IList<ITemplateParameterSet> ParameterSets
        {
            get { return parameterSets; }
        }

        /// <inheritdoc />
        public virtual void AddChild(ITemplate template)
        {
            throw new NotSupportedException("This template does not support the addition of arbitrary children.");
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
