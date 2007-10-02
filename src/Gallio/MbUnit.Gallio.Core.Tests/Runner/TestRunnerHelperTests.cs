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

extern alias MbUnit2;
using System;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Core.Runner;
using Castle.Core.Logging;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Runner
{
    [TestFixture]
    [TestsOn(typeof(TestRunnerHelper))]
    [Author("Julian", "jhidalgo@mercatus.cl")]
    public class TestRunnerHelperTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstantiation_NullProgressMonitor()
        {
            new TestRunnerHelper(null, NullLogger.Instance);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestInstantiation_NullLogger()
        {
            new TestRunnerHelper(NullProgressMonitorProvider.Instance, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetFilterToNull()
        {
            TestRunnerHelper helper = new TestRunnerHelper(NullProgressMonitorProvider.Instance, NullLogger.Instance);
            helper.Filter = null;
        }
    }
}
