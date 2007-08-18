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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Describes a test model component in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestComponent"/>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class TestComponentInfo : ModelComponentInfo, ITestComponent
    {
        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        protected TestComponentInfo()
        {
        }

        /// <summary>
        /// Creates a test component.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TestComponentInfo(string id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        /// Copies the contents of a test component.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TestComponentInfo(ITestComponent obj) : base(obj)
        {
        }
    }
}