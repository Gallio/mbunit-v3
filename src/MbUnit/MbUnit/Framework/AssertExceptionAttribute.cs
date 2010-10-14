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
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that the associated test's <see cref="TestOutcome"/> should be 
    /// <see cref="TestOutcome.Passed"/> 
    /// only if a specified exception is thrown during the test.
    /// Otherwise, the test's outcome should be <see cref="TestOutcome.Failed"/> 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The expected contents of the detected exception's <see cref="Exception.Message"/> may optionally be specified.
    /// </para>
    /// <para>
    /// The <see cref="TestOutcome"/> that is returned when a matching exception is thrown may optionally be specified.
    /// By default, the test will be treated as <see cref="TestOutcome.Passed"/> if a matching exception is thrown.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This C# example shows a test with a <see cref="AssertExceptionAttribute"/> that will 
    /// result in a <see cref="TestOutcome.Passed"/> 
    /// when the test throws a <see cref="NotSupportedException"/>.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This test will have an outcome of <see cref="TestOutcome.Passed"/>.
    /// /// </summary>
    /// [Test]
    /// [AssertException(typeof(NotSupportedException))]
    /// public void AssertExceptionExample1()
    /// {
    ///     throw new NotSupportedException();
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows a test with a <see cref="AssertExceptionAttribute"/> that will 
    /// result in a <see cref="TestOutcome.Failed"/> 
    /// unless the test throws a <see cref="NotSupportedException"/>.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This test will have an outcome of <see cref="TestOutcome.Failed"/> 
    /// /// whether the test throws a <see cref="NotImplementedException"/> or throws no exception.
    /// /// </summary>
    /// [Test]
    /// [AssertException(typeof(NotSupportedException))]
    /// public void AssertExceptionExample2()
    /// {
    ///     bool randomBool = new Random().Next() % 2 == 0;
    ///     if (randomBool)
    ///         throw new NotImplementedException();
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows a test with multiple <see cref="CatchExceptionAttribute"/>s that will: 
    /// result in a <see cref="TestOutcome"/> of <see cref="TestOutcome.Pending"/> 
    /// if the test throws a <see cref="NotImplementedException"/>;
    /// result in a <see cref="TestOutcome"/> with a 
    /// <see cref="TestOutcome.Status"/> of <see cref="TestStatus.Skipped"/> and a 
    /// <see cref="TestOutcome.Category"/> of <c>"notsupported"</c> 
    /// if the test throws a <see cref="NotSupportedException"/>;
    /// result in a <see cref="TestOutcome"/> with a 
    /// <see cref="TestOutcome.Status"/> of <see cref="TestStatus.Inconclusive"/> and a 
    /// <see cref="TestOutcome.Category"/> of <c>"deadlock"</c> 
    /// if the test throws a <see cref="TimeoutException"/> 
    /// with a <see cref="Exception.Message"/> that contains the substring <c>"deadlock"</c>;
    /// result in a <see cref="TestOutcome"/> with a 
    /// <see cref="TestOutcome.Status"/> of <see cref="TestStatus.Inconclusive"/> and a 
    /// <see cref="TestOutcome.Category"/> of <c>"timeout"</c> 
    /// if the test throws a <see cref="TimeoutException"/> 
    /// with a <see cref="Exception.Message"/> that contains the substring <c>"server not responding"</c>;
    /// result in a <see cref="TestOutcome"/> of <see cref="TestOutcome.Failed"/> 
    /// if the test throws any other <see cref="Exception"/> not listed above;
    /// result in a <see cref="TestOutcome"/> of <see cref="TestOutcome.Passed"/> 
    /// if the test does not throw any <see cref="Exception"/>s.
    /// <code><![CDATA[
    /// /// <remarks>
    /// /// This test can have any of several outcomes depending on what type of exception is thrown (if any) during test execution.
    /// /// </remarks>
    /// [Test]
    /// [CatchException(typeof(NotSupportedException), OutcomeStatus = TestStatus.Skipped, OutcomeCategory = "notsupported")]
    /// [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending")]
    /// [CatchException(typeof(TimeoutException), ExceptionMessage = "deadlock", OutcomeStatus = TestStatus.Inconclusive, OutcomeCategory = "deadlock", Order = 2)]
    /// [CatchException(typeof(TimeoutException), ExceptionMessage = "server not responding", OutcomeStatus = TestStatus.Inconclusive, OutcomeCategory = "timeout", Order = 3)]
    /// [AssertException(typeof(ArithmeticException), Order = 4)]
    /// public void AssertExceptionExample3()
    /// {
    ///     //throw new NotSupportedException(); //outcome would be: new TestOutcome(TestStatus.Skipped, "notsupported")
    ///     throw new NotImplementedException(); //outcome would be: TestOutcome.Pending
    ///     //throw new TimeoutException("A deadlock occurred."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "deadlock")
    ///     //throw new TimeoutException("The server is not responding."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "timeout")
    ///     //throw new TimeoutException("Transaction timeout."); //outcome would be: TestOutcome.Failed
    ///     //throw new TimeoutException(); //outcome would be: TestOutcome.Failed
    ///     //throw new ArithmeticException(); //outcome would be: TestOutcome.Passed
    ///     //return; //outcome would be: TestOutcome.Failed
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class AssertExceptionAttribute : CatchExceptionAttribute
    {

        /// <summary>
        /// Declares that the associated test is expected to throw an <see cref="Exception"/> of
        /// a particular type.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="exceptionType"/> is not the <see cref="Type"/> of an <see cref="Exception"/>.</exception>
        public AssertExceptionAttribute(Type exceptionType)
            : base(exceptionType, TestOutcome.Passed.Status, TestOutcome.Passed.Category)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("AssertExceptionAttribute(ExceptionType={0}{1}{2})",
                ExceptionType.FullName,
                ((Order == 0) ? "" : string.Format(", Order={0}", Order)),
                ((ExceptionMessage == null) ? "" : string.Format(", ExceptionMessage=\"{0}\"", ExceptionMessage.Replace("\"", "\\\"")))
                );
        }

        /// <inheritdoc />
        protected override bool OnInnerActionCompleted(Exception ex, bool isExpected)
        {
            if ((ex == null) || (!isExpected))
            {
                //NOTE: Instead of returning false to indicate that the exception should be rethrown 
                //(as CatchExceptionAttribute does) 
                //when a detected exception doesn't match the catch-criteria,
                //we will still "handle" the "unexpected" exception, 
                //but in a different way than for a "handled" exception.
                HandleUnexpectedException(ex);
                return true; //we handled the unexpected exception
            }
            else
            {
                //NOTE: We call the base implementation to inherit it's logging behavior
                return base.OnInnerActionCompleted(ex, isExpected);
            }

            //execution should never reach this code, but just in case...
            throw new SilentTestException(TestOutcome.Error);
        }

        /// <summary>
        /// Performs the work of handling a detected exception that does not match the catching-criteria.
        /// Also performs any work related to handling the scenario where an exception is not detected.
        /// </summary>
        /// <param name="ex"><c>null</c>, or the detected <see cref="Exception"/> that does not match the catching-criteria.</param>
        /// <exception cref="SilentTestException">
        /// Thrown to return a non-<see cref="TestOutcome.Passed"/> outcome back up the call stack.
        /// </exception>
        /// <seealso cref="CatchExceptionAttribute.HandleExpectedException"/>
        protected void HandleUnexpectedException(Exception ex)
        {
            if (ex == null)
            {
                using (TestLog.Failures.BeginSection("AssertException Failed"))
                {
                    TestLog.Failures.WriteLine("Expected an exception of type '{0}' but none was thrown.", ExceptionType);
                }
            }
            else
            {
                using (TestLog.Failures.BeginSection("AssertException Failed"))
                {
                    if (ExceptionMessage != null)
                    {
                        TestLog.Failures.WriteLine(
                            "Expected an exception of type '{0}' with message substring '{1}' but a different exception was thrown.",
                            ExceptionType, ExceptionMessage);
                    }
                    else
                    {
                        TestLog.Failures.WriteLine(
                            "Expected an exception of type '{0}' but a different exception was thrown.",
                            ExceptionType);
                    }

                    TestLog.Failures.WriteException(ex);
                }
            }

            throw new SilentTestException(TestOutcome.Failed);
        }

    }
}
