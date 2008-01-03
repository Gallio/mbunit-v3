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
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// Describes a test instance in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestInstance"/>
    [Serializable]
    [XmlRoot("testInstance", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestInstanceData : TestComponentData
    {
        private string testId;
        private string parentId;
        private bool isDynamic;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestInstanceData()
        {
        }

        /// <summary>
        /// Creates a test instance.
        /// </summary>
        /// <param name="id">The test instance id</param>
        /// <param name="name">The test instance name</param>
        /// <param name="testId">The test id</param>
        /// <param name="isDynamic">True if the test instance is dynamic</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/>, <paramref name="name"/> 
        /// or <paramref name="testId"/> is null</exception>
        public TestInstanceData(string id, string name, string testId, bool isDynamic)
            : base(id, name)
        {
            if (testId == null)
                throw new ArgumentNullException("testId");

            this.testId = testId;
            this.isDynamic = isDynamic;
        }

        /// <summary>
        /// Copies the contents of a test instance.
        /// </summary>
        /// <param name="source">The source test instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestInstanceData(ITestInstance source)
            : base(source)
        {
            testId = source.Test.Id;
            isDynamic = source.IsDynamic;

            if (source.Parent != null)
                parentId = source.Parent.Id;
        }

        /// <summary>
        /// Gets or sets whether the test instance is dynamic.
        /// </summary>
        /// <seealso cref="ITestInstance.IsDynamic"/>
        [XmlAttribute("isDynamic")]
        public bool IsDynamic
        {
            get { return isDynamic; }
            set { isDynamic = value; }
        }

        /// <summary>
        /// Gets or sets the id of the test to which the step belongs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="ITestInstance.Test"/>
        [XmlAttribute("testId")]
        public string TestId
        {
            get { return testId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                testId = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the parent test instance to which the step belongs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <seealso cref="ITestInstance.Parent"/>
        [XmlAttribute("parentId")]
        public string ParentId
        {
            get { return parentId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                parentId = value;
            }
        }
    }
}