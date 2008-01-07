// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Hosting
{
    /// <summary>
    /// This class provides a mechanism for consuming unhandled errors when the infrastructure
    /// is otherwise unable to deal with them locally.
    /// </summary>
    public static class Panic
    {
        [ThreadStatic]
        private static bool recursionGuard;

        /// <summary>
        /// Reports an unhandled exception.
        /// </summary>
        /// <param name="message">A message to explain how the exception was intercepted</param>
        /// <param name="unhandledException">The unhandled exception</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> or <paramref name="unhandledException"/> is null</exception>
        public static void UnhandledException(string message, Exception unhandledException)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (unhandledException == null)
                throw new ArgumentNullException("unhandledException");

            if (recursionGuard)
            {
                RecursionPolicy();
                return;
            }

            try
            {
                recursionGuard = true;

                ReportingPolicy(message, unhandledException);
            }
            catch (Exception ex)
            {
                FailurePolicy(ex);
            }
            finally
            {
                recursionGuard = false;
            }
        }

        private static void ReportingPolicy(string message, Exception unhandledException)
        {
            // TODO: Allow applications to customize this behavior.
            Runtime.Logger.Fatal("Internal Error: {0}\n{1}", message, unhandledException);
        }

        private static void FailurePolicy(Exception ex)
        {
            // TODO: Allow applications to customize this behavior.
        }

        private static void RecursionPolicy()
        {
            // TODO: Allow applications to customize this behavior.
        }
    }
}