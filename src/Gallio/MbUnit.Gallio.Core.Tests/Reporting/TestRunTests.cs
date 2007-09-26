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
using System.Collections.Generic;
using MbUnit.Core.Reporting;
using MbUnit2::MbUnit.Framework;
using Rhino.Mocks;

namespace MbUnit.Core.Tests.Reporting
{
    [TestFixture]
    [TestsOn(typeof(TestRun))]
    [Author("Vadim")]
    public class TestRunTests
    {
        private MockRepository _mocks;
        private IStepRun _stepRunMock;
        private TestRun _testRun;

        [SetUp]
        public void TestStart()
        {
            _mocks = new MockRepository();
            _stepRunMock = _mocks.CreateMock<IStepRun>();
            _testRun = new TestRun("id", _stepRunMock);
        }

        [RowTest]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        [Row("newId")]
        public void ConstructorTest(string testId)
        {
            TestRun testRun = new TestRun(testId, _stepRunMock);
            Assert.AreEqual(testId, testRun.TestId);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorWithNullStepRun()
        {
            new TestRun("id", null);
        }

        [RowTest]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        [Row("newId")]
        public void TestIdSetTest(string testId)
        {
            _testRun.TestId = testId;
            Assert.AreEqual(testId, _testRun.TestId);
        }

        [Test]
        public void RootStepRunTest()
        {
            SetupResult.For(_stepRunMock.StepId).PropertyBehavior();
            SetupResult.For(_stepRunMock.StepName).PropertyBehavior();
            _mocks.ReplayAll();
            _stepRunMock.StepId = "newStepId";
            _stepRunMock.StepName = "mockStepName";
            _testRun.RootStepRun = _stepRunMock;
            Assert.AreEqual("newStepId", _testRun.RootStepRun.StepId);
            Assert.AreEqual("mockStepName", _testRun.RootStepRun.StepName);
            _mocks.VerifyAll();
        }

        [Test]
        [ExpectedArgumentNullException]
        public void RootStepRunNullTest()
        {
            _testRun.RootStepRun = null;
        }

        [Test]
        public void StepRunsWithOneChildTest()
        {
            List<IStepRun> children = new List<IStepRun>();
            IStepRun stepRunChild = MockRepository.GenerateStub<IStepRun>();
            stepRunChild.StepId = "childId";
            stepRunChild.StepName = "childStepName";
            children.Add(stepRunChild);
            Expect.Call(_stepRunMock.StepId).PropertyBehavior();
            Expect.Call(_stepRunMock.StepName).PropertyBehavior();
            Expect.Call(_stepRunMock.Children).Return(children);
            _mocks.ReplayAll();
            _stepRunMock.StepId = "newStepId";
            _stepRunMock.StepName = "mockStepName";
            foreach (IStepRun step in _testRun.StepRuns)
            {
                Console.WriteLine(step.StepId);
            }
            _mocks.VerifyAll();
        }
    }
}