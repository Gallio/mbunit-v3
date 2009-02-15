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
using Gallio.Runtime;

namespace Gallio.Utilities
{
    /// <summary>
    /// Provides a few functions for working with <see cref="EventHandler"/> and
    /// <see cref="EventHandler{T}"/>.
    /// </summary>
    public static class EventHandlerUtils
    {
        /// <summary>
        /// Safely invokes each delegate in the invocation list of an event handler.
        /// Sends any exceptions thrown by the handler to <see cref="UnhandledExceptionPolicy.Report"/>.
        /// </summary>
        /// <param name="handlerChain">The event handler chain</param>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        public static void SafeInvoke(EventHandler handlerChain, object sender, EventArgs e)
        {
            if (handlerChain == null)
                return;

            foreach (EventHandler handler in handlerChain.GetInvocationList())
            {
                try
                {
                    handler(sender, e);
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        /// <summary>
        /// Safely invokes each delegate in the invocation list of an event handler.
        /// Sends any exceptions thrown by the handler to <see cref="UnhandledExceptionPolicy.Report"/>.
        /// </summary>
        /// <param name="handlerChain">The event handler chain</param>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        public static void SafeInvoke<T>(EventHandler<T> handlerChain, object sender, T e)
            where T : EventArgs
        {
            if (handlerChain == null)
                return;

            foreach (EventHandler<T> handler in handlerChain.GetInvocationList())
            {
                try
                {
                    handler(sender, e);
                }
                catch (Exception ex)
                {
                    ReportException(ex);
                }
            }
        }

        private static void ReportException(Exception ex)
        {
            UnhandledExceptionPolicy.Report("An exception occurred in an event handler.", ex);
        }
    }
}
