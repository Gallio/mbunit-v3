// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Runner;
using Gallio.TestResources.MbUnit;
using MbUnit.Framework;
using Gallio.NAntTasks;
using NAnt.Core;
using NAnt.Core.Types;
using Rhino.Mocks;

namespace Gallio.NAntTasks.Tests
{
    /// <summary>
    /// A set of unit tests for the <see cref="GallioTask" /> class (a custom Gallio task for NAnt).
    /// </summary>
    /// <remarks>
    /// If you modify these tests please make sure to review the corresponding
    /// tests for the Gallio MSBuild task, since the both the tasks for NAnt and MSBuild
    /// should exhibit a similar behavior and features set.
    /// </remarks>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(GallioTask))]
    [Category("UnitTests")]
    public class GallioTaskUnitTest
    {
        #region Private Members

        private FileSet[] assemblies;
        private readonly string resultProperty = "ExitCode";
        private readonly string resultPropertiesPrefix = "Gallio.";

        #endregion

        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            FileSet fs = new FileSet();
            string testAssemblyPath = new Uri(typeof(SimpleTest).Assembly.CodeBase).LocalPath;
            fs.FileNames.Add(testAssemblyPath);
            assemblies = new FileSet[] { fs };
        }

        #endregion

        #region Tests

        [Test]
        public void RunWithNoArguments()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Execute();
            AssertResult(task, ResultCode.NoTests);
            // If nothing ran then all the statistics properties should be set to zero
            AssertResultProperty(task, "TestCount", 0);
            AssertResultProperty(task, "PassCount", 0);
            AssertResultProperty(task, "FailureCount", 0);
            AssertResultProperty(task, "IgnoreCount", 0);
            AssertResultProperty(task, "InconclusiveCount", 0);
            AssertResultProperty(task, "RunCount", 0);
            AssertResultProperty(task, "SkipCount", 0);
            AssertResultProperty(task, "AssertCount", 0);
            AssertResultProperty(task, "Duration", 0);
        }

        [Test]
        public void NullReportTypes()
        {
            InstrumentedGallioTask task = CreateTask();
            task.ReportTypes = null;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        public void EmptyReportTypes()
        {
            InstrumentedGallioTask task = CreateTask();
            task.ReportTypes = String.Empty;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        public void NullReportDirectory()
        {
            InstrumentedGallioTask task = CreateTask();
            task.ReportDirectory = null;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        public void NullReportNameFormat()
        {
            InstrumentedGallioTask task = CreateTask();
            task.ReportNameFormat = null;
            // Just make sure it doesn't crash
            task.Execute();
        }

        [Test]
        [ExpectedException(typeof(BuildException))]
        public void RunAssembly()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.FailOnError = true;
            try
            {
                task.Execute();
            }
            catch (BuildException)
            {
                AssertResult(task, ResultCode.Failure);
                throw;
            }
        }

        [Test]
        public void RunAssemblyAndIgnoreFailures()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Execute();
            AssertResult(task, ResultCode.Failure);
        }

        [Test]
        public void RunType()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type: Gallio.TestResources.MbUnit.PassingTests";
            task.Execute();
            AssertResult(task, ResultCode.Success);
            AssertResultProperty(task, "TestCount", 2);
            AssertResultProperty(task, "PassCount", 2);
            AssertResultProperty(task, "FailureCount", 0);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 3);
        }

        [Test]
        public void RunFailingFixture()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type: Gallio.TestResources.MbUnit.FailingTests";
            task.Execute();
            AssertResult(task, ResultCode.Failure);
            AssertResultProperty(task, "TestCount", 2);
            AssertResultProperty(task, "PassCount", 0);
            AssertResultProperty(task, "FailureCount", 2);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 0);
        }

        [Test]
        public void RunSingleTest()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type: Gallio.TestResources.MbUnit.PassingTests & Member: Pass";
            task.Execute();
            AssertResult(task, ResultCode.Success);
            AssertResultProperty(task, "TestCount", 1);
            AssertResultProperty(task, "PassCount", 1);
            AssertResultProperty(task, "FailureCount", 0);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 3);
        }

        [Test]
        public void RunSingleFailingTest()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type: Gallio.TestResources.MbUnit.FailingTests & Member: Fail";
            task.Execute();
            AssertResult(task, ResultCode.Failure);
            AssertResultProperty(task, "TestCount", 1);
            AssertResultProperty(task, "PassCount", 0);
            AssertResultProperty(task, "FailureCount", 1);
            AssertDurationIsGreaterThanZero(task);
            // The assert count is not reliable but we should be fine with simple asserts
            AssertResultProperty(task, "AssertCount", 0);
        }

        [Test]
        public void RunIgnoredTests()
        {
            InstrumentedGallioTask task = CreateTask();
            task.Assemblies = assemblies;
            task.Filter = "Type: Gallio.TestResources.MbUnit.IgnoredTests";
            task.Execute();
            AssertResult(task, ResultCode.Success);
            AssertResultProperty(task, "TestCount", 1);
            AssertResultProperty(task, "IgnoreCount", 1);
            AssertDurationIsGreaterThanZero(task);
        }

        [Test]
        public void AddHintAndPluginDirectories()
        {
            InstrumentedGallioTask task = CreateTask();
            DirSet ds = new DirSet();
            ds.FileNames.Add(@"C:\Windows");
            task.HintDirectories = new DirSet[] { ds };
            task.PluginDirectories = new DirSet[] { ds };
            task.Execute();
        }

        #endregion

        #region Private Methods

        private InstrumentedGallioTask CreateTask()
        {
            InstrumentedGallioTask task = new InstrumentedGallioTask();
            task.InitializeTaskConfiguration();
            task.FailOnError = false;
            task.ResultProperty = resultProperty;
            task.ResultPropertiesPrefix = resultPropertiesPrefix;

            return task;
        }

        private void AssertResult(Element task, int expectedValue)
        {
            Assert.AreEqual(task.Properties[resultProperty], expectedValue.ToString());
        }

        private void AssertResultProperty(Element task, string name, int expectedValue)
        {
            Assert.AreEqual(task.Properties[resultPropertiesPrefix + name], expectedValue.ToString());
        }

        private void AssertDurationIsGreaterThanZero(Element task)
        {
            Assert.GreaterThan(task.Properties[resultPropertiesPrefix + "Duration"], 0.ToString());
        }

        #endregion
    }
}
