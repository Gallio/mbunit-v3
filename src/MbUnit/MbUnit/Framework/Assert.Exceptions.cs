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
using System.Text;
using Gallio;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region Throws
        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception type then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught.  To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <typeparam name="TExpectedException">The expected type of exception</typeparam>
        /// <param name="action">The action delegate to evaluate</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static TExpectedException Throws<TExpectedException>(Action action)
            where TExpectedException : Exception
        {
            return Throws<TExpectedException>(action, null, null);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type; and that 
        /// the exception has an inner exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception types then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught. To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <typeparam name="TExpectedException">The expected type of exception</typeparam>
        /// <typeparam name="TExpectedInnerException">The expected type of the inner exception</typeparam>
        /// <param name="action">The action delegate to evaluate</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static TExpectedException Throws<TExpectedException, TExpectedInnerException>(Action action)
            where TExpectedException : Exception
            where TExpectedInnerException : Exception
        {
            return Throws<TExpectedException, TExpectedInnerException>(action, null, null);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception type then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught.  To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <typeparam name="TExpectedException">The expected type of exception</typeparam>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static TExpectedException Throws<TExpectedException>(Action action, string messageFormat, params object[] messageArgs)
            where TExpectedException : Exception
        {
            return (TExpectedException)Throws(typeof(TExpectedException), null, action, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type; and that 
        /// the exception has an inner exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception types then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught. To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <typeparam name="TExpectedException">The expected type of the exception</typeparam>
        /// <typeparam name="TExpectedInnerException">The expected type of the inner exception</typeparam>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static TExpectedException Throws<TExpectedException, TExpectedInnerException>(Action action, string messageFormat, params object[] messageArgs)
            where TExpectedException : Exception
            where TExpectedInnerException : Exception
        {
            return (TExpectedException)Throws(typeof(TExpectedException), typeof(TExpectedInnerException), action, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception type then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught.  To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="action">The action delegate to evaluate</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedExceptionType"/>
        /// or <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static Exception Throws(Type expectedExceptionType, Action action)
        {
            return Throws(expectedExceptionType, null, action, null);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type; and that 
        /// the exception has an inner exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception types then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught. To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="expectedInnerExceptionType">The expected inner exception type, or null to ignore the inner exception</param>
        /// <param name="action">The action delegate to evaluate</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedExceptionType"/>
        /// or <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static Exception Throws(Type expectedExceptionType, Type expectedInnerExceptionType, Action action)
        {
            return Throws(expectedExceptionType, expectedInnerExceptionType, action, null);
        }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception type then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught. To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedExceptionType"/>
        /// or <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static Exception Throws(Type expectedExceptionType, Action action, string messageFormat, params object[] messageArgs)
        {
            return Throws(expectedExceptionType, null, action, messageFormat, messageArgs);
     }

        /// <summary>
        /// Verifies that a block of code throws an exception of a particular type; and that 
        /// the exception has an inner exception of a particular type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the block of code throws a subtype of the expected exception types then
        /// this method will still succeed.
        /// </para>
        /// <para>
        /// This method returns the exception that was caught.  To verify additional
        /// properties of the exception, such as the exception message, follow up this
        /// assertion with additional ones that verify these properties of the exception object
        /// that was returned.
        /// </para>
        /// </remarks>
        /// <param name="expectedExceptionType">The expected exception type</param>
        /// <param name="expectedInnerExceptionType">The expected inner exception type, or null to ignore the inner exception</param>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <returns>The exception that was thrown</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedExceptionType"/>
        /// or <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static Exception Throws(Type expectedExceptionType, Type expectedInnerExceptionType, Action action, string messageFormat, params object[] messageArgs)
        {
            if (expectedExceptionType == null)
                throw new ArgumentNullException("expectedExceptionType");
            if (action == null)
                throw new ArgumentNullException("action");

            Exception result = null;
            AssertionHelper.Verify(() =>
            {
                try
                {
                    action();
                    return new AssertionFailureBuilder("Expected the block to throw an exception.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Expected Exception Type", expectedExceptionType)
                        .ToAssertionFailure();
                }
                catch (Exception actualException)
                {
                    if (expectedExceptionType.IsInstanceOfType(actualException))
                    {
                        result = actualException;

                        if (expectedInnerExceptionType != null)
                        {
                            if (expectedInnerExceptionType.IsInstanceOfType(actualException.InnerException))
                            {
                                return null;
                            }

                            if (actualException.InnerException == null)
                            {
                                return new AssertionFailureBuilder("The block threw an exception of the expected type, but having no inner expection.")
                                   .SetMessage(messageFormat, messageArgs)
                                   .AddRawLabeledValue("Expected Inner Exception Type", expectedInnerExceptionType)
                                   .ToAssertionFailure();
                            }

                            return new AssertionFailureBuilder("The block threw an exception of the expected type, but having an unexpected inner expection.")
                                .SetMessage(messageFormat, messageArgs)
                                .AddRawLabeledValue("Expected Inner Exception Type", expectedInnerExceptionType)
                                .AddException(actualException.InnerException)
                                .ToAssertionFailure();
                        }

                        return null;
                    }

                    return new AssertionFailureBuilder("The block threw an exception of a different type than was expected.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Expected Exception Type", expectedExceptionType)
                        .AddException(actualException)
                        .ToAssertionFailure();
                }
            });

            return result;
        }
        #endregion

        #region DoesNotThrow
        /// <summary>
        /// Verifies that a block of code does not throw an exception of any type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The purpose of this assertion is to improve the readability of tests
        /// that only verify that an exception was not thrown.  Using this assertion
        /// makes a positive and explicit statement that not throwing an exception
        /// is itself the primary behavior that is being verified.
        /// </para>
        /// </remarks>
        /// <param name="action">The action delegate to evaluate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotThrow(Action action)
        {
            DoesNotThrow(action, null, null);
        }


        /// <summary>
        /// Verifies that a block of code does not throw an exception of any type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The purpose of this assertion is to improve the readability of tests
        /// that only verify that an exception was not thrown.  Using this assertion
        /// makes a positive and explicit statement that not throwing an exception
        /// is itself the primary behavior that is being verified.
        /// </para>
        /// </remarks>
        /// <param name="action">The action delegate to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void DoesNotThrow(Action action, string messageFormat, params object[] messageArgs)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            AssertionHelper.Verify(delegate
            {
                try
                {
                    action();
                    return null;
                }
                catch (Exception actualException)
                {
                    return new AssertionFailureBuilder("The block threw an exception but none was expected.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(actualException)
                        .ToAssertionFailure();
                }
            });
        }
        #endregion
    }
}
