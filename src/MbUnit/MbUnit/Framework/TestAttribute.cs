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
using System.Diagnostics;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies that a method represents a single test case within a fixture.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, if the method throws an unexpected exception,
    /// the test will be deemed to have failed.  Otherwise, the test will pass.
    /// </para>
    /// <para>
    /// The default behavior may be modified by test decorator attributes that
    /// may alter the execution environment of the test, catch and reinterpret
    /// any exceptions it throws, or impose additional constraints upon its execution.
    /// </para>
    /// <para>
    /// Output from the test, such as text written to the console, is captured
    /// by the framework and will be included in the test report.  Additional
    /// information can also be logged during test execution using the <see cref="TestLog" />
    /// class.
    /// </para>
    /// <para>
    /// A test method has a timeout of 10 minutes by default.  This may be changed
    /// using the <see cref="TimeoutAttribute" />.
    /// </para>
    /// <para>
    /// The method to which this attribute is applied must be declared by the
    /// fixture class.  The method may be static.  If it has parameters, then the
    /// test is considered to be data-driven.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = false, Inherited = true)]
    public class TestAttribute : TestMethodPatternAttribute
    {
        // TODO: We should support explicit ordering of tests based on
        //       an Order property similar to decorators.  Then we can deprecate the
        //       TestSequence attribute.

        /// <inheritdoc />
        [DebuggerNonUserCode]
        protected override void Execute(PatternTestInstanceState state)
        {
            string expectedExceptionType = state.TestStep.Metadata.GetValue(MetadataKeys.ExpectedException)
                ?? state.Test.Metadata.GetValue(MetadataKeys.ExpectedException);

            if (expectedExceptionType == null)
            {
                state.InvokeTestMethod();
                return;
            }

            try
            {
                state.InvokeTestMethod();

                using (TestLog.Failures.BeginSection("Expected Exception"))
                    TestLog.Failures.WriteLine("Expected an exception of type '{0}' but none was thrown.", expectedExceptionType);
            }
            catch (Exception ex)
            {
                string expectedExceptionMessage = state.TestStep.Metadata.GetValue(MetadataKeys.ExpectedExceptionMessage)
                    ?? state.Test.Metadata.GetValue(MetadataKeys.ExpectedExceptionMessage);

                Type exceptionType = ex.GetType();

                if (ReflectionUtils.IsAssignableFrom(expectedExceptionType, exceptionType)
                    && (expectedExceptionMessage == null || ex.Message.Contains(expectedExceptionMessage)))
                    return;

                if (ex is TestException)
                    throw;

                using (TestLog.Failures.BeginSection("Expected Exception"))
                {
                    if (expectedExceptionMessage != null)
                    {
                        TestLog.Failures.WriteLine(
                            "Expected an exception of type '{0}' with message substring '{1}' but a different exception was thrown.",
                            expectedExceptionType, expectedExceptionMessage);
                    }
                    else
                    {
                        TestLog.Failures.WriteLine(
                            "Expected an exception of type '{0}' but a different exception was thrown.",
                            expectedExceptionType);
                    }

                    TestLog.Failures.WriteException(ex);
                }
            }

            throw new SilentTestException(TestOutcome.Failed);
        }
    }
}
