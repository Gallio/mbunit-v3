// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using Gallio.Runtime;

namespace Gallio.Utilities
{
    /// <summary>
    /// Provides helper functions for manipulating <see cref="Exception" />s.
    /// </summary>
    public static class ExceptionUtils
    {
        /// <summary>
        /// <para>
        /// Safely converts an exception to a string.
        /// </para>
        /// <para>
        /// This method protects the caller from unexpected failures that may occur while
        /// reporting an exception of untrusted origin.  If an error occurs while converting the
        /// exception to a string, this method returns a generic description of the exception
        /// that can be used instead of the real thing.
        /// </para>
        /// <para>
        /// It can happen that converting an exception to a string produces a secondary exception
        /// because the <see cref="Exception.ToString" /> method has been overridden or because the
        /// stack frames associated with the exception are malformed.  For example, we observed
        /// one case of a <see cref="TypeLoadException" /> being thrown while printing an exception
        /// because one of the stack frames referred to a dynamic method with incorrect metadata.
        /// </para>
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <returns>The string contents</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null</exception>
        /// <seealso cref="Exception.ToString"/>
        public static string SafeToString(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            try
            {
                return ex.ToString();
            }
            catch (Exception ex2)
            {
                return String.Format("An exception of type '{0}' occurred.  Details are not available because a second exception of type '{1}' occurred in Exception.ToString().",
                    ex.GetType().FullName, ex2.GetType().FullName);
            }
        }

        /// <summary>
        /// Safely obtains the <see cref="Exception.Message"/> component of an exception.
        /// </summary>
        /// <returns>The message</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null</exception>
        /// <seealso cref="SafeToString"/>
        public static string SafeGetMessage(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            try
            {
                return ex.Message ?? "";
            }
            catch (Exception ex2)
            {
                return String.Format("An exception of type '{0}' occurred.  Details are not available because a second exception of type '{1}' occurred in Exception.ToString().",
                    ex.GetType().FullName, ex2.GetType().FullName);
            }
        }

        /// <summary>
        /// Safely obtains the <see cref="Exception.StackTrace"/> component of an exception.
        /// </summary>
        /// <returns>The stack trace</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="ex"/> is null</exception>
        /// <seealso cref="SafeToString"/>
        public static string SafeGetStackTrace(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            try
            {
                return ex.StackTrace ?? "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Rethrows an exception without discarding its stack trace.
        /// This enables the inner exception of <see cref="TargetInvocationException" />
        /// to be unwrapped.
        /// </summary>
        /// <remarks>
        /// This implementation is based on code by Brad Wilson.
        /// </remarks>
        /// <param name="ex">The exception to rethrow</param>
        [DebuggerStepThrough, DebuggerHidden]
        [ReflectionPermission(SecurityAction.Assert, MemberAccess=true)]
        public static void RethrowWithNoStackTraceLoss(Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException("ex");

            // Hack the stack trace so it appears to have been preserved.
            // If we can't hack the stack trace, then there's not much we can do as anything we
            // choose will alter the semantics of test execution.
            if (RuntimeDetection.IsUsingMono)
            {
                MethodInfo method = typeof(Exception).GetMethod("FixRemotingException", BindingFlags.Instance | BindingFlags.NonPublic);
                ex = (Exception) method.Invoke(ex, null);
            }
            else
            {
                MethodInfo method = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
                method.Invoke(ex, null);
            }

            throw ex;
        }

        /// <summary>
        /// Invokes a method without producing a <see cref="TargetInvocationException" />
        /// </summary>
        /// <param name="method">The method to invoke</param>
        /// <param name="obj">The instance on which to invoke the method, or null if none</param>
        /// <param name="args">The method arguments, or null if none</param>
        /// <returns>The method return value, or null if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is null</exception>
        [DebuggerStepThrough, DebuggerHidden]
        public static object InvokeMethodWithoutTargetInvocationException(MethodBase method, object obj, object[] args)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            try
            {
                return method.Invoke(obj, args);
            }
            catch (TargetInvocationException ex)
            {
                RethrowWithNoStackTraceLoss(ex.InnerException);
                throw;
            }
        }
    }
}