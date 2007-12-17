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
using Gallio.Runner;
using Gallio.TestResources.MbUnit2;
using MbUnit.Framework;
using Gallio.PowerShellCmdlet;

namespace Gallio.PowerShellCmdlet.Tests
{
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(GallioCmdlet))]
    [Category("UnitTests")]
    public class GallioCmdletUnitTest
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
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            TestLauncherResult result = cmdlet.Execute();
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
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.ReportTypes = null;
            // Just make sure it doesn't crash
            Assert.IsNotNull(cmdlet.Execute());
        }

        [Test]
        public void EmptyReportTypes()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.ReportTypes = new string[] { String.Empty };
            // Just make sure it doesn't crash
            Assert.IsNotNull(cmdlet.Execute());
        }

        [Test]
        public void NullReportDirectory()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.ReportDirectory = null;
            // Just make sure it doesn't crash
            Assert.IsNotNull(cmdlet.Execute());
        }

        [Test]
        public void NullReportNameFormat()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.ReportNameFormat = null;
            // Just make sure it doesn't crash
            Assert.IsNotNull(cmdlet.Execute());
        }

        [Test]
        public void RunAssembly()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.Assemblies = assemblies;
            TestLauncherResult result = cmdlet.Execute();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Failure);
        }

        [Test]
        public void RunType()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.Assemblies = assemblies;
            cmdlet.Filter = "Type: Gallio.TestResources.MbUnit2.PassingTests";
            TestLauncherResult result = cmdlet.Execute();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Success);
            Assert.AreEqual(result.Statistics.TestCount, 4);
            Assert.AreEqual(result.Statistics.PassCount, 4);
            Assert.AreEqual(result.Statistics.FailureCount, 0);
            Assert.GreaterThan(result.Statistics.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(result.Statistics.AssertCount, 3);
        }

        [Test]
        public void RunFailingFixture()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.Assemblies = assemblies;
            cmdlet.Filter = "Type: Gallio.TestResources.MbUnit2.FailingFixture";
            TestLauncherResult result = cmdlet.Execute();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Failure);
            Assert.AreEqual(result.Statistics.TestCount, 2);
            Assert.AreEqual(result.Statistics.PassCount, 1);
            Assert.AreEqual(result.Statistics.FailureCount, 1);
            Assert.GreaterThan(result.Statistics.Duration, 0);
            // The assert count is not reliable but we should be fine with simple
            // asserts
            Assert.AreEqual(result.Statistics.AssertCount, 0);
        }

        [Test]
        public void RunSingleTest()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.Assemblies = assemblies;
            cmdlet.Filter = "Type: Gallio.TestResources.MbUnit2.PassingTests & Member: Pass";
            TestLauncherResult result = cmdlet.Execute();
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
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.Assemblies = assemblies;
            cmdlet.Filter = "Type: Gallio.TestResources.MbUnit2.FailingFixture & Member: Fail";
            TestLauncherResult result = cmdlet.Execute();
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
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.Assemblies = assemblies;
            cmdlet.Filter = "Type: Gallio.TestResources.MbUnit2.IgnoredTests";
            TestLauncherResult result = cmdlet.Execute();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ResultCode, ResultCode.Success);
            Assert.AreEqual(result.Statistics.TestCount, 2);
            Assert.AreEqual(result.Statistics.IgnoreCount, 2);
            Assert.GreaterThan(result.Statistics.Duration, 0);
        }

        [Test]
        public void ExecutionFailure()
        {
            InstrumentedGallioCmdlet cmdlet = CreateCmdlet();
            cmdlet.TerminateOnFailure = false;
            cmdlet.HintDirectories = new string[] { null };
            cmdlet.PluginDirectories = new string[] { null };
            TestLauncherResult result = cmdlet.Execute();
            Assert.IsNotNull(result);
        }

        #endregion

        #region Private Methods

        private static InstrumentedGallioCmdlet CreateCmdlet()
        {
            InstrumentedGallioCmdlet cmdlet = new InstrumentedGallioCmdlet();

            return cmdlet;
        }

        #endregion
    }
}
