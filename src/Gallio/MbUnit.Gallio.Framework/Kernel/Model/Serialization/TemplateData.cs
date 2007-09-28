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
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Model.Serialization
{
    /// <summary>
    /// Describes a template in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplate"/>
    [Serializable]
    [XmlRoot("template", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TemplateData : TemplateComponentData
    {
        private readonly List<TemplateData> children;
        private readonly List<TemplateParameterData> parameters;
        private bool isGenerator;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TemplateData()
        {
            children = new List<TemplateData>();
            parameters = new List<TemplateParameterData>();
        }

        /// <summary>
        /// Creates a template info.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TemplateData(string id, string name)
            : base(id, name)
        {
            children = new List<TemplateData>();
            parameters = new List<TemplateParameterData>();
        }

        /// <summary>
        /// Copies the contents of a tmplate.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateData(ITemplate source)
            : base(source)
        {
            isGenerator = source.IsGenerator;
            children = new List<TemplateData>(source.Children.Count);
            parameters = new List<TemplateParameterData>(source.Parameters.Count);

            GenericUtils.ConvertAndAddAll(source.Children, children, delegate(ITemplate child)
            {
                return new TemplateData(child);
            });

            GenericUtils.ConvertAndAddAll(source.Parameters, parameters, delegate(ITemplateParameter parameter)
            {
                return new TemplateParameterData(parameter);
            });
        }

        /// <summary>
        /// Gets the mutable list of children.
        /// </summary>
        /// <seealso cref="IModelTreeNode{T}.Children"/>
        [XmlArray("children", IsNullable=false)]
        [XmlArrayItem("template", IsNullable=false)]
        public List<TemplateData> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets the mutable list of parameters.
        /// </summary>
        /// <seealso cref="ITemplate.Parameters"/>
        [XmlArray("parameters", IsNullable = false)]
        [XmlArrayItem("parameter", IsNullable = false)]
        public List<TemplateParameterData> Parameters
        {
            get { return parameters; }
        }

        /// <inheritdoc />
        [XmlAttribute("isGenerator")]
        public bool IsGenerator
        {
            get { return isGenerator; }
            set { isGenerator = value; }
        }
    }
}