// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Common.Xml;
using System.Collections.Generic;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Represents a Gallio test result.
    /// </summary>
    /// <remarks>
    /// Be VERY careful not to refer to any Gallio types that are not
    /// in the Tip Proxy assembly.  They are not in the GAC, consequently Visual
    /// Studio may be unable to load them when it needs to pass the test element instance
    /// across a remoting channel.
    /// </remarks>
    /// <todo>
    /// Include more result information.
    /// </todo>
    [Serializable]
    public sealed class GallioTestResult : TestResultAggregation
    {
        private const string TestStepRunXmlKey = "Gallio.TestStepRunXml";

        private string testStepRunXml;

        public GallioTestResult(Guid runId, ITestElement test)
            : base(Environment.MachineName, runId, test)
        {
        }

        public GallioTestResult(string computerName, Guid runId, ITestElement test)
            : base(computerName, runId, test)
        {
        }

        public GallioTestResult(TestResult result)
            : base(result)
        {
        }

        private GallioTestResult(GallioTestResult result)
            : base(result)
        {
            testStepRunXml = result.testStepRunXml;
        }

        private GallioTestResult(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            testStepRunXml = info.GetString(TestStepRunXmlKey);
        }

        /// <summary>
        /// Returns true if this test result is only container for other results
        /// and is not itself a test step run.
        /// </summary>
        public bool IsAggregateRoot
        {
            get { return testStepRunXml == null; }
        }

        /// <summary>
        /// Gets or sets the test step run xml, or null if the result is an aggregate root.
        /// </summary>
        public string TestStepRunXml
        {
            get { return testStepRunXml; }
            set { testStepRunXml = value; }
        }

        /// <summary>
        /// Adds a test result as an inner result.
        /// </summary>
        /// <param name="testResult">The test result to add.</param>
        public void AddInnerResult(GallioTestResult testResult)
        {
            int oldLength = m_innerResults.Length;

            Array.Resize(ref m_innerResults, oldLength + 1);
            m_innerResults[oldLength] = testResult;
        }

        /// <summary>
        /// Sets the list of inner results.
        /// </summary>
        /// <param name="testResults">The test results to set.</param>
        public void SetInnerResults(IList<GallioTestResult> testResults)
        {
            m_innerResults = new GallioTestResult[testResults.Count];
            for (int i = 0; i < testResults.Count; i++)
                m_innerResults[i] = testResults[i];
        }

        public override object Clone()
        {
            return new GallioTestResult(this);
        }

        public void SetTestName(string name)
        {
            m_testName = name;
        }

        public void SetTimings(DateTime startTime, DateTime endTime, TimeSpan duration)
        {
            m_startTime = startTime;
            m_endTime = endTime;
            m_duration = duration;
        }

        public override void Load(XmlElement element, XmlTestStoreParameters parameters)
        {
            base.Load(element, parameters);

            testStepRunXml = XmlPersistenceUtils.LoadFromElement(element, TestStepRunXmlKey);
        }

        public override void Save(XmlElement element, XmlTestStoreParameters parameters)
        {
            base.Save(element, parameters);

            XmlPersistenceUtils.SaveToElement(element, TestStepRunXmlKey, testStepRunXml);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(TestStepRunXmlKey, testStepRunXml);
        }
    }
}
