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
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Common.Xml;

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
    public sealed class GallioTestResult : TestResult
    {
        private const string TestStepRunXmlKey = "Gallio.TestStepRunXml";

        private string testStepRunXml;

        public GallioTestResult(GallioTestResult result)
            : base(result)
        {
            testStepRunXml = result.testStepRunXml;
        }

        public GallioTestResult(TestResult result)
            : base(result)
        {
        }

        public GallioTestResult(string computerName, Guid runId, ITestElement test)
            : base(computerName, runId, test)
        {
        }

        private GallioTestResult(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            testStepRunXml = info.GetString(TestStepRunXmlKey);
        }

        public string TestStepRunXml
        {
            get { return testStepRunXml; }
            set { testStepRunXml = value; }
        }

        public override object Clone()
        {
            return new GallioTestResult(this);
        }

        public void MergeFrom(GallioTestResult source)
        {
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
