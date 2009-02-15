// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;
using Microsoft.VisualStudio.TestTools.TestAdapter;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Marshal-by-ref base <see cref="ITestAdapter" /> proxy implementation.
    /// </summary>
    public class MarshalByRefTestAdapterProxy : MarshalByRefObject, ITestAdapter
    {
        private readonly ITestAdapter target;

        public MarshalByRefTestAdapterProxy(ITestAdapter target)
        {
            this.target = target;
        }

        public void Run(ITestElement testElement, ITestContext testContext)
        {
            target.Run(testElement, testContext);
        }

        public void Cleanup()
        {
            target.Cleanup();
        }

        public void StopTestRun()
        {
            target.StopTestRun();
        }

        public void AbortTestRun()
        {
            target.AbortTestRun();
        }

        public void PauseTestRun()
        {
            target.PauseTestRun();
        }

        public void ResumeTestRun()
        {
            target.ResumeTestRun();
        }

        public void Initialize(IRunContext runContext)
        {
            target.Initialize(runContext);
        }

        public void ReceiveMessage(object obj)
        {
            target.ReceiveMessage(obj);
        }

        public void PreTestRunFinished(IRunContext runContext)
        {
            target.PreTestRunFinished(runContext);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
