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
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a template parameter in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplateParameter"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TemplateParameterInfo : TemplateComponentInfo
    {
        private string typeName;
        private int index;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TemplateParameterInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TemplateParameterInfo(ITemplateParameter obj)
            : base(obj)
        {
            typeName = obj.Type.FullName;
            index = obj.Index;
        }

        /// <summary>
        /// Gets or sets the fully-qualified type name of the parameter's value type.  (non-null)
        /// </summary>
        /// <seealso cref="ITemplateParameter.Type"/>
        [XmlAttribute("type")]
        public string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the index of the parameter.
        /// </summary>
        /// <seealso cref="ITemplateParameter.Index"/>
        [XmlAttribute("index")]
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
    }
}