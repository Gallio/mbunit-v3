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
using System.Collections;
using Microsoft.VisualStudio.TestTools.Common;
using TestResult=Microsoft.VisualStudio.TestTools.Common.TestResult;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Proxies the <see cref="ITip" /> interface over to the actual implementation
    /// after initializing the loader.
    /// </summary>
    public class GallioTipProxy : ITip
    {
        private readonly ITip target;

        public GallioTipProxy(ITmi tmi)
        {
            target = ProxyHelper.GetTargetFactory().CreateTip(tmi);
        }

        public void Dispose()
        {
            target.Dispose();
        }

        public ICollection Load(string location, ProjectData projectData, IWarningHandler warningHandler)
        {
            return target.Load(location, projectData, warningHandler);
        }

        public void Unload(string location, IWarningHandler warningHandler)
        {
            target.Unload(location, warningHandler);
        }

        public void Save(ITestElement[] tests, string location, ProjectData projectData)
        {
            target.Save(tests, location, projectData);
        }

        public TestResult MergeResults(TestResult inMemory, TestResultMessage fromTheWire)
        {
            return target.MergeResults(inMemory, fromTheWire);
        }

        public void Reset()
        {
            target.Reset();
        }

        public void BeforeRemoveTestRun(Guid runId)
        {
            target.BeforeRemoveTestRun(runId);
        }

        public TestType TestType
        {
            get { return target.TestType; }
        }
    }
}
