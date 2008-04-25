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
using System.Collections.Generic;
using System.ComponentModel;

namespace Gallio.Utilities
{
    /// <summary>
    /// Provides helpers for cross-thread synchronization.
    /// </summary>
    public static class Sync
    {
        /// <summary>
        /// Synchronizes an action.
        /// </summary>
        /// <param name="invoker">The invoker, such as a WinForms control</param>
        /// <param name="action">The action</param>
        /// <exception cref="Exception">The exception thrown by the action</exception>
        public static void Invoke(ISynchronizeInvoke invoker, Action action)
        {
            if (invoker.InvokeRequired)
            {
                Exception exception = (Exception)invoker.Invoke(
                    new Func<Action, Exception>(InvokeAndCaptureException), new object[] { action });

                if (exception != null)
                    ExceptionUtils.RethrowWithNoStackTraceLoss(exception);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Synchronizes a function that returns a value.
        /// </summary>
        /// <param name="invoker">The invoker, such as a WinForms control</param>
        /// <param name="func">The function</param>
        /// <returns>The value returned by the function</returns>
        /// <typeparam name="T">The function return type</typeparam>
        /// <exception cref="Exception">The exception thrown by the function</exception>
        public static T Invoke<T>(ISynchronizeInvoke invoker, Func<T> func)
        {
            if (invoker.InvokeRequired)
            {
                KeyValuePair<Exception, T> pair = (KeyValuePair<Exception, T>)invoker.Invoke(
                    new Func<Func<T>, KeyValuePair<Exception, T>>(InvokeAndCaptureException), new object[] { func });

                if (pair.Key != null)
                    ExceptionUtils.RethrowWithNoStackTraceLoss(pair.Key);
                return pair.Value;
            }
            else
            {
                return func();
            }
        }

        private static Exception InvokeAndCaptureException(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private static KeyValuePair<Exception, T> InvokeAndCaptureException<T>(Func<T> func)
        {
            try
            {
                T result = func();
                return new KeyValuePair<Exception,T>(null, result);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<Exception, T>(ex, default(T));
            }
        }
    }
}
