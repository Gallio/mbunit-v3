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
using System.IO;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    /// <summary>
    /// Tests the test assembly working directory and application base directory defaults.
    /// </summary>
    [TestFixture]
    [RunSample(typeof(WorkingDirectoryAndApplicationBaseSample))]
    public class WorkingDirectoryAndApplicationBaseTest : BaseTestWithSampleRunner
    {
        [Test]
        public void WorkingDirectoryIsSameAsDirectoryContainingTestAssembly()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(WorkingDirectoryAndApplicationBaseSample).GetMethod("WorkingDirectoryIsSameAsDirectoryContainingTestAssembly")));
            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
        }

        [Test]
        public void ApplicationBaseIsSameAsDirectoryContainingTestAssembly()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(WorkingDirectoryAndApplicationBaseSample).GetMethod("ApplicationBaseIsSameAsDirectoryContainingTestAssembly")));
            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
        }

        [TestFixture, Explicit("Sample")]
        internal class WorkingDirectoryAndApplicationBaseSample
        {
            [Test]
            public void WorkingDirectoryIsSameAsDirectoryContainingTestAssembly()
            {
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)),
                    Environment.CurrentDirectory);
            }

            [Test]
            public void ApplicationBaseIsSameAsDirectoryContainingTestAssembly()
            {
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)),
                    AppDomain.CurrentDomain.BaseDirectory);
            }
        }
    }
}
