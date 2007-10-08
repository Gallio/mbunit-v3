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
using System.Text;
using System.Xml.Serialization;
using MbUnit.Core.Reporting;
using MbUnit.Core.Harness;
using MbUnit.Model.Serialization;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Summarizes the execution of a test for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("testRun", Namespace=SerializationUtils.XmlNamespace)]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class TestRun
    {
        private string testId;
        private StepRun rootStepRun;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestRun()
        {
        }

        /// <summary>
        /// Creates a test run.
        /// </summary>
        /// <param name="testId">The test id</param>
        /// <param name="rootStepRun">The root step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> or
        /// <paramref name="rootStepRun"/> is null</exception>
        public TestRun(string testId, StepRun rootStepRun)
        {
            if (testId == null)
                throw new ArgumentNullException(@"testId");
            if (rootStepRun == null)
                throw new ArgumentNullException(@"rootStepRun");

            this.testId = testId;
            this.rootStepRun = rootStepRun;
        }

        /// <summary>
        /// Gets or sets the id of the test that was run.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("id")]
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
        /// Gets or sets the root step of the test run.
        /// The value cannot be null because a test run always has a root step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("stepRun", IsNullable = false)]
        public StepRun RootStepRun
        {
            get { return rootStepRun; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                rootStepRun = value;
            }
        }

        /// <summary>
        /// Recursively enumerates all test run steps.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<StepRun> StepRuns
        {
            get
            {
                return EnumerateStepRunsRecursively(rootStepRun);
            }
        }

        private static IEnumerable<StepRun> EnumerateStepRunsRecursively(StepRun parent)
        {
            yield return parent;

            foreach (StepRun child in parent.Children)
                foreach (StepRun stepRun in EnumerateStepRunsRecursively(child))
                    yield return stepRun;
        }
    }
}
