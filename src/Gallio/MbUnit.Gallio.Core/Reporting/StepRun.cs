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
using MbUnit.Framework.Kernel.Results;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Summarizes the execution of a single test step for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("stepRun", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class StepRun
    {
        private readonly List<StepRun> children;
        private string stepId;
        private string stepName;
        private string stepFullName;
        private DateTime startTime;
        private DateTime endTime;
        private TestResult result;
        private ExecutionLog executionLog;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private StepRun()
        {
            children = new List<StepRun>();
        }

        /// <summary>
        /// Creates a test run step.
        /// </summary>
        /// <param name="stepId">The step id</param>
        /// <param name="stepName">The name of the step</param>
        /// <param name="stepFullName">The full name of the step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stepId"/>,
        /// <paramref name="stepName"/> or <paramref name="stepFullName"/> is null</exception>
        public StepRun(string stepId, string stepName, string stepFullName)
        {
            if (stepId == null)
                throw new ArgumentNullException(@"stepId");
            if (stepName == null)
                throw new ArgumentNullException(@"stepName");
            if (stepFullName == null)
                throw new ArgumentNullException(@"stepFullName");

            this.stepId = stepId;
            this.stepName = stepName;
            this.stepFullName = stepFullName;

            children = new List<StepRun>();
        }

        /// <summary>
        /// Gets or sets the id of the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("id")]
        public string StepId
        {
            get { return stepId; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                stepId = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("name")]
        public string StepName
        {
            get { return stepName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                stepName = value;
            }
        }

        /// <summary>
        /// Gets or sets the full name of the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("fullName")]
        public string StepFullName
        {
            get { return stepFullName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");
                stepFullName = value;
            }
        }

        /// <summary>
        /// Gets the list of child steps.
        /// </summary>
        [XmlArray("children", IsNullable=false)]
        [XmlArrayItem("stepRun", IsNullable=false)]
        public List<StepRun> Children
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
                    throw new ArgumentNullException(@"value");
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
                    throw new ArgumentNullException(@"value");
                executionLog = value;
            }
        }
    }
}
