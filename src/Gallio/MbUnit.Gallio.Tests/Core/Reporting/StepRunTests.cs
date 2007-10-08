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
using MbUnit.Model.Execution;
using MbUnit.Core.Reporting;
using MbUnit.Model.Serialization;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Tests.Core.Reporting
{
    [TestFixture]
    [TestsOn(typeof(StepRun))]
    [Author("Vadim")]
    public class StepRunTests
    {
        private StepRun _stepRun;

        [SetUp]
        public void TestStart()
        {
            _stepRun = new StepRun(new StepData("id", "name", "fullName", "testId"));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorExceptionTest()
        {
            new StepRun(null);
        }

        [Test]
        public void ConstructorTest()
        {
            StepData step = new StepData("id", "name", "fullName", "testId");
            StepRun stepRun = new StepRun(step);
            Assert.AreSame(step, stepRun.Step);
            Assert.AreEqual(0, stepRun.Children.Count);
        }

        [Test]
        public void ResultSetTest()
        {
            _stepRun.Result = new TestResult();
            Assert.IsNotNull(_stepRun.Result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ResultSetExceptionTest()
        {
            _stepRun.Result = null;
        }

        [Test]
        public void ResultGetTest()
        {
            Assert.IsNotNull(_stepRun.Result);
        }

        [Test]
        public void StepSetTest()
        {
            _stepRun.Step = new StepData("stepId", "stepName", "fullName", "testId");
            Assert.IsNotNull(_stepRun.Step);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void StepSetExceptionTest()
        {
            _stepRun.Step = null;
        }

        [Test]
        public void StepGetTest()
        {
            Assert.IsNotNull(_stepRun.Step);
        }

        [Test]
        public void ExecutionLogSetTest()
        {
            _stepRun.ExecutionLog = new ExecutionLog();
            Assert.IsNotNull(_stepRun.ExecutionLog);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ExecutionLogSetExceptionTest()
        {
            _stepRun.ExecutionLog = null;
        }

        [Test]
        public void ExecutionLogGetTest()
        {
            Assert.IsNotNull(_stepRun.ExecutionLog);
        }
    }
}