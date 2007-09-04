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
    /// Describes a test in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITest"/>
    [Serializable]
    [XmlRoot("test", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestInfo : TestComponentInfo, ITest
    {
        private readonly List<TestInfo> children;
        private bool isTestCase;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestInfo()
        {
            children = new List<TestInfo>();
        }

        /// <summary>
        /// Creates a test info.
        /// </summary>
        /// <param name="id">The component id</param>
        /// <param name="name">The component name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="name"/> is null</exception>
        public TestInfo(string id, string name)
            : base(id, name)
        {
            children = new List<TestInfo>();
        }

        /// <summary>
        /// Copies the contents of a test.
        /// </summary>
        /// <param name="obj">The model object</param>
        public TestInfo(ITest obj)
            : base(obj)
        {
            children = new List<TestInfo>();

            ListUtils.ConvertAndAddAll(obj.Children, children, delegate(ITest child)
            {
                return new TestInfo(child);
            });

            isTestCase = obj.IsTestCase;
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
        [XmlArrayItem("test", IsNullable = false)]
        public List<TestInfo> Children
        {
            get { return children; }
        }

        #region ITest implementation

        ITemplateBinding ITest.TemplateBinding
        {
            get { throw new NotSupportedException(); }
        }

        IList<ITest> ITest.Dependencies
        {
            get { throw new NotSupportedException(); }
        }

        TestBatch ITest.Batch
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        ITest IModelTreeNode<ITest>.Parent
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        IList<ITest> IModelTreeNode<ITest>.Children
        {
            get
            {
                return ListUtils.CopyAllToArray(children);
            }
        }

        void IModelTreeNode<ITest>.AddChild(ITest node)
        {
            children.Add((TestInfo)node);
        }

        #endregion
    }
}