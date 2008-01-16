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
using MbUnit.TestResources;
using MbUnit.Framework;
using Gallio.PowerShellCommands;

namespace Gallio.PowerShellCommands.Tests
{
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(RunGallioCommand))]
    [Category("UnitTests")]
    public class RunGallioCommandUnitTest
    {
        #region Private Members

        private string[] assemblies;

        #endregion

        #region SetUp and TearDown

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            string testAssemblyPath = new Uri(typeof(SimpleTest).Assembly.CodeBase).LocalPath;
            assemblies = new string[] { testAssemblyPath };
        }

        #endregion

        #region Tests

        [Test]
        public void RunWithNoArguments()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.NoTests);
            // If nothing ran then all the statistics properties should be set to zero
            Assert.AreEqual(result.Statistics.TestCount, 0);
            Assert.AreEqual(result.Statistics.FailureCount, 0);
            Assert.AreEqual(result.Statistics.IgnoreCount, 0);
            Assert.AreEqual(result.Statistics.InconclusiveCount, 0);
            Assert.AreEqual(result.Statistics.RunCount, 0);
            Assert.AreEqual(result.Statistics.SkipCount, 0);
            Assert.AreEqual(result.Statistics.AssertCount, 0);
            Assert.AreEqual(result.Statistics.Duration, 0, 0.1);
        }

        [Test]
        public void NullReportTypes()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.ReportTypes = null;
            // Just make sure it doesn't crash
            Assert.IsNotNull(command.ExecuteWithMessagePump());
        }

        [Test]
        public void EmptyReportTypes()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.ReportTypes = new string[] { String.Empty };
            // Just make sure it doesn't crash
            Assert.IsNotNull(command.ExecuteWithMessagePump());
        }

        [Test]
        public void NullReportDirectory()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.ReportDirectory = null;
            // Just make sure it doesn't crash
            Assert.IsNotNull(command.ExecuteWithMessagePump());
        }

        [Test]
        public void NullReportNameFormat()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.ReportNameFormat = null;
            // Just make sure it doesn't crash
            Assert.IsNotNull(command.ExecuteWithMessagePump());
        }

        [Test]
        public void RunAssembly()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.Assemblies = assemblies;
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Failure);
        }

        [Test]
        public void RunType()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.Assemblies = assemblies;
            command.Filter = "Type: Gallio.TestResources.MbUnit.PassingTests";
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Success);
            Assert.AreEqual(result.Statistics.TestCount, 2);
            Assert.AreEqual(result.Statistics.PassCount, 2);
            Assert.AreEqual(result.Statistics.FailureCount, 0);
            Assert.GreaterThan(result.Statistics.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(result.Statistics.AssertCount, 3);
        }

        [Test]
        public void RunFailingFixture()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.Assemblies = assemblies;
            command.Filter = "Type: Gallio.TestResources.MbUnit.FailingTests";
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Failure);
            Assert.AreEqual(result.Statistics.TestCount, 2);
            Assert.AreEqual(result.Statistics.PassCount, 0);
            Assert.AreEqual(result.Statistics.FailureCount, 2);
            Assert.GreaterThan(result.Statistics.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(result.Statistics.AssertCount, 0);
        }

        [Test]
        public void RunSingleTest()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.Assemblies = assemblies;
            command.Filter = "Type: Gallio.TestResources.MbUnit.PassingTests and Member: Pass";
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Success);
            Assert.AreEqual(result.Statistics.TestCount, 1);
            Assert.AreEqual(result.Statistics.PassCount, 1);
            Assert.AreEqual(result.Statistics.FailureCount, 0);
            Assert.GreaterThan(result.Statistics.Duration, 0);
            // The assert count is not reliable but we should be fine with simple asserts
            Assert.AreEqual(result.Statistics.AssertCount, 3);
        }

        [Test]
        public void RunSingleFailingTest()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.Assemblies = assemblies;
            command.Filter = "Type: Gallio.TestResources.MbUnit.FailingTests and Member: Fail";
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Failure);
            Assert.AreEqual(result.Statistics.TestCount, 1);
            Assert.AreEqual(result.Statistics.PassCount, 0);
            Assert.AreEqual(result.Statistics.FailureCount, 1);
            Assert.GreaterThan(result.Statistics.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(result.Statistics.AssertCount, 0);
        }

        [Test]
        public void RunIgnoredTests()
        {
            InstrumentedRunGallioCommand command = CreateCmdlet();
            command.Assemblies = assemblies;
            command.Filter = "Type: Gallio.TestResources.MbUnit.IgnoredTests";
            TestLauncherResult result = command.ExecuteWithMessagePump();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Success);
            Assert.AreEqual(result.Statistics.TestCount, 1);
            Assert.AreEqual(result.Statistics.IgnoreCount, 1);
            Assert.GreaterThan(result.Statistics.Duration, 0);
        }

        #endregion

        #region Private Methods

        private static InstrumentedRunGallioCommand CreateCmdlet()
        {
            InstrumentedRunGallioCommand command = new InstrumentedRunGallioCommand();

            return command;
        }

        #endregion
    }
}
