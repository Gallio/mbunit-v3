// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Common.Reflection;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Pattern
{
    /// <summary>
    /// This test verifies that the pattern test framework limits the total
    /// stack depth for execution.  This is in consideration of the fact that
    /// the Visual Studio debugger has serious performance issues single-stepping
    /// through code when the stack depth is high.  Previously the pattern
    /// test framework would regularly have a stack depth well over 150 levels!
    /// Now we have specifically inlined certain methods and changed control structures
    /// (such as using IDisposable blocks and using statements instead of lambdas) to
    /// minimize stack depth.  The code is a bit uglier but performance is significantly
    /// better during debugging.
    /// </summary>
    [RunSample(typeof(StackDepthSample))]
    public class PatternTestFrameworkStackDepthTest : BaseTestWithSampleRunner
    {
        // acceptable depth is based on the best measured depth plus a little slack for runtime variations
        [Test]
        [Row(typeof(StackDepthSample), "SimpleTest", 32 + 3)]
        [Row(typeof(StackDepthSample), "DataDrivenTest", 32 + 3)]
        [Row(typeof(StackDepthSample), "DecoratedTest", 43 + 3)]
        [Row(typeof(StackDepthSample.Nested), "NestedTest", 34 + 3)]
        public void StackDepthIsBounded(Type sampleType, string methodName, int maxAcceptableStackDepth)
        {
            var primaryTestStepRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(sampleType.GetMethod(methodName)));

            StringBuilder combinedLogFromAllSteps = new StringBuilder();
            foreach (var run in primaryTestStepRun.AllTestStepRuns)
                combinedLogFromAllSteps.Append(run.TestLog.ToString());

            Match match = Regex.Match(combinedLogFromAllSteps.ToString(), "Stack depth: ([0-9]+)");
            int stackDepth = int.Parse(match.Groups[1].Value);
            Assert.LessThan(stackDepth, maxAcceptableStackDepth, "Actual stack depth was greater than acceptable threshold.");
        }

        /// <summary>
        /// With the debugger attached, step-over manually through all trials of this test as quickly
        /// as possible from BEGIN to END to time how long it takes to progress.  Write this number
        /// down and compare it across multiple runs to observe how optimizing stack depth has
        /// helped or hindered.
        /// </summary>
        [Test, Explicit("Manual timing test.")]
        public void ManualSingleStepTime()
        {
            const int trials = 5;
            const int stepsPerTrial = 5;

            TimeSpan totalElapsed = TimeSpan.Zero;
            for (int trial = 1; trial <= trials; trial++)
            {
                // --BEGIN--
                Stopwatch stopwatch = Stopwatch.StartNew();
                totalElapsed += TimeSpan.Zero; // waste time
                totalElapsed += TimeSpan.Zero; // waste time
                totalElapsed += TimeSpan.Zero; // waste time
                totalElapsed += stopwatch.Elapsed;
                // --END--
            }

            TestLog.WriteLine("Time per single-step: approx. {0}ms.", totalElapsed.TotalMilliseconds / trials / stepsPerTrial);
        }

        [Explicit("Sample")]
        public class StackDepthSample
        {
            private static void PrintStackInfo()
            {
                StackTrace trace = new StackTrace(true);
                TestLog.WriteLine("Stack depth: {0}", trace.FrameCount);
                TestLog.WriteLine(trace);
            }

            [Test]
            public void SimpleTest()
            {
                PrintStackInfo();
            }

            [Test]
            [Row(0)]
            public void DataDrivenTest(int value)
            {
                PrintStackInfo();
            }

            [Test]
            [Repeat(1)]
            public void DecoratedTest()
            {
                PrintStackInfo();
            }

            public class Nested
            {
                [Test]
                public void NestedTest()
                {
                    PrintStackInfo();
                }
            }
        }
    }
}
