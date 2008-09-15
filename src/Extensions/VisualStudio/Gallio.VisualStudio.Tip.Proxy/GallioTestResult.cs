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
using Microsoft.VisualStudio.TestTools.Common;

namespace Gallio.VisualStudio.Tip
{
    // TODO: Save all step run details so that we can provide a custom result viewer.
    [Serializable]
    public sealed class GallioTestResult : TestResult
    {
        public GallioTestResult(GallioTestResult result)
            : base(result)
        {
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
    }
}
