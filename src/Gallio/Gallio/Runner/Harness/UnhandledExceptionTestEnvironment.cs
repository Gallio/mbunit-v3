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
using Gallio.Common.Policies;
using Gallio.Model.Execution;

namespace Gallio.Runner.Harness
{
    /// <summary>
    /// Logs unhandled exceptions instead of killing the AppDomain.
    /// </summary>
    public class UnhandledExceptionTestEnvironment : ITestEnvironment
    {
        /// <inheritdoc />
        public IDisposable SetUp()
        {
            return new State();
        }

        private sealed class State : IDisposable
        {
            public State()
            {
                UnhandledExceptionPolicy.CorrelateUnhandledException += CorrelateUnhandledException;
            }

            public void Dispose()
            {
                UnhandledExceptionPolicy.CorrelateUnhandledException -= CorrelateUnhandledException;
            }

            private static void CorrelateUnhandledException(object sender, CorrelatedExceptionEventArgs e)
            {
                ITestContext context = TestContextTrackerAccessor.Instance.CurrentContext;
                if (context != null)
                    e.AddCorrelationMessage(String.Format("The exception occurred while test step '{0}' was running.", context.TestStep.FullName));
            }
        }
    }
}