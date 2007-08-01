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
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a template parameter set in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplateParameterSet"/>
    [Serializable]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TemplateParameterSetInfo : TemplateComponentInfo
    {
        private TemplateParameterInfo[] parameters;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TemplateParameterSetInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TemplateParameterSetInfo(ITemplateParameterSet obj)
            : base(obj)
        {
            parameters = ListUtils.ConvertAllToArray<ITemplateParameter, TemplateParameterInfo>(obj.Parameters,
                delegate(ITemplateParameter parameter)
                {
                    return new TemplateParameterInfo(parameter);
                });
        }

        /// <summary>
        /// Gets or sets the test parameters.  (non-null but possibly empty)
        /// </summary>
        /// <seealso cref="ITemplateParameterSet.Parameters"/>
        [XmlArray("parameters", IsNullable=false)]
        [XmlArrayItem("parameter", IsNullable=false)]
        public TemplateParameterInfo[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
    }
}