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

using System;
using MbUnit.Framework;
using MbUnit.Runner;
using MbUnit.TestResources.MbUnit2;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Rhino.Mocks;

namespace MbUnit.Tasks.MSBuild.Tests
{
    /// <summary>
    /// A set of unit tests for the MbUnit class (a custom MbUnit task for MSBuild).
    /// </summary>
    /// <remarks>
    /// If you modify these tests please make sure to review the tests in the
    /// NAntTasksUnitTests fixture, since the both the tasks for NAnt and MSBuild
    /// should exhibit a similar behavior and features set.
    /// </remarks>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnit))]
    [Category("UnitTests")]
    public class MbUnitTaskUnitTest
    {
        #region Private Members
        
        private IBuildEngine stubbedBuildEngine = null;
        private ITaskItem[] assemblies;

        #endregion

        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            stubbedBuildEngine = MockRepository.GenerateStub<IBuildEngine>();
            string testAssemblyPath = new Uri(typeof(SimpleTest).Assembly.CodeBase).LocalPath;
            TaskItem ti = new TaskItem(testAssemblyPath);
            assemblies = new ITaskItem[] {ti};
        }

        #endregion

        #region Tests

        [Test]
        public void RunWithNoArguments()
        {
            InstrumentedMbUnitTask task = CreateTask();
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.NoTests);
            // If nothing ran then all the statistics properties should be set to zero
            Assert.AreEqual(task.TestCount, 0);
            Assert.AreEqual(task.FailureCount, 0);
            Assert.AreEqual(task.IgnoreCount, 0);
            Assert.AreEqual(task.InconclusiveCount, 0);
            Assert.AreEqual(task.RunCount, 0);
            Assert.AreEqual(task.SkipCount, 0);
            Assert.AreEqual(task.AssertCount, 0);
            Assert.AreEqual(task.Duration, 0, 0.1);
        }

        [Test]
        public void NullReportTypes()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportTypes = null;
            // Just make sure it doesn't crash
            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void NullReportDirectory()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportDirectory = null;
            // Just make sure it doesn't crash
            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void NullReportNameFormat()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.ReportNameFormat = null;
            // Just make sure it doesn't crash
            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void RunAssembly()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            Assert.IsFalse(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
        }

        [Test]
        public void RunAssemblyAndIgnoreFailures()
        {

            InstrumentedMbUnitTask task = CreateTask();
            task.IgnoreFailures = true;
            task.Assemblies = assemblies;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
        }

        [Test]
        public void RunType()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.IgnoreFailures = true;
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.PassingTests";
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Success);
            Assert.AreEqual(task.TestCount, 4);
            Assert.AreEqual(task.PassCount, 4);
            Assert.AreEqual(task.FailureCount, 0);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(task.AssertCount, 3);
        }

        [Test]
        public void RunFailingFixture()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.FailingFixture";
            Assert.IsFalse(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
            Assert.AreEqual(task.TestCount, 2);
            Assert.AreEqual(task.PassCount, 1);
            Assert.AreEqual(task.FailureCount, 1);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(task.AssertCount, 0);
        }

        [Test]
        public void RunSingleTest()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.PassingTests;Member=Pass";
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Success);
            Assert.AreEqual(task.TestCount, 1);
            Assert.AreEqual(task.PassCount, 1);
            Assert.AreEqual(task.FailureCount, 0);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(task.AssertCount, 3);
        }

        [Test]
        public void RunSingleFailingTest()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.FailingFixture;Member=Fail";
            Assert.IsFalse(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
            Assert.AreEqual(task.TestCount, 1);
            Assert.AreEqual(task.PassCount, 0);
            Assert.AreEqual(task.FailureCount, 1);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(task.AssertCount, 0);
        }

        [Test]
        public void RunIgnoredTests()
        {
            InstrumentedMbUnitTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.IgnoredTests";
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Success);
            Assert.AreEqual(task.TestCount, 2);
            Assert.AreEqual(task.IgnoreCount, 2);
            Assert.GreaterThan(task.Duration, 0);
        }

        #endregion

        #region Private Methods

        private InstrumentedMbUnitTask CreateTask()
        {
            InstrumentedMbUnitTask task = new InstrumentedMbUnitTask();
            task.BuildEngine = stubbedBuildEngine;
            task.IgnoreFailures = false;

            return task;
        }

        #endregion
    }
}
