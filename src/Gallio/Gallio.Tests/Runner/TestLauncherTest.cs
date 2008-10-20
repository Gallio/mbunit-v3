// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using MbUnit.Framework;
using Gallio.Runtime.Logging;

namespace Gallio.Tests.Runner
{
    [TestFixture]
    [TestsOn(typeof(TestLauncher))]
    [Author("Julian", "jhidalgo@mercatus.cl")]
    [Annotation(AnnotationType.Warning, "These tests are almost useless.  Fixme.")]
    public class TestLauncherTest
    {
        private TestLauncher launcher;

        [SetUp]
        public void SetUp()
        {
            launcher = new TestLauncher();
        }

        [Test]
        public void Logger_DefaultIsNullLogger()
        {
            Assert.AreEqual(NullLogger.Instance, launcher.Logger); 
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Logger_ThrowsIfNull()
        {
            launcher.Logger = null;
        }

        [Test]
        public void ProgressMonitorProvider_DefaultIsNullProgressMonitorProvider()
        {
            Assert.AreEqual(NullProgressMonitorProvider.Instance, launcher.ProgressMonitorProvider);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProgressMonitorProvider_ThrowsIfNull()
        {
            launcher.ProgressMonitorProvider = null;
        }

        [Test]
        public void Filter_DefaultIsAnyFilter()
        {
            Assert.IsInstanceOfType(typeof(AnyFilter<ITest>), launcher.TestExecutionOptions.Filter);
        }

        [Test]
        public void TestRunnerName()
        {
            Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, launcher.TestRunnerFactoryName);
        }
    }
}
