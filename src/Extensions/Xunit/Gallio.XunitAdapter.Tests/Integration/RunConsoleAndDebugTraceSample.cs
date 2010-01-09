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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.XunitAdapter.TestResources;

namespace Gallio.XunitAdapter.Tests.Integration
{
    [TestFixture]
    [RunSample(typeof(ConsoleAndDebugTraceSample))]
    public class RunConsoleAndDebugTraceSample : BaseTestWithSampleRunner
    {
        [Test]
        public void CapturesConsoleOutput()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ConsoleAndDebugTraceSample).GetMethod("ConsoleOutput")));
            AssertLogContains(run, "Hello", MarkupStreamNames.ConsoleOutput);
        }

        [Test]
        public void CapturesConsoleError()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ConsoleAndDebugTraceSample).GetMethod("ConsoleError")));
            AssertLogContains(run, "Hello", MarkupStreamNames.ConsoleOutput); // xUnit consolidates error and output
            //AssertLogContains(run, "Hello", MarkupStreamNames.ConsoleError);
        }

        /* xUnit disables all debug/trace listeners except its own so it is not possible
         * to capture Debug output right now.
        [Test]
        public void CapturesDebugTrace()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(ConsoleAndDebugTraceSample).GetMethod("DebugTrace")));
            AssertLogContains(run, "Hello", MarkupStreamNames.DebugTrace);
        }
         */
    }
}
