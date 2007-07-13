using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using MbUnit.Core;
using MbUnit.Core.Runtime;
using MbUnit.Core.Services;
using MbUnit.Core.Services.Assert;
using MbUnit.Core.Utilities;
using MbUnit.Core.Exceptions;
using System.Globalization;

namespace MbUnit.Framework
{
    /// <summary>
    /// The assert class contains numerous procedures for evaluating assertion
    /// conditions.  Normally an assertion failure results in a <see cref="AssertionException" />
    /// being thrown however within the bounds of an <see cref="Assert.Multiple" /> block
    /// multiple assertion failures belonging to a common logical unit may be gathered
    /// without any such exceptions being thrown.
    /// </summary>
    public static class Assert
    {
        /// <summary>
        /// Gets the assertion service for the current context.
        /// </summary>
        public static IAssertionService AssertionService
        {
            get { return Runtime.Instance.AssertionService; }
        }

        #region Override Equals and ReferenceEquals for safety
        /// <summary>
        /// Always yields an assertion failure.
        /// This is done to ensure there is no mistake by calling this function
        /// instead of <see cref="Assert.AreEqual"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new void Equals(object a, object b)
        {
            Assert.Fail("Do not call Assert.Equals.  Use Assert.AreEquals instead.");
        }

        /// <summary>
        /// Always yields an assertion failure.
        /// This is done to ensure there is no mistake by calling this function
        /// instead of <see cref="Assert.AreSame"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static new void ReferenceEquals(object a, object b)
        {
            Assert.Fail("Do not call Assert.ReferenceEquals.  Use Assert.AreSame instead.");
        }
        #endregion


        #region Primitives
        /// <summary>
        /// Runs a block of code in a certain context and collects the assertion results
        /// that were passed to <see cref="Verify" /> within the block.
        /// </summary>
        /// <param name="block">The code block to run</param>
        /// <param name="allowMultipleFailures">If true, allows multiple failures to occur
        /// and collects them all.  Otherwise aborts execution of the code block (by throwing
        /// <see cref="AssertionException" /> when an assertion failure occurs and then
        /// later catching that exception).</param>
        /// <returns>The array of assertions that were verified, may be zero or more</returns>
        public static AssertionResult[] Run(Block block, bool allowMultipleFailures)
        {
            return AssertionService.Run(Context.CurrentContext, block, allowMultipleFailures);
        }

        /// <summary>
        /// Evaluates an assertion condition and returns its result.
        /// </summary>
        /// <param name="assertion">The description of the assertion to evaluate</param>
        /// <param name="condition">The assertion condition delegate</param>
        /// <returns>The assertion result, never null</returns>
        public static AssertionResult Evaluate(Assertion assertion, AssertionCondition condition)
        {
            return AssertionService.Evaluate(Context.CurrentContext, assertion, condition);
        }

        /// <summary>
        /// Evaluates an assertion condition and returns its result.
        /// This form is provided for convenience to implement new kinds of assertions.
        /// </summary>
        /// <param name="assertionId">The assertion id</param>
        /// <param name="assertionDescription">The assertion description</param>
        /// <param name="detaildetailMessageFormat">The assertion message format</param>
        /// <param name="detaildetailMessageArgs">The assertion message arguments</param>
        /// <param name="condition">The assertion condition</param>
        /// <param name="assertionArgNamesAndValues">An alternating sequence of name / value pairs
        /// describing the assertion's arguments</param>
        /// <returns>The assertion result, never null</returns>
        public static AssertionResult Evaluate(string assertionId, string assertionDescription,
            string detaildetailMessageFormat, object[] detaildetailMessageArgs, AssertionCondition condition,
            params object[] assertionArgNamesAndValues)
        {
            Assertion assertion = new Assertion(assertionId, assertionDescription,
                String.Format(CultureInfo.CurrentCulture, detaildetailMessageFormat, detaildetailMessageArgs),
                assertionArgNamesAndValues);
            return Evaluate(assertion, condition);
        }

        /// <summary>
        /// Verifies that the result succeeded.
        /// Adds the result to the list being prepared for the current assertion scope
        /// such as the containing <see cref="Run" /> block.
        /// Throws an <see cref="AssertionException" /> to terminate execution of the
        /// code block if invoked within a scope that does not permit multiple failures.
        /// </summary>
        /// <param name="result">The assertion result to verify</param>
        public static void Verify(AssertionResult result)
        {
            AssertionService.Verify(Context.CurrentContext, result);
        }

        /// <summary>
        /// Evaluates an assertion condition and verifies that it succeeded.
        /// This form is provided for convenience to implement new kinds of assertions.
        /// </summary>
        /// <param name="assertionId">The assertion id</param>
        /// <param name="assertionDescription">The assertion description</param>
        /// <param name="detaildetailMessageFormat">The assertion message format</param>
        /// <param name="detaildetailMessageArgs">The assertion message arguments</param>
        /// <param name="condition">The assertion condition</param>
        /// <param name="assertionArgNamesAndValues">An alternating sequence of name / value pairs
        /// describing the assertion's arguments</param>
        public static void EvaluateAndVerify(string assertionId, string assertionDescription,
            string detaildetailMessageFormat, object[] detaildetailMessageArgs, AssertionCondition condition,
            params object[] assertionArgNamesAndValues)
        {
            AssertionResult result = Evaluate(assertionId, assertionDescription,
                detaildetailMessageFormat, detaildetailMessageArgs, condition, assertionArgNamesAndValues);
            Verify(result);
        }
        #endregion Primitives


        #region Assertion Counting
        /// <summary>
        /// Gets the total number of assertions verified so far in the current context.
        /// </summary>
        public static int AssertCount
        {
            get
            {
                return AssertionService.GetAssertionCount(Context.CurrentContext);
            }
        }

        /// <summary>
        /// Increments the total number of assertions verified so far in the current context.
        /// </summary>
        [Obsolete("The total number of assertions verified is now tracked automatically by the Assert.Verify primitive."
            + "  If you were using this method implement custom assertions then you should refactor them based on the new primitives"
            + " which have improved diagnostic and reporting characteristics.")]
        public static void IncrementAssertCount()
        {
            EvaluateAndVerify("Assert.IncrementAssertCount", "Increments the assertion count prior to an MbUnit v2 style custom assertion.",
                "", null, delegate
            {
                return AssertionResult.CreateSuccessResult();
            });
        }
        #endregion


        /* Not implemented yet.
        #region With
        /// <summary>
        /// Includes a named object in assertion failure reports.
        /// Takes a snapshot of the current state of an object and includes it
        /// in the report to help diagnose a particular failure.
        /// </summary>
        /// <example>
        /// <code>
        /// Uri uri = new Uri("foo://bar");
        /// using (Assert.With("uri", uri))
        /// {
        ///     Assert.AreEqual("foo://bar", uri.AbsolutePath);
        /// }
        /// </code>
        /// </example>
        /// <param name="objectName">The name of the object to include in the report</param>
        /// <param name="objectValue">The value of the object to include in the report</param>
        /// <returns>An object that when disposed indicates the end of the block</returns>
        public static IDisposable With(string objectName, object objectValue)
        {
            return With(new object[] { objectName, objectValue });
        }

        /// <summary>
        /// Includes zero or more named objects in assertion failure reports.
        /// Takes a snapshot of the current state of the objects and includes them
        /// in the report to help diagnose a particular failure.
        /// </summary>
        /// <example>
        /// <code>
        /// Uri uri = new Uri("foo://bar");
        /// string request = "Quux";
        /// using (Assert.With("uri", uri, "request", request))
        /// {
        ///     Assert.AreEqual("foo://bar", uri.AbsolutePath);
        /// }
        /// </code>
        /// </example>
        /// <param name="objectNamesAndValues">The array of alternating name and value pairs for objects to
        /// include in the report</param>
        /// <returns>An object that when disposed indicates the end of the block</returns>
        public static IDisposable With(params object[] objectNamesAndValues)
        {
            return AssertionService.With(Context.CurrentContext, objectNamesAndValues);
        }
        #endregion
        */

        #region Ignore
        /// <summary>
        /// Executes a block of code.
        /// Evaluates the assertions within but ignores their results and permits execution to
        /// continue uninterrupted.  Also ignores (but reports) any exception thrown within the
        /// block and returns control to the caller of Ignore.
        /// </summary>
        /// <param name="block">The block of code to execute.</param>
        /// <remarks>
        /// An ignored assertion block is useful for debugging complicated assertions because
        /// it permits the test to keep running through to completion or the first exception thrown.
        /// Ignored assertions should never be present in production code except as a temporary workaround.
        /// 
        /// Consequently, this method never throws an <see cref="AssertionException" />.
        /// </remarks>
        public static void Ignore(Block block)
        {
            Ignore(block, "");
        }

        /// <summary>
        /// Executes a block of code consisting of multiple related assertions.
        /// Evaluates the assertions within but ignores their results and permits execution to
        /// continue uninterrupted.  Also ignores (but reports) any exception thrown within
        /// the block and returns control to the caller of Ignore.
        /// </summary>
        /// <param name="block">The block of code to execute.</param>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <remarks>
        /// An ignored assertion block is useful for debugging complicated assertions because
        /// it permits the test to keep running through to completion or the first exception thrown.
        /// Ignored assertions should never be present in production code except as a temporary workaround.
        ///
        /// Consequently, this method never throws an <see cref="AssertionException" />.
        /// </remarks>
        public static void Ignore(Block block, string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.Ignore", "Ignores assertion failures or errors within the block and permits execution to continue uninterrupted.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                AssertionResult[] innerResults = Run(block, true);
                AssertionResult result = new AssertionResult(AssertionResultType.Ignore, "", null, innerResults);

                result.InnerResults = innerResults;
                return result;
            });
        }
        #endregion


        #region Multiple
        /// <summary>
        /// Executes a block of code consisting of multiple related assertions.
        /// Maintains a tally of all assertion failures that occur within the block
        /// but allows execution to proceed unless an uncaught exception occurs.  When the
        /// block exits, reports all assertion failures together as a unit.
        /// </summary>
        /// <param name="block">The block of code to execute.</param>
        /// <remarks>
        /// A multiple assertion block is useful for verifying the state of a component
        /// with many parts that require several assertions to check but are logically
        /// still one verification.  This feature can accelerate debugging because more
        /// diagnostic information about a component becomes available in one pass.
        /// </remarks>
        /// <exception cref="AssertionException">Thrown if one or more assertions failed and not nested
        /// within another <see cref="Assert.Multiple" /> block.</exception>
        public static void Multiple(Block block)
        {
            Multiple(block, "");
        }

        /// <summary>
        /// Executes a block of code consisting of multiple related assertions.
        /// Maintains a tally of all assertion failures and errors that occur within the block
        /// but allows execution to proceed unless an uncaught exception occurs.  When the
        /// block exits, reports all assertion failures and errors together as a unit.
        /// </summary>
        /// <param name="block">The block of code to execute.</param>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <remarks>
        /// A multiple assertion block is useful for verifying the state of a component
        /// with many parts that require several assertions to check but are logically
        /// still one verification.  This feature can accelerate debugging because more
        /// diagnostic information about a component becomes available in one pass.
        /// </remarks>
        /// <exception cref="AssertionException">Thrown if one or more assertions failed and not nested
        /// within another <see cref="Assert.Multiple" /> block.</exception>
        public static void Multiple(Block block, string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.Multiple", "Verifies that at all assertions within a code block succeeded.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                AssertionResult[] innerResults = Run(block, true);

                int failureCount = 0;
                foreach (AssertionResult innerResult in innerResults)
                    if (! innerResult.IsOk)
                        failureCount += 1;

                AssertionResult result;
                if (failureCount == 0)
                    result = AssertionResult.CreateSuccessResult();
                else
                    result = AssertionResult.CreateFailureResult(String.Format(CultureInfo.CurrentCulture,
                        "There were {0} assertion failures or errors among the {1} assertions within the code block.",
                        failureCount, innerResults.Length));

                result.InnerResults = innerResults;
                return result;
            });
        }
        #endregion


        #region Fail
        /// <summary>
        /// Always reports an assertion failure.
        /// </summary>
        /// <exception cref="AssertionException">Thrown unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static void Fail()
        {
            Fail("");
        }

        /// <summary>
        /// Always reports an assertion failure.
        /// </summary>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <remarks>
        /// The error message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </remarks>
        /// <exception cref="AssertionException">Thrown unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static void Fail(string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.Fail", "Always reports an assertion failure.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                return AssertionResult.CreateFailureResult("Failed.");
            });
        }
        #endregion


        #region Warning
        /// <summary>
        /// Reports a warning about an inconclusive assertion or a condition that might produce
        /// inconclusive results such as a violated environmental invariant or the failure to
        /// clean up after a test which may result in a diminution of test isolation.
        /// </summary>
        /// <param name="detailMessageFormat">The format of the warning message.</param>
        /// <param name="detailMessageArgs">The array of warning message format arguments.</param>
        /// <remarks>
        /// The warning message is formatted using <see cref="String.Format(string, object[])"/>.
        /// </remarks>
        public static void Warning(string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.Warning", "Reports a warning about an inconclusive assertion or a condition that "
                + " might produce inconclusive results such as a violated environmental invariant or the failure to"
                + " clean up after a test which may result in a diminution of test isolation.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                return new AssertionResult(AssertionResultType.Inconclusive, "A warning was emitted by a test.", null, null);
            });
        }
        #endregion


        #region ExpectedFailure
        /// <summary>
        /// Verifies that a block of code contains an assertion failure.
        /// Reports an assertion failure if no <see cref="AssertionException" /> exception
        /// was thrown by the block, otherwise consumes the failure and continues normally.
        /// This method is used to invert the meaning of a sequence of assertions within
        /// a block of code and is approximately equivalent to using <see cref="ExpectedException" />
        /// with an exception type of <see cref="AssertionException" />.
        /// </summary>
        /// <param name="block">The block of code to execute.</param>
        /// <returns>The <see cref="AssertionResult" /> representing the first failure that occurred
        /// within the block.  May be null if nested within a <see cref="Assert.Multiple" /> block
        /// and no failure occurred.</returns>
        /// <exception cref="AssertionException">Thrown if no failure occurs within the block unless
        /// this call is nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static AssertionResult ExpectedFailure(Block block)
        {
            return ExpectedFailure(block, "");
        }

        /// <summary>
        /// Verifies that a block of code contains an assertion failure.
        /// Reports an assertion failure if no <see cref="AssertionException" /> exception
        /// was thrown by the block, otherwise consumes the failure and continues normally.
        /// This method is used to invert the meaning of a sequence of assertions within
        /// a block of code and is approximately equivalent to using <see cref="ExpectedException" />
        /// with an exception type of <see cref="AssertionException" />.
        /// </summary>
        /// <param name="block">The block of code to execute.</param>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <returns>The <see cref="AssertionResult" /> representing the first failure that occurred
        /// within the block.  May be null if nested within a <see cref="Assert.Multiple" /> block
        /// and no failure occurred.</returns>
        /// <exception cref="AssertionException">Thrown if no failure occurs within the block unless
        /// this call is nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static AssertionResult ExpectedFailure(Block block, string detailMessageFormat, params object[] detailMessageArgs)
        {
            AssertionResult firstFailureResult = null;

            EvaluateAndVerify("Assert.ExpectedFailure", "Verifies that at least one assertion within a code block failed.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                AssertionResult[] innerResults = Run(block, false);

                if (innerResults.Length == 0)
                    return AssertionResult.CreateFailureResult("The block did not evaluate any assertions so none of them failed.");

                foreach (AssertionResult innerResult in innerResults)
                {
                    if (innerResult.ResultType == AssertionResultType.Failure)
                    {
                        firstFailureResult = innerResult;
                        break;
                    }
                }

                if (firstFailureResult == null)
                {
                    return AssertionResult.CreateFailureResult(String.Format(CultureInfo.CurrentCulture,
                        "Expected at least 1 assertion failure among the {0} assertions within the code block.",
                        innerResults.Length), null, innerResults);
                }

                return AssertionResult.CreateSuccessResult(innerResults);
            });

            return firstFailureResult;
        }
        #endregion


        #region ExpectedException
        /// <summary>
        /// Verifies that a block of code throws an exception of the specified type.
        /// Reports an assertion failure if no exception was thrown or if the exception
        /// that was thrown does not derive from <paramref name="expectedExceptionType"/>.
        /// </summary>
        /// <param name="expectedExceptionType">The type of exception that is expected.</param>
        /// <param name="block">The block of code to execute.</param>
        /// <returns>The <see cref="Exception" /> that was caught or null if none and inside
        /// a <see cref="Assert.Multiple" /> block.</returns>
        /// <exception cref="AssertionException">Thrown on failure unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static Exception ExpectedException(Type expectedExceptionType, Block block)
        {
            return ExpectedException(expectedExceptionType, block);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of the specified type.
        /// Reports an assertion failure if no exception was thrown or if the exception
        /// that was thrown does not derive from <paramref name="expectedExceptionType"/>.
        /// </summary>
        /// <param name="expectedExceptionType">The type of exception that is expected.</param>
        /// <param name="block">The block of code to execute.</param>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <returns>The <see cref="Exception" /> of the expected type that was caught or possibly null
        /// if none and inside a <see cref="Assert.Multiple" /> block.</returns>
        /// <exception cref="AssertionException">Thrown on failure unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static Exception ExpectedException(Type expectedExceptionType, Block block, string detailMessageFormat, params object[] detailMessageArgs)
        {
            Exception exceptionOfExpectedType = null;
            Exception actualException = null;
            try
            {
                block();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            EvaluateAndVerify("Assert.ExpectedException", "Verifies that the code block threw an exception of the expected type.", detailMessageFormat, detailMessageArgs, delegate
            {
                if (actualException == null)
                    return AssertionResult.CreateFailureResult("The code block did not throw any exception.");

                if (!expectedExceptionType.IsInstanceOfType(actualException))
                    return AssertionResult.CreateFailureResult(String.Format(CultureInfo.CurrentCulture,
                        "The code block actually threw an exception of type {0}.", actualException.GetType().FullName));

                exceptionOfExpectedType = actualException;
                return AssertionResult.CreateSuccessResult();
            }, "expectedExceptionType", expectedExceptionType, "actualException", actualException);

            return exceptionOfExpectedType;
        }
        #endregion


        #region IsTrue, IsFalse
        /// <summary>
        /// Verifies that a boolean value is true otherwise reports an assertion failure.
        /// </summary>
        /// <param name="actualValue">The boolean value to verify.</param>
        /// <exception cref="AssertionException">Thrown on failure unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static void IsTrue(bool actualValue)
        {
            IsTrue(actualValue, "");
        }

        /// <summary>
        /// Verifies that a boolean value is true otherwise reports an assertion failure.
        /// </summary>
        /// <param name="actualValue">The boolean value to verify.</param>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <exception cref="AssertionException">Thrown on failure unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static void IsTrue(bool actualValue, string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.IsTrue", "Verifies that a value is true.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                if (!actualValue)
                    return AssertionResult.CreateFailureResult("The actual value was false but we expected true.");

                return AssertionResult.CreateSuccessResult();
            }, "actualValue", actualValue);
        }

        /// <summary>
        /// Verifies that a boolean value is false otherwise reports an assertion failure.
        /// </summary>
        /// <param name="actualValue">The boolean value to verify.</param>
        /// <exception cref="AssertionException">Thrown on failure unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static void IsFalse(bool actualValue)
        {
            IsFalse(actualValue, "");
        }

        /// <summary>
        /// Verifies that a boolean value is false otherwise reports an assertion failure.
        /// </summary>
        /// <param name="actualValue">The boolean value to verify.</param>
        /// <param name="detailMessageFormat">The format of the assertion detail message.</param>
        /// <param name="detailMessageArgs">The array of assertion detail message format arguments.</param>
        /// <exception cref="AssertionException">Thrown on failure unless nested within a <see cref="Assert.Multiple" /> block.</exception>
        public static void IsFalse(bool actualValue, string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.IsFalse", "Verifies that a value is false.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                if (actualValue)
                    return AssertionResult.CreateFailureResult("The actual value was true but we expected false.");

                return AssertionResult.CreateSuccessResult();
            }, "actualValue", actualValue);
        }
        #endregion


        #region AreEqual
        public static void AreEqual(object expectedValue, object actualValue)
        {
            AreEqual(expectedValue, actualValue, "");
        }

        public static void AreEqual(object expectedValue, object actualValue, string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.AreEqual", "Verifies that the actual value equals the expected value.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                if (!ValueEquals(expectedValue, actualValue))
                    return AssertionResult.CreateFailureResult("The actual value did not equal the expected value.");

                return AssertionResult.CreateSuccessResult();
            }, "expectedValue", expectedValue, "actualValue", actualValue);
        }
        #endregion


        #region AreSame
        public static void AreSame(object expectedValue, object actualValue)
        {
            AreSame(expectedValue, actualValue, "");
        }

        public static void AreSame(object expectedValue, object actualValue, string detailMessageFormat, params object[] detailMessageArgs)
        {
            EvaluateAndVerify("Assert.AreSame", "Verifies that the actual value is the same as the expected value by reference.",
                detailMessageFormat, detailMessageArgs, delegate
            {
                if (! Object.ReferenceEquals(expectedValue, actualValue))
                    return AssertionResult.CreateFailureResult("The actual value was not the same as the expected value by reference.");

                return AssertionResult.CreateSuccessResult();
            }, "expectedValue", expectedValue, "actualValue", actualValue);
        }
        #endregion


        #region Helpers
        /// <summary>
        /// Determines if two values are equal or both null.
        /// If the arguments are of different primitive numeric types, they are converted
        /// to strings and compared.  Otherwise they are compared using <see cref="Object.Equals" />.
        /// </summary>
        /// <param name="first">The first value to compare.</param>
        /// <param name="second">The second value to compare.</param>
        /// <returns>True if the values are equal.</returns>
        public static bool ValueEquals(object first, object second)
        {
            if (Object.ReferenceEquals(first, second))
                return true;

            if (first == null || second == null)
                return false; // One must be null but not the other

            // Special handling for numeric types.
            IConvertible firstConvertible = GetConvertibleInterfaceForNumericValue(first);
            IConvertible secondConvertible = GetConvertibleInterfaceForNumericValue(second);

            if (firstConvertible != null && secondConvertible != null)
            {
                string firstString = firstConvertible.ToString(CultureInfo.InvariantCulture);
                string secondString = secondConvertible.ToString(CultureInfo.InvariantCulture);
                return firstString == secondString;
            }

            // Common handling for all other types.
            return first.Equals(second);
        }

        /// <summary>
        /// If the value is of a numeric type, returns its <see cref="IConvertible" /> interface.
        /// </summary>
        /// <param name="value">The value</param>
        /// <returns>The value's convertible interface</returns>
        private static IConvertible GetConvertibleInterfaceForNumericValue(object value)
        {
            IConvertible convertibleValue = value as IConvertible;
            if (convertibleValue != null)
            {
                switch (convertibleValue.GetTypeCode())
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return convertibleValue;
                }
            }

            return null;
        }
        #endregion
    }
}
