// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
    /// Describes a test step in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="ITestStep"/>
    [Serializable]
    [XmlRoot("testStep", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestStepData : TestComponentData
    {
        private string fullName;
        private string parentId;
        private string testInstanceId;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestStepData()
        {
        }

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <param name="id">The step id</param>
        /// <param name="name">The step name</param>
        /// <param name="fullName">The full name of the step</param>
        /// <param name="testInstanceId">The test instance id</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/>, <paramref name="name"/>,
        /// <paramref name="fullName"/> or <paramref name="testInstanceId"/> is null</exception>
        public TestStepData(string id, string name, string fullName, string testInstanceId)
            : base(id, name)
        {
            if (fullName == null)
                throw new ArgumentNullException(@"fullName");
            if (testInstanceId == null)
                throw new ArgumentNullException("testInstanceId");

            this.fullName = fullName;
            this.testInstanceId = testInstanceId;
        }

        /// <summary>
        /// Copies the contents of a test step.
        /// </summary>
        /// <param name="source">The source test step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestStepData(ITestStep source)
            : base(source)
        {
            fullName = source.FullName;
            testInstanceId = source.TestInstance.Id;

            if (source.Parent != null)
                parentId = source.Parent.Id;
        }

        /// <summary>
        /// Gets or sets the full name of the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("fullName")]
        public string FullName
        {
            get { return fullName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                fullName = value;
            }
        }

        /// <summary>
        /// Gets or sets the id of the parent step.
        /// </summary>
        [XmlAttribute("parentId")]
        public string ParentId
        {
            get { return parentId; }
            set { parentId = value; }
        }

        /// <summary>
        /// Gets or sets the id of the test instance to which the step belongs.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("testId")]
        public string TestInstanceId
        {
            get { return testInstanceId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                testInstanceId = value;
            }
        }
    }
}