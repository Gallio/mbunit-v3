// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Utilities;

namespace Gallio.Framework
{
    /// <summary>
    /// The <see cref="TestActionInvoker" /> provides methods for safely running
    /// actions as part of tests.
    /// </summary>
    public static class TestActionInvoker
    {
        /// <summary>
        /// Runs a particular action.  If the action throws an exception, this method
        /// catches it, logs it, and produces an appropriate <see cref="TestOutcome" />.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <param name="description">A description of the action being performed,
        /// to be used when reporting failures, or null if none</param>
        /// <param name="expectedExceptionType">The expected exception type name, fullname or assembly-qualified name,
        /// or null if none</param>
        /// <returns>The outcome of the action, never null</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null</exception>
        public static TestOutcome Run(Action action, string description, string expectedExceptionType)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            try
            {
                action();

                if (expectedExceptionType != null)
                {
                    description = description ?? "Expected Exception";
                    using (Log.Failures.BeginSection(description))
                        Log.Failures.WriteLine("Expected an exception of type '{0}' but none was thrown.", expectedExceptionType);

                    return TestOutcome.Failed;
                }

                return TestOutcome.Passed;
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                    ex = ex.InnerException;

                description = description ?? "Exception";

                TestOutcome outcome;
                if (ex is TestException)
                {
                    outcome = ((TestException)ex).Outcome;
                }
                else
                {
                    if (expectedExceptionType != null)
                    {
                        Type exceptionType = ex.GetType();
                        if (exceptionType.Name == expectedExceptionType
                            || exceptionType.FullName == expectedExceptionType
                            || exceptionType.AssemblyQualifiedName == expectedExceptionType)
                            return TestOutcome.Passed;

                        using (Log.Failures.BeginSection(description))
                        {
                            Log.Failures.WriteLine("Expected an exception of type '{0}' but a different exception was thrown.", expectedExceptionType);
                            Log.Failures.WriteLine(ExceptionUtils.SafeToString(ex));
                        }

                        return TestOutcome.Failed;
                    }

                    outcome = TestOutcome.Failed;
                }

                switch (outcome.Status)
                {
                    case TestStatus.Inconclusive:
                        Log.Warnings.WriteException(ex, description);
                        break;

                    case TestStatus.Failed:
                        Log.Failures.WriteException(ex, description);
                        break;
                }

                return outcome;
            }
        }
    }
}
