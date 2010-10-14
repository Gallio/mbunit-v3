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
    /// Declares that the associated test's <see cref="TestOutcome"/> should be interpreted as having completed 
    /// with a specified <see cref="TestOutcome"/> if a specified exception is thrown during the test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The expected contents of the detected exception's <see cref="Exception.Message"/> may optionally be specified.
    /// </para>
    /// <para>
    /// The <see cref="TestOutcome"/> that is returned when a matching exception is thrown may optionally be specified.
    /// By default, the test will be treated as <see cref="TestOutcome.Error"/> if a matching exception is thrown.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// This C# example shows a test with a <see cref="CatchExceptionAttribute"/> that will 
    /// result in a <see cref="TestOutcome.Error"/> (the default <see cref="Outcome"/>) 
    /// if the test throws a <see cref="NotSupportedException"/>.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This test will have an outcome of <see cref="TestOutcome.Error"/>.
    /// /// </summary>
    /// [Test]
    /// [CatchException(typeof(NotSupportedException))]
    /// public void CatchExceptionExample1()
    /// {
    ///     throw new NotSupportedException();
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows a test with a <see cref="CatchExceptionAttribute"/> that will 
    /// result in a <see cref="TestOutcome.Pending"/> 
    /// if the test throws a <see cref="NotImplementedException"/>.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This test will have an outcome of <see cref="TestOutcome.Pending"/>.
    /// /// </summary>
    /// [Test]
    /// [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Pending")]
    /// public void CatchExceptionExample2()
    /// {
    ///     throw new NotImplementedException();
    /// }
    /// ]]></code>
    /// </para>
    /// <para>
    /// This C# example shows a test with a <see cref="CatchExceptionAttribute"/> that will 
    /// result in a <see cref="TestOutcome.Passed"/> 
    /// if the test throws a <see cref="NotImplementedException"/> 
    /// or if the test doesn't throw any exceptions.
    /// <code><![CDATA[
    /// /// <summary>
    /// /// This test will have an outcome of <see cref="TestOutcome.Passed"/> 
    /// /// whether the test throws a <see cref="NotImplementedException"/> or throws no exception.
    /// /// </summary>
    /// [Test]
    /// [CatchException(typeof(NotImplementedException), StandardOutcome = "TestOutcome.Passed")]
    /// public void CatchExceptionExample3()
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
    /// public void CatchExceptionExample4()
    /// {
    ///     //throw new NotSupportedException(); //outcome would be: new TestOutcome(TestStatus.Skipped, "notsupported")
    ///     throw new NotImplementedException(); //outcome would be: TestOutcome.Pending
    ///     //throw new TimeoutException("A deadlock occurred."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "deadlock")
    ///     //throw new TimeoutException("The server is not responding."); //outcome would be: new TestOutcome(TestStatus.Inconclusive, "timeout")
    ///     //throw new TimeoutException("Transaction timeout."); //outcome would be: TestOutcome.Failed
    ///     //throw new TimeoutException(); //outcome would be: TestOutcome.Failed
    ///     //throw new ArithmeticException(); //outcome would be: TestOutcome.Failed
    ///     //return; //outcome would be: TestOutcome.Passed
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class CatchExceptionAttribute : TestDecoratorPatternAttribute
    {
        private readonly Type exceptionType;
        private string exceptionMessage;
        private TestStatus outcomeStatus = TestOutcome.Error.Status;
        private string outcomeCategory = TestOutcome.Error.Category;

        //TODO: Add the ability to specify how the ExceptionMessage is compared: Contains (the default)ExactMatch, RegEx, etc.
        //private Enum exceptionMessageComparisonMethod = StringComparisonMethod.Contains;

        /// <summary>
        /// Declares that the associated test is expected to throw an <see cref="Exception"/> of
        /// a particular type.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="exceptionType"/> is not the <see cref="Type"/> of an <see cref="Exception"/>.</exception>
        public CatchExceptionAttribute(Type exceptionType)
            : this(exceptionType, TestOutcome.Error.Status, TestOutcome.Error.Category)
        {
        }

        /// <summary>
        /// Declares that the associated test is expected to throw an <see cref="Exception"/> of
        /// a particular type, and the <see cref="TestStatus"/> of the <see cref="TestOutcome"/> 
        /// that should be used if/when it does.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="outcomeStatus">
        /// The <see cref="TestStatus"/> that will be used to construct the 
        /// <see cref="TestOutcome"/> if a detected exception is handled.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="exceptionType"/> is not the <see cref="Type"/> of an <see cref="Exception"/>.</exception>
        public CatchExceptionAttribute(Type exceptionType, TestStatus outcomeStatus)
            : this(exceptionType, outcomeStatus, TestOutcome.Error.Category)
        {
        }

        /// <summary>
        /// Declares that the associated test is expected to throw an <see cref="Exception"/> of
        /// a particular type, and the <see cref="TestOutcome"/> 
        /// that should be used if/when it does.
        /// </summary>
        /// <param name="exceptionType">The expected exception type.</param>
        /// <param name="outcomeStatus">
        /// The <see cref="TestStatus"/> that will be used to construct the 
        /// <see cref="TestOutcome"/> if a detected exception is handled.
        /// </param>
        /// <param name="outcomeCategory">
        /// The <see cref="TestOutcome.Category"/> that will be used to construct the 
        /// <see cref="TestOutcome"/> if a detected exception is handled.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="exceptionType"/> is not the <see cref="Type"/> of an <see cref="Exception"/>.</exception>
        public CatchExceptionAttribute(Type exceptionType, TestStatus outcomeStatus, string outcomeCategory)
        {
            if (exceptionType == null)
                throw new ArgumentNullException("exceptionType");
            if (!typeof(Exception).IsAssignableFrom(exceptionType))
                throw new ArgumentException(string.Format("Type '{0}' is not an Exception Type.", exceptionType.FullName), "exceptionType");

            this.exceptionType = exceptionType;
            this.outcomeStatus = outcomeStatus;
            this.outcomeCategory = outcomeCategory;
        }

        /// <summary>
        /// Gets the type of <see cref="Exception"/> that is handled.
        /// </summary>
        public Type ExceptionType
        {
            get { return exceptionType; }
        }

        /// <summary>
        /// Gets or sets the substring that must be contained in the detected <see cref="Exception"/>'s <see cref="Exception.Message"/> for it to be handled, 
        /// or <c>null</c> if none specified.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TestOutcome.Category"/> that will be used to construct the 
        /// <see cref="TestOutcome"/> when a detected exception is handled.
        /// </summary>
        /// <see cref="Outcome"/>
        /// <see cref="OutcomeCategory"/>
        public TestStatus OutcomeStatus
        {
            get { return outcomeStatus; }
            set { outcomeStatus = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TestOutcome.Category"/> that will be used to construct the 
        /// <see cref="TestOutcome"/> if a detected exception is handled.
        /// </summary>
        /// <see cref="Outcome"/>
        /// <see cref="OutcomeStatus"/>
        public string OutcomeCategory
        {
            get { return outcomeCategory; }
            set { outcomeCategory = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="TestOutcome"/> that will be used if a matching exception is thrown.
        /// Defaults to <see cref="TestOutcome.Error"/>.
        /// </summary>
        public TestOutcome Outcome
        {
            get { return new TestOutcome(outcomeStatus, outcomeCategory); }
            set
            {
                OutcomeStatus = value.Status;
                OutcomeCategory = value.Category;
            }
        }

        /// <summary>
        /// This is a <see cref="string"/>-typed alias of the <see cref="Outcome"/> property.
        /// </summary>
        public string StandardOutcome
        {
            get { return Outcome.DisplayName; }
            set { Outcome = TestOutcome.GetStandardOutcome(value); }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("CatchExceptionAttribute(ExceptionType={0}{1}, OutcomeStatus=TestStatus.{2}{3}{4})",
                ExceptionType.FullName,
                ((Order == 0) ? "" : string.Format(", Order={0}", Order)),
                OutcomeStatus,
                ((OutcomeCategory == null) ? "" : string.Format(", OutcomeCategory=\"{0}\"", OutcomeCategory.Replace("\"", "\\\""))),
                ((ExceptionMessage == null) ? "" : string.Format(", ExceptionMessage=\"{0}\"", ExceptionMessage.Replace("\"", "\\\"")))
                );
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            ActionChain<PatternTestInstanceState> executeTestInstanceChain = scope.TestBuilder.TestInstanceActions.ExecuteTestInstanceChain;
            executeTestInstanceChain.Around((state, inner) => ExecuteTestInstanceChainCatchExceptionDecorator(inner, state));
        }

        /// <summary>
        /// This method does the actual work of catching and handling (or not) <see cref="Exception"/>s.
        /// </summary>
        private void ExecuteTestInstanceChainCatchExceptionDecorator(Action<PatternTestInstanceState> inner, PatternTestInstanceState state)
        {
            try
            {
                inner(state);
                OnInnerActionCompleted(null);
            }
            catch (TestException ex)
            {
                //If the exception is a TestException it must be rethrown 
                //because it represents the (non-Passed) TestOutcome of the inner action

                //TODO: Review: Should we call ExceptionUtils.RethrowWithNoStackTraceLoss(ex) here?
                ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                throw;
            }
            catch (Exception ex)
            {
                //NOTE: Ideally, the whole catch block would be implemented inside of <see cref="OnInnerActionCompleted"/>.
                //However, that is not possible for technical reasons.
                //See the Implementation Note in the remarks section of the <see cref="OnInnerActionCompleted"/>
                //documentation for details.

                if (OnInnerActionCompleted(ex))
                {
                    return;
                }
                else
                {
                    //TODO: Review: Should we call ExceptionUtils.RethrowWithNoStackTraceLoss(ex) here?
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Performs the work of analysing (and either handling or rethrowing) a detected exception.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method could be overridden by inheritors to extend or modify the behavior 
        /// associated with detecting <see cref="Exception"/>s.
        /// <para>
        /// Theoretically, the <see cref="ExpectedExceptionAttribute"/> could be implemented as a subclass of 
        /// <see cref="CatchExceptionAttribute"/> that overrides the behavior of 
        /// <see cref="OnInnerActionCompleted(System.Exception,bool)"/> to always return <c>true</c>.
        /// </para>
        /// </para>
        /// <para>
        /// Implementation Note: Normally this method would have a return type of <c>void</c>. 
        /// However, since it is not possible rethrow a caught exception using "throw;" 
        /// except from directly within the <c>catch</c> block 
        /// (i.e. the same method the <c>catch</c> block is declared in),
        /// it was necessary to use the returned value to indicate to the calling method whether or not the 
        /// detected <see cref="Exception"/> should be re-thrown.
        /// </para>
        /// </remarks>
        /// <param name="ex">The detected <see cref="Exception"/>.</param>
        /// <param name="isExpected">Indicates whether <paramref name="ex"/> matches the catch-criteria of the instance.</param>
        /// <returns>
        /// <c>true</c> to indicate that the <see cref="TestOutcome"/> should be <see cref="TestOutcome.Passed"/> 
        /// (e.g. when <paramref name="isExpected"/> is <c>true</c>).
        /// Else <c>false</c> to indicate that the detected exception (if any) was not handled 
        /// and should be rethrown (if needed).
        /// </returns>
        /// <exception cref="SilentTestException">
        /// Propogated when thrown by <see cref="HandleExpectedException"/> to indicate that the detected exception was handled, 
        /// and that the <see cref="TestOutcome"/> should be something other than <see cref="TestOutcome.Passed"/>.
        /// </exception>
        protected virtual bool OnInnerActionCompleted(Exception ex, bool isExpected)
        {
            if (isExpected)
            {
                HandleExpectedException(ex);

                //Returning <c>true</c> signifies a <see cref="TestOutcome.Passed"/> outcome.
                //Any other outcome will be "returned" as a <see cref="TestException"/> thrown by <see cref="HandleExpectedException"/>
                return true;
            }
            else if (ex == null)
            {
                //We also want to treat a successful inner action (i.e. one that didn't throw an exception) as Passed
                return true;
            }
            return false;
        }

        private bool OnInnerActionCompleted(Exception ex)
        {
            return OnInnerActionCompleted(ex, IsExpectedException(ex));
        }

        /// <summary>
        /// Indicates whether a detected <see cref="Exception"/> (or the lack of one) is expected, 
        /// and should be caught and handled (by <see cref="HandleExpectedException"/>) 
        /// based upon the instance's matching properties.
        /// </summary>
        /// <seealso cref="ExceptionType"/>
        /// <seealso cref="ExceptionMessage"/>
        /// <seealso cref="OnInnerActionCompleted(System.Exception,bool)"/>
        protected bool IsExpectedException(Exception ex)
        {
            //NOTE: Much of this logic was adapted from the ExpectedException logic in <see cref="TestAttribute.Execute"/>
            return (ex != null)
                && (ReflectionUtils.IsAssignableFrom(exceptionType.FullName, ex.GetType())
                && (exceptionMessage == null || ex.Message.Contains(exceptionMessage)));
        }

        /// <summary>
        /// Performs the work of handling a detected exception that matches the catching-criteria.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method could be overridden by inheritors to extend or modify the behavior 
        /// associated with handling <see cref="Exception"/>s that match the configured catching-criteria.
        /// </para>
        /// </remarks>
        /// <param name="ex">The detected <see cref="Exception"/> that matches the catching-criteria.</param>
        /// <exception cref="SilentTestException">
        /// Thrown to return a non-<see cref="TestOutcome.Passed"/> outcome back up the call stack.
        /// </exception>
        protected virtual void HandleExpectedException(Exception ex)
        {
            if (ex == null)
            {
                //throw new ArgumentNullException("ex");
                return;
            }

            string testExceptionMessage = string.Format("An exception was handled by a {0}.", this.ToString());
            string exceptionSummary = string.Format("{0}, {1}.",
                ex.GetType().FullName,
                ((ex.Message == null) ? "null" : string.Format("\"{0}\"", ex.Message.Replace("\"", "\\\""))));
            TestLog.WriteException(ex, string.Format("Caught exception. {0}", testExceptionMessage));
            //throw new SilentTestException(Outcome,
            //    string.Format("{0} Exception: {1}", testExceptionMessage, exceptionSummary));

            TestOutcome outcome = Outcome;
            bool outcomeRequiresTestException = (outcome != TestOutcome.Passed);
            if (outcomeRequiresTestException)
            {
                throw new SilentTestException(outcome,
                    string.Format("{0} Exception: {1}", testExceptionMessage, exceptionSummary));
            }
            else
            {
                //simply return without throwing an exception to signify a "passed" outcome
                return;
            }
        }

    }
}
