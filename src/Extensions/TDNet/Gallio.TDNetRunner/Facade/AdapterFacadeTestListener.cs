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
using TestDriven.Framework;

namespace Gallio.TDNetRunner.Facade
{
    /// <summary>
    /// A facade and remote proxy for the TestDriven.Net test listener.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the TestDriven.Net interfaces.
    /// </remarks>
    internal class AdapterFacadeTestListener : MarshalByRefObject, IFacadeTestListener
    {
        private readonly ITestListener testListener;

        public AdapterFacadeTestListener(ITestListener testListener)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");

            this.testListener = testListener;
        }

        public void TestFinished(FacadeTestResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            testListener.TestFinished(FacadeUtils.ToTestResult(result));
        }

        public void TestResultsUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            testListener.TestResultsUrl(url);
        }

        public void WriteLine(string text, FacadeCategory category)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            testListener.WriteLine(text, FacadeUtils.ToCategory(category));
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
