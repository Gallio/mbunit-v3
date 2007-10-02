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

namespace MbUnit.Core.Model
{
    /// <summary>
    /// Describes a template model component in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITemplateComponent"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public abstract class TemplateComponentData : ModelComponentData
    {
        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        protected TemplateComponentData()
        {
        }

        /// <summary>
        /// Creates a template component.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TemplateComponentData(string id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        /// Copies the contents of a template component.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateComponentData(ITemplateComponent source)
            : base(source)
        {
        }
    }
}