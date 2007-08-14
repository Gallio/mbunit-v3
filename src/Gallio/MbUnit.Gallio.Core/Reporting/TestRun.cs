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
using MbUnit.Framework.Kernel.Results;
using MbUnit.Framework.Kernel.Utilities;

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
        private DateTime startTime;
        private DateTime endTime;
        private TestResult result;
        private ExecutionLog executionLog;

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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testId"/> is null</exception>
        public TestRun(string testId)
        {
            if (testId == null)
                throw new ArgumentNullException("testId");

            this.testId = testId;
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
                    throw new ArgumentNullException("value");
                testId = value;
            }
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
        [XmlElement("result", IsNullable = false)]
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
                    throw new ArgumentNullException("value");
                result = value;
            }
        }

        /// <summary>
        /// Gets or sets the execution log.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("executionLog", IsNullable = false)]
        public ExecutionLog ExecutionLog
        {
            get
            {
                if (executionLog == null)
                    executionLog = new ExecutionLog();
                return executionLog;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                executionLog = value;
            }
        }
    }
}
