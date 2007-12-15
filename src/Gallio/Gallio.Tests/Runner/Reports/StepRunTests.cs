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
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Model.Serialization;
using MbUnit2::MbUnit.Framework;

namespace Gallio.Tests.Runner.Reports
{
    [TestFixture]
    [TestsOn(typeof(TestStepRun))]
    [Author("Vadim")]
    public class StepRunTests
    {
        private TestStepRun testStepRun;

        [SetUp]
        public void TestStart()
        {
            testStepRun = new TestStepRun(new TestStepData("id", "name", "fullName", "testId"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorExceptionTest()
        {
            new TestStepRun(null);
        }

        [Test]
        public void ConstructorTest()
        {
            TestStepData step = new TestStepData("id", "name", "fullName", "testId");
            TestStepRun testStepRun = new TestStepRun(step);
            Assert.AreSame(step, testStepRun.Step);
            Assert.AreEqual(0, testStepRun.Children.Count);
        }

        [Test]
        public void ResultSetTest()
        {
            testStepRun.Result = new TestResult();
            Assert.IsNotNull(testStepRun.Result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ResultSetExceptionTest()
        {
            testStepRun.Result = null;
        }

        [Test]
        public void ResultGetTest()
        {
            Assert.IsNotNull(testStepRun.Result);
        }

        [Test]
        public void StepSetTest()
        {
            testStepRun.Step = new TestStepData("stepId", "stepName", "fullName", "testId");
            Assert.IsNotNull(testStepRun.Step);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StepSetExceptionTest()
        {
            testStepRun.Step = null;
        }

        [Test]
        public void StepGetTest()
        {
            Assert.IsNotNull(testStepRun.Step);
        }

        [Test]
        public void ExecutionLogSetTest()
        {
            testStepRun.ExecutionLog = new ExecutionLog();
            Assert.IsNotNull(testStepRun.ExecutionLog);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ExecutionLogSetExceptionTest()
        {
            testStepRun.ExecutionLog = null;
        }

        [Test]
        public void ExecutionLogGetTest()
        {
            Assert.IsNotNull(testStepRun.ExecutionLog);
        }
    }
}