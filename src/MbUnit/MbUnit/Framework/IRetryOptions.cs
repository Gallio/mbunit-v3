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
using Gallio.Common;
using Gallio.Framework.Assertions;
using System.Threading;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies options for the <see cref="Retry"/> operation.
    /// </summary>
    /// <seealso cref="Retry"/>
    public interface IRetryOptions
    {
        /// <summary>
        /// Specifies the maximum number of evaluation attempts, before the <see cref="Retry.Until(Func{bool})"/>, or 
        /// <see cref="Retry.Until(WaitHandle)"/> operation fails.
        /// </summary>
        /// <remarks>
        /// If not specified, the condition will be evaluated an infinite number of times, or until some
        /// other stop criterion is reached.
        /// </remarks>
        /// <param name="repeat">The maximum number of evaluation cycles, or zero or a negative value to specify an unlimited number of attempts.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Repeat"/> option was called more than once.</exception>
        IRetryOptions Repeat(int repeat);

        /// <summary>
        /// Specifies a polling time expressed in milliseconds between each consecutive evaluation of the condition.
        /// </summary>
        /// <remarks>
        /// If not specified, the default polling time is zero; which causes the condition to be evaluated as fast as possible, with
        /// only a brief suspension of the current thread (<see cref="Thread.Sleep(int)"/>).
        /// </remarks>
        /// <param name="milliseconds">The polling time expressed in milliseconds.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="milliseconds"/> is negative.</exception>
        IRetryOptions WithPolling(int milliseconds);

        /// <summary>
        /// Specifies a polling time between each consecutive evaluation of the condition.
        /// </summary>
        /// <remarks>
        /// If not specified, the default polling time is zero; which causes the condition to be evaluated as fast as possible, with
        /// only a brief suspension of the current thread (<see cref="Thread.Sleep(TimeSpan)"/>).
        /// </remarks>
        /// <param name="polling">The polling time.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="polling"/> represents a negative duration.</exception>
        IRetryOptions WithPolling(TimeSpan polling);

        /// <summary>
        /// Specifies a timeout value expressed in milliseconds. The retry operation fails if the 
        /// overall duration exceeds the specified timeout value.
        /// </summary>
        /// <remarks>
        /// If not specified, the default timeout value is set to 10 seconds.
        /// </remarks>
        /// <param name="milliseconds">The timeout value expressed in milliseconds, or a negative value to specify no timeout value.
        /// A value of zero, will let the condition to be evaluated only once.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        IRetryOptions WithTimeout(int milliseconds);

        /// <summary>
        /// Specifies a timeout value. The retry operation fails if the 
        /// overall duration exceeds the specified timeout value.
        /// </summary>
        /// <remarks>
        /// If not specified, the default timeout value is set to 10 seconds.
        /// </remarks>
        /// <param name="timeout">The timeout value, or a negative value to specify no timeout value.
        /// A value of zero, will let the condition to be evaluated only once.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        IRetryOptions WithTimeout(TimeSpan timeout);

        /// <summary>
        /// Specifies a custom action to be executed between each consecutive attempt, but not before the first one.
        /// </summary>
        /// <param name="action">A custom action to be executed.</param>
        /// <returns>An instance to specify other options for the retry operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the option was called more than once.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        IRetryOptions DoBetween(Action action);

        /// <summary>
        /// Specifies a custom formatted message to be added to the text of the assertion raised when
        /// the retry operation has failed.
        /// </summary>
        /// <param name="messageFormat">A user-supplied assertion failure message string, or null if none</param>
        /// <param name="messageArgs">The format arguments, or null or empty if none</param>
        /// <returns></returns>
        IRetryOptions WithFailureMessage(string messageFormat, object[] messageArgs);

        /// <summary>
        /// Specifies the condition to evaluate repeatedly, and starts the entire operation.
        /// The condition is considered fulfilled when it returns true.
        /// </summary>
        /// <param name="condition">The condition to evaluate</param>
        /// <exception cref="AssertionFailureException">Thrown when the condition is false, and a timeout occured, or the maximum
        /// number of evaluation attempts was reached.</exception>
        void Until(Func<bool> condition);

        /// <summary>
        /// Specifies a <see cref="WaitHandle"/> instance to wait for being signaled, and starts the entire operation.
        /// </summary>
        /// <param name="waitHandle">The wait handle to evaluate</param>
        /// <exception cref="AssertionFailureException">Thrown when the wait handle is unsignaled, and a timeout occured, or the maximum
        /// number of evaluation attempts was reached.</exception>
        void Until(WaitHandle waitHandle);

        /// <summary>
        /// Specifies a <see cref="Thread"/> instance to wait for being terminated, and starts the entire operation.
        /// </summary>
        /// <param name="tread">The thread to evaluate</param>
        /// <exception cref="AssertionFailureException">Thrown when the thread is still alive, and a timeout occured, or the maximum
        /// number of evaluation attempts was reached.</exception>
        void Until(Thread tread);
    }
}
