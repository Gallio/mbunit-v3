// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Summarizes the execution of a single test step for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("testStepRun", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class TestStepRun
    {
        private readonly List<TestStepRun> children;
        private TestStepData step;
        private DateTime startTime;
        private DateTime endTime;
        private TestResult result;
        private TestLog testLog;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestStepRun()
        {
            children = new List<TestStepRun>();
        }

        /// <summary>
        /// Creates a test run step.
        /// </summary>
        /// <param name="step">Information about the step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="step" /> is null</exception>
        public TestStepRun(TestStepData step)
            : this()
        {
            if (step == null)
                throw new ArgumentNullException(@"step");

            this.step = step;
        }

        /// <summary>
        /// Gets or sets information about the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("testStep", IsNullable=false, Namespace=XmlSerializationUtils.GallioNamespace)]
        public TestStepData Step
        {
            get { return step; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                step = value;
            }
        }

        /// <summary>
        /// Gets the list of child step runs.
        /// </summary>
        [XmlArray("children", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("testStepRun", typeof(TestStepRun), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<TestStepRun> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets or sets the time when the test run started.
        /// </summary>
        [XmlAttribute("startTime")]
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Gets or sets the time when the test run ended.
        /// </summary>
        [XmlAttribute("endTime")]
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        /// <summary>
        /// Gets or sets the test result from the run.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("result", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public TestResult Result
        {
            get
            {
                if (result == null)
                    result = new TestResult();
                return result;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                result = value;
            }
        }

        /// <summary>
        /// Gets or sets the test log.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("testLog", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public TestLog TestLog
        {
            get
            {
                if (testLog == null)
                    testLog = new TestLog();
                return testLog;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                testLog = value;
            }
        }

        /// <summary>
        /// Recursively enumerates all test step runs including this one.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<TestStepRun> AllTestStepRuns
        {
            get { return TreeUtils.GetPreOrderTraversal(this, GetChildren); }
        }

        private static IEnumerable<TestStepRun> GetChildren(TestStepRun node)
        {
            return node.Children;
        }
    }
}
