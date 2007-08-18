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
    /// Describes a template parameter set in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplateParameterSet"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public sealed class TemplateParameterSetInfo : TemplateComponentInfo
    {
        private List<TemplateParameterInfo> parameters;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TemplateParameterSetInfo()
        {
            parameters = new List<TemplateParameterInfo>();
        }

        /// <summary>
        /// Creates an empty parameter set info.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TemplateParameterSetInfo(string id, string name)
            : base(id, name)
        {
            parameters = new List<TemplateParameterInfo>();
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TemplateParameterSetInfo(ITemplateParameterSet obj)
            : base(obj)
        {
            parameters = new List<TemplateParameterInfo>(obj.Parameters.Count);

            ListUtils.ConvertAndAddAll(obj.Parameters, parameters,
            delegate(ITemplateParameter parameter)
            {
                return new TemplateParameterInfo(parameter);
            });
        }

        /// <summary>
        /// Gets the mutable list of parameters.
        /// </summary>
        /// <seealso cref="ITemplateParameterSet.Parameters"/>
        [XmlArray("parameters", IsNullable=false)]
        [XmlArrayItem("parameter", IsNullable=false)]
        public List<TemplateParameterInfo> Parameters
        {
            get { return parameters; }
        }
    }
}