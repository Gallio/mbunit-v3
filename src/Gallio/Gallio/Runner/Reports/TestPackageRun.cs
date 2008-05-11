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
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Summarizes the execution of a test package for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class TestPackageRun
    {
        private Statistics statistics;
        private TestStepRun rootTestStepRun;
        private DateTime startTime;
        private DateTime endTime;

        /// <summary>
        /// Creates an empty package run.
        /// </summary>
        public TestPackageRun()
        {
        }

        /// <summary>
        /// Gets or sets the time when the package run started.
        /// </summary>
        [XmlAttribute("startTime")]
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Gets or sets the time when the package run ended.
        /// </summary>
        [XmlAttribute("endTime")]
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        /// <summary>
        /// Gets or sets the root test step run, or null if the root test has not run.
        /// </summary>
        [XmlElement("testStepRun", IsNullable = false)]
        public TestStepRun RootTestStepRun
        {
            get { return rootTestStepRun; }
            set { rootTestStepRun = value; }
        }

        /// <summary>
        /// Gets or sets the statistics for the package run.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("statistics", Namespace = XmlSerializationUtils.GallioNamespace, IsNullable = false)]
        public Statistics Statistics
        {
            get
            {
                if (statistics == null)
                    statistics = new Statistics();
                return statistics;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                statistics = value;
            }
        }

        /// <summary>
        /// Recursively enumerates all test step runs including the root test step run.
        /// </summary>
        [XmlIgnore]
        public IEnumerable<TestStepRun> AllTestStepRuns
        {
            get
            {
                if (rootTestStepRun == null)
                    return EmptyArray<TestStepRun>.Instance;

                return TreeUtils.GetPreOrderTraversal(rootTestStepRun, GetChildren);
            }
        }

        private static IEnumerable<TestStepRun> GetChildren(TestStepRun node)
        {
            return node.Children;
        }
    }
}
