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
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// A step is a delimited region of a test.  Each step appears in the report as
    /// if it were a dynamically generated test nested within the body of the test
    /// (or some other step) that spawned it.  The step has its own execution log,
    /// pass/fail/inconclusive result and in all other respects behaves much like
    /// an ordinary test would.
    /// </para>
    /// <para>
    /// The number of steps within a test does not need to be known ahead of time.
    /// This can be useful in situations where insufficient information is known
    /// about the internal structure of a test to be able to fully populate the
    /// test tree will all of its details.  Because steps are dynamically generated
    /// at runtime, they appear in test reports but they are invisible to test runners.
    /// that traverse the test tree.
    /// </para>
    /// <para>
    /// There are many interesting uses for steps.  For example:
    /// <list type="bullet">
    /// <item>A single test consisting of a long sequence of actions can be subdivided
    /// into steps to simplify analysis.</item>
    /// <item>A test might depend on environmental configuration that cannot be
    /// known a priori.</item>
    /// <item>A performance test might be scheduled to run for a certain duration
    /// but the total number of iterations is unknown.  By running each iteration
    /// as a step within a single test, the test report can display the execution
    /// log and pass/fail result of each iteration independently of the others.</item>
    /// <item>A script-driven test driver could execute a scripted sequence of verification
    /// commands as a distinct step.  If the script is written in a general purpose
    /// programming language, the total number of commands and the order in which they
    /// will be performed might not be known ahead of time.  Using steps enables the
    /// integration of tests written in forms that cannot be directly adapted
    /// to MbUnit's testing primitives.</item>
    /// <item>When testing non-deterministic algorithms, it is sometimes useful to repeat
    /// a test multiple times under slightly different conditions until a certain level
    /// of confidence is reached.  The variety of conditions tested might be determined
    /// adaptively based on an error estimation metric.  Using steps each condition
    /// verified can be reported independently.</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <example>
    /// <para>
    /// Running a test for a set maximum duration.
    /// <code>
    /// [Test]
    /// public void MeasurePerformance()
    /// {
    ///     // Warm up.
    ///     Step.Run("Warm Up", DoSomething);
    /// 
    ///     // Run as many iterations as possible for 10 seconds.
    ///     int iterations = 0;
    ///     Stopwatch stopwatch = Stopwatch.StartNew();
    ///     while (stopwatch.ElapsedMilliseconds &lt; 10*1000)
    ///     {
    ///         iterations += 1;
    ///         Step.Run("Iteration #" + i, DoSomething);
    ///     }
    /// 
    ///     double iterationsPerSecond = iterations * 1000.0 / stopwatch.ElapsedMilliseconds;
    ///     Assert.GreaterEqualThan(iterationsPerSecond, 5, "Unacceptable performance.");
    /// }
    /// 
    /// private void DoSomething()
    /// {
    ///     objectUnderTest.Wibble();
    /// }
    /// </code>
    /// </para>
    /// </example>
    /// <todo author="jeff">
    /// Support running other test fixtures and tests as nested steps.
    /// eg. Step.RunTestFixture("Name", typeof(SomeFixture));
    /// </todo>
    public static class Step
    {
        /// <summary>
        /// Gets the current step.
        /// </summary>
        public static IStep CurrentStep
        {
            get { return Context.CurrentStep; }
        }

        /// <summary>
        /// Runs a block of code as a step.
        /// </summary>
        /// <remarks>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="block">The block of code to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="block"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        public static void Run(string name, Block block)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Name must not be empty.", "name");
            if (block == null)
                throw new ArgumentNullException("block");

            throw new NotImplementedException();
        }
    }
}
