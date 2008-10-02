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
using TestDriven.TestRunner.Framework;

namespace Gallio.TDNetRunner.Core
{
    internal class AdapterProxyTestListener : MarshalByRefObject, IProxyTestListener
    {
        private readonly ITestListener testListener;
        private readonly ITraceListener traceListener;

        public AdapterProxyTestListener(ITestListener testListener, ITraceListener traceListener)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (traceListener == null)
                throw new ArgumentNullException("traceListener");

            this.testListener = testListener;
            this.traceListener = traceListener;
        }

        public void TestFinished(ProxyTestResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            testListener.TestFinished(ToTestResultSummary(result));
        }

        public void Write(string text, string category)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (category == null)
                throw new ArgumentNullException("category");

            // Ignore category for now because TDNet doesn't display it very nicely.
            // traceListener.Write(text, category);
            traceListener.Write(text);
        }

        public void WriteLine(string text, string category)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (category == null)
                throw new ArgumentNullException("category");

            // Ignore category for now because TDNet doesn't display it very nicely.
            // traceListener.WriteLine(text, category);
            traceListener.WriteLine(text);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private static TestResultSummary ToTestResultSummary(ProxyTestResult result)
        {
            if (result == null)
                return null;

            return new TestResultSummary()
            {
                IsExecuted = result.IsExecuted,
                IsFailure = result.IsFailure,
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Name = result.Name,
                StackTrace = result.StackTrace,
                TestRunner = GallioTestRunner.testRunnerName,
                TimeSpan = result.TimeSpan,
                TotalTests = result.TotalTests
            };
        }
    }
}
