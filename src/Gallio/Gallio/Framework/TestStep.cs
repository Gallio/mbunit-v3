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
using System.Runtime.CompilerServices;
using Gallio;
using Gallio.Common;
using Gallio.Framework.Assertions;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// Provides functions for manipulating test steps.
    /// </para>
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
    /// to the framework's native testing primitives.</item>
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
    /// Running as many iterations of a test as possible for a set maximum duration.
    /// Each iteration will run as a separate test step so that it has its own test
    /// execution log and test outcome included in the test report.
    /// <code>
    /// [Test]
    /// public void MeasurePerformance()
    /// {
    ///     // Warm up.
    ///     TestStep.RunStepAndVerify("Warm Up", DoSomething, TestOutcome.Passed);
    /// 
    ///     // Run as many iterations as possible for 10 seconds.
    ///     int iterations = 0;
    ///     Stopwatch stopwatch = Stopwatch.StartNew();
    ///     while (stopwatch.ElapsedMilliseconds &lt; 10*1000)
    ///     {
    ///         iterations += 1;
    ///         TestStep.RunStepAndVerify("Iteration #" + i, DoSomething, TestOutcome.Passed);
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
    [SystemInternal]
    public static class TestStep
    {
        /// <summary>
        /// Gets reflection information about the current step.
        /// </summary>
        public static TestStepInfo CurrentStep
        {
            get { return TestContext.CurrentContext.TestStep; }
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates
        /// it with the calling function.  Does not verify the outcome of the step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method does not verify that the test step completed successfully.  Check the
        /// <see cref="TestContext.Outcome" /> of the test step or call <see cref="RunStepAndVerifyOutcome(string, Action, TestOutcome)"/>
        /// to ensure that the expected outcome was obtained.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TestContext RunStep(string name, Action action)
        {
            return TestContext.CurrentContext.RunStep(name, action, null, false, Reflector.GetCallingFunction());
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates
        /// it with the calling function.  Does not verify the outcome of the step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method does not verify that the test step completed successfully.  Check the
        /// <see cref="TestContext.Outcome" /> of the test step or call <see cref="RunStepAndVerifyOutcome(string, Action, TimeSpan?, TestOutcome)"/>
        /// to ensure that the expected outcome was obtained.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="timeout">The step execution timeout, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative.</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TestContext RunStep(string name, Action action, TimeSpan? timeout)
        {
            return TestContext.CurrentContext.RunStep(name, action, timeout, false, Reflector.GetCallingFunction());
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates it
        /// with the specified code reference.  Does not verify the outcome of the step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method does not verify that the test step completed successfully.  Check the
        /// <see cref="TestContext.Outcome" /> of the test step or call <see cref="RunStepAndVerifyOutcome(string, Action, TimeSpan?, bool, ICodeElementInfo, TestOutcome)"/>
        /// to ensure that the expected outcome was obtained.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="timeout">The step execution timeout, or null if none.</param>
        /// <param name="isTestCase">True if the step represents an independent test case.</param>
        /// <param name="codeElement">The associated code element, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative.</exception>
        public static TestContext RunStep(string name, Action action, TimeSpan? timeout, bool isTestCase, ICodeElementInfo codeElement)
        {
            return TestContext.CurrentContext.RunStep(name, action, timeout, isTestCase, codeElement);
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates
        /// it with the calling function.  Verifies that the step produced the expected outcome.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method verifies that the step produced the expected outcome.  If a different outcome
        /// was obtained, then raises an assertion failure.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="expectedOutcome">The expected outcome of the step.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="AssertionFailureException">Thrown if the expected outcome was not obtained.</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TestContext RunStepAndVerifyOutcome(string name, Action action, TestOutcome expectedOutcome)
        {
            return TestContext.CurrentContext.RunStepAndVerifyOutcome(name, action, null, false, Reflector.GetCallingFunction(), expectedOutcome);
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates
        /// it with the calling function.  Verifies that the step produced the expected outcome.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method verifies that the step produced the expected outcome.  If a different outcome
        /// was obtained, then raises an assertion failure.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="timeout">The step execution timeout, or null if none.</param>
        /// <param name="expectedOutcome">The expected outcome of the step.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="AssertionFailureException">Thrown if the expected outcome was not obtained.</exception>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TestContext RunStepAndVerifyOutcome(string name, Action action, TimeSpan? timeout, TestOutcome expectedOutcome)
        {
            return TestContext.CurrentContext.RunStepAndVerifyOutcome(name, action, timeout, false, Reflector.GetCallingFunction(), expectedOutcome);
        }

        /// <summary>
        /// Performs an action as a new step within the current context and associates it
        /// with the specified code reference.  Verifies that the step produced the expected outcome.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method creates a new child context with a new nested <see cref="ITestStep" />,
        /// enters the child context, performs the action, then exits the child context.
        /// </para>
        /// <para>
        /// This method may be called recursively to create nested steps or concurrently
        /// to create parallel steps.
        /// </para>
        /// <para>
        /// This method verifies that the step produced the expected outcome.  If a different outcome
        /// was obtained, then raises an assertion failure.
        /// </para>
        /// </remarks>
        /// <param name="name">The name of the step.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="timeout">The step execution timeout, or null if none.</param>
        /// <param name="isTestCase">True if the step represents an independent test case.</param>
        /// <param name="codeElement">The associated code element, or null if none.</param>
        /// <param name="expectedOutcome">The expected outcome of the step.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or
        /// <paramref name="action"/> is null.</exception>
        /// <returns>The context of the step that ran.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is negative.</exception>
        /// <exception cref="AssertionFailureException">Thrown if the expected outcome was not obtained.</exception>
        public static TestContext RunStepAndVerifyOutcome(string name, Action action, TimeSpan? timeout, bool isTestCase, ICodeElementInfo codeElement, TestOutcome expectedOutcome)
        {
            return TestContext.CurrentContext.RunStepAndVerifyOutcome(name, action, timeout, isTestCase, codeElement, expectedOutcome);
        }

        /// <summary>
        /// Adds metadata to the step that is running in the context.
        /// </summary>
        /// <param name="metadataKey">The metadata key.</param>
        /// <param name="metadataValue">The metadata value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null.</exception>
        public static void AddMetadata(string metadataKey, string metadataValue)
        {
            TestContext.CurrentContext.AddMetadata(metadataKey, metadataValue);
        }
    }
}