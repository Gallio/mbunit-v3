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
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(DependsOnAttribute))]
    [RunSample(typeof(DependsOnSample))]
    public class DependsOnTest : BaseTestWithSampleRunner
    {
        [Test]
        public void DependentTestsRunInOrderByDependency()
        {
            TestStepRun test1Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test1_DependsOnTest2_Fail")));
            TestStepRun test2Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test2_Pass")));
            TestStepRun test3Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test3_DependsOnTest1_Fail")));
            TestStepRun test4Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test4_DependsOnTest2_Pass")));

            Assert.LessThanOrEqualTo(test2Run.StartTime, test1Run.StartTime);
            Assert.LessThanOrEqualTo(test1Run.StartTime, test3Run.StartTime);
            Assert.LessThanOrEqualTo(test2Run.StartTime, test4Run.StartTime);
        }

        [Test]
        public void DependentTestsRunIfTheirDependenciesPasses()
        {
            TestStepRun test1Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test1_DependsOnTest2_Fail")));
            TestStepRun test4Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test4_DependsOnTest2_Pass")));

            Assert.Contains(test1Run.TestLog.ToString(), "Run");
            Assert.AreEqual(TestOutcome.Failed, test1Run.Result.Outcome);

            Assert.Contains(test4Run.TestLog.ToString(), "Run");
            Assert.AreEqual(TestOutcome.Passed, test4Run.Result.Outcome);
        }

        [Test]
        public void DependentTestsFailedWithoutRunningTheirDependenciesFail()
        {
            TestStepRun test3Run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(DependsOnSample).GetMethod("Test3_DependsOnTest1_Fail")));

            Assert.Contains(test3Run.TestLog.ToString(), "Skipped due to an unsatisfied test dependency.");
            Assert.DoesNotContain(test3Run.TestLog.ToString(), "Run");
            Assert.AreEqual(TestOutcome.Skipped, test3Run.Result.Outcome);
        }

        [TestFixture, Explicit("Sample")]
        internal class DependsOnSample
        {
            [Test, DependsOn("Test2_Pass")]
            public void Test1_DependsOnTest2_Fail()
            {
                TestLog.WriteLine("Run");
                Assert.Fail("Boom!");
            }

            [Test]
            public void Test2_Pass()
            {
                TestLog.WriteLine("Run");
            }

            [Test, DependsOn("Test1_DependsOnTest2_Fail")]
            public void Test3_DependsOnTest1_Fail()
            {
                TestLog.WriteLine("Run");
            }

            [Test, DependsOn("Test2_Pass")]
            public void Test4_DependsOnTest2_Pass()
            {
                TestLog.WriteLine("Run");
            }
        }

        internal class Foo
        {
            [Test]
            public void Test1()
            {
                throw new NotImplementedException();
            }
            [Test]
            [DependsOn("Test1")]
            public void Test2()
            {

            }
        }
    }
}
