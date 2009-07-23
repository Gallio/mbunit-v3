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
using System.Threading;
using Gallio.Common;
using JetBrains.Util;
using Action = Gallio.Common.Action;

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// Provides a mechanism to suppress ReSharper logger exceptions 
    /// such as assertion failures being displayed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is used to work around bugs within the R# object model
    /// that may interfere with normal use of the application.
    /// </para>
    /// <para>
    /// For example, R# 4.0's metadata API appears to contain a bug parsing out
    /// attributes with typed array parameters.  This causes an assertion error to be
    /// raised.  However Gallio is capable of tolerating these problems itself.
    /// </para>
    /// </remarks>
    internal static class ReSharperExceptionDialogSuppressor
    {
        private static readonly object syncRoot = new object();
        private static int nesting;
        private static bool initialSetting;

        /// <summary>
        /// Temporarily disables exception logging.
        /// </summary>
        /// <param name="bugExplanation">An explanation of the bug that we are working around.</param>
        /// <param name="action">The action to perform.</param>
        public static void Suppress(string bugExplanation, Action action)
        {
            try
            {
                lock (syncRoot)
                {
                    if (++nesting == 1)
                    {
                        initialSetting = Logger.AllowExceptions;
                        Logger.AllowExceptions = true;
                    }
                }

                action();
            }
#if RESHARPER_31
            catch (InternalErrorException ex)
#else
            catch (LoggerException ex)
#endif
            {
                throw new InternalErrorException(bugExplanation, ex);
            }
            finally
            {
                lock (syncRoot)
                {
                    if (--nesting == 0)
                        Logger.AllowExceptions = initialSetting;
                }
            }
        }
    }
}
