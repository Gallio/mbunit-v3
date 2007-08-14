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
        /// Gets or sets the id of the test that was run.
        /// </summary>
        [XmlAttribute("id")]
        public string TestId
        {
            get { return testId; }
            set { testId = value; }
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
        [XmlElement("result")]
        public TestResult Result
        {
            get { return result; }
            set { result = value; }
        }

        /// <summary>
        /// Gets or sets the execution log.
        /// </summary>
        [XmlElement("executionLog")]
        public ExecutionLog ExecutionLog
        {
            get { return executionLog; }
            set { executionLog = value; }
        }
    }
}
