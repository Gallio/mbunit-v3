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
using Gallio.Model.Serialization;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Summarizes the execution of a single test instance for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("testInstanceRun", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestInstanceRun
    {
        private TestInstanceData testInstance;
        private TestStepRun rootTestStepRun;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestInstanceRun()
        {
        }

        /// <summary>
        /// Creates a test instance step.
        /// </summary>
        /// <param name="testInstance">Information about the test instance</param>
        /// <param name="rootTestStepRun">The root test step run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testInstance" /> 
        /// or <paramref name="rootTestStepRun"/> is null</exception>
        public TestInstanceRun(TestInstanceData testInstance, TestStepRun rootTestStepRun)
        {
            if (testInstance == null)
                throw new ArgumentNullException("testInstance");
            if (rootTestStepRun == null)
                throw new ArgumentNullException("rootTestStepRun");

            this.testInstance = testInstance;
            this.rootTestStepRun = rootTestStepRun;
        }

        /// <summary>
        /// Gets or sets information about the test instance.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("testInstance", IsNullable=false, Namespace=SerializationUtils.XmlNamespace)]
        public TestInstanceData TestInstance
        {
            get { return testInstance; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                testInstance = value;
            }
        }

        /// <summary>
        /// Gets or sets the root test step run of the test instance.
        /// The value cannot be null because a test instance must have a root step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("testStepRun", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
        public TestStepRun RootTestStepRun
        {
            get { return rootTestStepRun; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                rootTestStepRun = value;
            }
        }

        /// <summary>
        /// Recursively enumerates all test run steps.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<TestStepRun> TestStepRuns
        {
            get
            {
                return EnumerateStepRunsRecursively(rootTestStepRun);
            }
        }

        private static IEnumerable<TestStepRun> EnumerateStepRunsRecursively(TestStepRun parent)
        {
            yield return parent;

            foreach (TestStepRun child in parent.Children)
                foreach (TestStepRun stepRun in EnumerateStepRunsRecursively(child))
                    yield return stepRun;
        }    
    }
}