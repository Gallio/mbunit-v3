// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text.RegularExpressions;
using Gallio.Common.Policies;
using Gallio.MSTestAdapter.TestResources;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;

namespace Gallio.MSTestAdapter.Tests.Integration
{
    [TestFixture]
    [RunSample(typeof(EnvironmentSetupSample))]
    public class EnvironmentSetupTest : MSTestIntegrationTest
    {
        private string tempDir = SpecialPathPolicy.For("MSTestAdapter").GetTempDirectory().FullName;

        // Currently the code base, app base and config file may refer to
        // resources that are not in the deployed test dir.  This does not
        // seem to be too problematic for most uses.  -- Jeff.
#if false
        [Test]
        public void CodeBase_ShouldBeDeployedAssembly()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(EnvironmentSetupSample).GetMethod("WriteDiagnostics")));
            Assert.Like(run.TestLog.ToString(),
                "^CodeBase: " + Regex.Escape(new Uri(tempDir).ToString()) + "/.*/TestDir/Out/Gallio.MSTestAdapter.TestResources.dll$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        [Test]
        public void AppBase_ShouldBeDeployedTestDirOut()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(EnvironmentSetupSample).GetMethod("WriteDiagnostics")));
            Assert.Like(run.TestLog.ToString(),
                "^AppBase: " + Regex.Escape(tempDir) + @"\\.*\\TestDir\\Out$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
#endif

        [Test]
        public void CurrentDir_ShouldBeDeployedTestDirOut()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(EnvironmentSetupSample).GetMethod("WriteDiagnostics")));
            Assert.Like(run.TestLog.ToString(),
                "^CurrentDir: " + Regex.Escape(tempDir) + @"\\.*\\TestDir\\Out$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }

        [Test]
        public void TestDir_ShouldBeDeployedTestDir()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(EnvironmentSetupSample).GetMethod("WriteDiagnostics")));
            Assert.Like(run.TestLog.ToString(),
                "^TestDir: " + Regex.Escape(tempDir) + @"\\.*\\TestDir$",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
    }
}
