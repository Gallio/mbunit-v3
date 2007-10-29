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
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Summarizes the execution of a single test step for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("stepRun", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class StepRun
    {
        private readonly List<StepRun> children;
        private StepData step;
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
        /// <param name="step">Information about the step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="step" /> is null</exception>
        public StepRun(StepData step)
        {
            if (step == null)
                throw new ArgumentNullException(@"step");

            this.step = step;

            children = new List<StepRun>();
        }

        /// <summary>
        /// Gets or sets information about the step.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlElement("step", IsNullable=false, Namespace=SerializationUtils.XmlNamespace)]
        public StepData Step
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
        /// Gets the list of child steps.
        /// </summary>
        [XmlArray("children", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
        [XmlArrayItem("stepRun", typeof(StepRun), IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
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
        [XmlElement("result", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
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
        [XmlElement("executionLog", IsNullable = false, Namespace = SerializationUtils.XmlNamespace)]
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
