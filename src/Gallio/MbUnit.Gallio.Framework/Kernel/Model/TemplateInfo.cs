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
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Describes a template in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplate"/>
    [Serializable]
    [XmlRoot("template", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TemplateInfo : TemplateComponentInfo
    {
        private List<TemplateInfo> children;
        private List<TemplateParameterSetInfo> parameterSets;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TemplateInfo()
        {
            children = new List<TemplateInfo>();
            parameterSets = new List<TemplateParameterSetInfo>();
        }

        /// <summary>
        /// Creates a template info.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TemplateInfo(string id, string name)
            : base(id, name)
        {
            children = new List<TemplateInfo>();
            parameterSets = new List<TemplateParameterSetInfo>();
        }

        /// <summary>
        /// Copies the contents of a tmplate.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TemplateInfo(ITemplate obj)
            : base(obj)
        {
            children = new List<TemplateInfo>(obj.Children.Count);
            parameterSets = new List<TemplateParameterSetInfo>(obj.ParameterSets.Count);

            ListUtils.ConvertAndAddAll(obj.Children, children, delegate(ITemplate child)
            {
                return new TemplateInfo(child);
            });

            ListUtils.ConvertAndAddAll(obj.ParameterSets, parameterSets, delegate(ITemplateParameterSet parameterSet)
            {
                return new TemplateParameterSetInfo(parameterSet);
            });
        }

        /// <summary>
        /// Gets the mutable list of children.
        /// </summary>
        /// <seealso cref="ITemplate.Children"/>
        [XmlArray("children", IsNullable=false)]
        [XmlArrayItem("template", IsNullable=false)]
        public List<TemplateInfo> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets the mutable list of parameter sets.
        /// </summary>
        /// <seealso cref="ITemplate.ParameterSets"/>
        [XmlArray("parameterSets", IsNullable = false)]
        [XmlArrayItem("parameterSet", IsNullable = false)]
        public List<TemplateParameterSetInfo> ParameterSets
        {
            get { return parameterSets; }
        }
    }
}