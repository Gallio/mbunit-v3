using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
