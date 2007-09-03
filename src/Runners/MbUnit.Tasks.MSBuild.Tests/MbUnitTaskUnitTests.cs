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
using System.IO;
using MbUnit.Core.Runner;
using MbUnit2::MbUnit.Framework;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Rhino.Mocks;

namespace MbUnit.Tasks.MSBuild.Tests
{
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnit))]
    public class MbUnitTaskUnitTests
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
            string testAssemblyPath =
                Path.GetFullPath(
                    @"..\..\..\TestResources\MbUnit.TestResources.MbUnit2\bin\MbUnit.TestResources.MbUnit2.dll");
            TaskItem ti = new TaskItem(testAssemblyPath);
            assemblies = new ITaskItem[] {ti};
        }

        #endregion

        #region Tests

        [Test]
        public void RunWithNoArguments()
        {
            MbUnit task = CreateTask();
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.NoTests);
            Assert.AreEqual(task.TestCount, 0);
        }

        [Test]
        public void NullReportTypes()
        {
            MbUnit task = CreateTask();
            task.ReportTypes = null;
            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void NullReportDirectory()
        {
            MbUnit task = CreateTask();
            task.ReportDirectory = null;
            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void NullReportNameFormat()
        {
            MbUnit task = CreateTask();
            task.ReportNameFormat = null;
            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void RunAssembly()
        {
            MbUnit task = CreateTask();
            task.Assemblies = assemblies;
            Assert.IsFalse(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
        }

        [Test]
        public void RunAssemblyAndIgnoreFailures()
        {
            
            MbUnit task = CreateTask();
            task.IgnoreFailures = true;
            task.Assemblies = assemblies;
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
        }

        [Test]
        public void RunType()
        {
            MbUnit task = CreateTask();
            task.IgnoreFailures = true;
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.PassingTests";
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Success);
            Assert.AreEqual(task.TestCount, 4);
            Assert.AreEqual(task.PassCount, 4);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable
            //Assert.AreEqual(task.AssertCount, 0);
        }

        [Test]
        public void RunFailingFixture()
        {
            MbUnit task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.FailingFixture";
            Assert.IsFalse(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
            Assert.AreEqual(task.TestCount, 2);
            Assert.AreEqual(task.FailureCount, 1);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable
            //Assert.AreEqual(task.AssertCount, 1);
        }

        [Test]
        public void RunSingleTest()
        {
            MbUnit task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.PassingTests;Member=Pass";
            Assert.IsTrue(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Success);
            Assert.AreEqual(task.TestCount, 1);
            Assert.AreEqual(task.PassCount, 1);
            Assert.AreEqual(task.FailureCount, 0);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable
            //Assert.AreEqual(task.AssertCount, 1);
        }

        [Test]
        public void RunSingleFailingTest()
        {
            MbUnit task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type=MbUnit.TestResources.MbUnit2.FailingFixture;Member=Fail";
            Assert.IsFalse(task.Execute());
            Assert.AreEqual(task.ExitCode, ResultCode.Failure);
            Assert.AreEqual(task.TestCount, 1);
            Assert.AreEqual(task.PassCount, 0);
            Assert.AreEqual(task.FailureCount, 1);
            Assert.GreaterThan(task.Duration, 0);
            // The assert count is not reliable
            //Assert.AreEqual(task.AssertCount, 1);
        }

        [Test]
        public void RunIgnoredTests()
        {
            MbUnit task = CreateTask();
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

        private MbUnit CreateTask()
        {
            MbUnit task = new MbUnit();
            task.BuildEngine = stubbedBuildEngine;
            task.IgnoreFailures = false;

            return task;
        }

        #endregion
    }
}
