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

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// Describes a template in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplate"/>
    [Serializable]
    [XmlRoot("template", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class TemplateInfo : TemplateComponentInfo
    {
        private TemplateInfo[] children;
        private TemplateParameterSetInfo[] parameterSets;

        /// <summary>
        /// Creates an empty object.
        /// </summary>
        public TemplateInfo()
        {
        }

        /// <summary>
        /// Creates an serializable description of a model object.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TemplateInfo(ITemplate obj)
            : base(obj)
        {
            List<TemplateInfo> childrenInfo = new List<TemplateInfo>();
            foreach (ITemplate child in obj.Children)
                childrenInfo.Add(new TemplateInfo(child));
            children = childrenInfo.ToArray();

            parameterSets = ListUtils.ConvertAllToArray<ITemplateParameterSet, TemplateParameterSetInfo>(obj.ParameterSets,
                delegate(ITemplateParameterSet parameterSet)
                {
                    return new TemplateParameterSetInfo(parameterSet);
                });
        }

        /// <summary>
        /// Gets or sets the children.  (non-null but possibly empty)
        /// </summary>
        /// <seealso cref="ITemplate.Children"/>
        [XmlArray("children", IsNullable=false)]
        [XmlArrayItem("template", IsNullable=false)]
        public TemplateInfo[] Children
        {
            get { return children; }
            set { children = value; }
        }

        /// <summary>
        /// Gets or sets the parameter sets.  (non-null but possibly empty)
        /// </summary>
        /// <seealso cref="ITemplate.ParameterSets"/>
        [XmlArray("parameterSets", IsNullable = false)]
        [XmlArrayItem("parameterSet", IsNullable = false)]
        public TemplateParameterSetInfo[] ParameterSets
        {
            get { return parameterSets; }
            set { parameterSets = value; }
        }
    }
}