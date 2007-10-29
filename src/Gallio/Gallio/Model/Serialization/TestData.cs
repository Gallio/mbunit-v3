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
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// Describes a test in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITest"/>
    [Serializable]
    [XmlRoot("test", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestData : TestComponentData
    {
        private readonly List<TestData> children;
        private bool isTestCase;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestData()
        {
            children = new List<TestData>();
        }

        /// <summary>
        /// Creates a test info.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TestData(string id, string name)
            : base(id, name)
        {
            children = new List<TestData>();
        }

        /// <summary>
        /// Copies the contents of a test.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestData(ITest source)
            : base(source)
        {
            children = new List<TestData>();

            GenericUtils.ConvertAndAddAll(source.Children, children, delegate(ITest child)
            {
                return new TestData(child);
            });

            isTestCase = source.IsTestCase;
        }

        /// <summary>
        /// Gets or sets whether this node is a test case.
        /// </summary>
        /// <seealso cref="ITest.IsTestCase"/>
        [XmlAttribute("isTestCase")]
        public bool IsTestCase
        {
            get { return isTestCase; }
            set { isTestCase = value; }
        }

        /// <summary>
        /// Gets the mutable list of children.
        /// </summary>
        /// <seealso cref="IModelTreeNode{T}.Children"/>
        [XmlArray("children", IsNullable = false)]
        [XmlArrayItem("test", typeof(TestData), IsNullable = false)]
        public List<TestData> Children
        {
            get { return children; }
        }
    }
}