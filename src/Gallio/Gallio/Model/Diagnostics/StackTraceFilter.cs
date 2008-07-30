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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using Gallio.Model.Diagnostics;
using Gallio.Utilities;

namespace Gallio.Model.Diagnostics
{
    /// <summary>
    /// <para>
    /// Provides methods for filtering the stack trace for tests.
    /// </para>
    /// <para>
    /// The objective of this filtering is to omit frames that are not relevant to the user
    /// when reporting test failures.
    /// </para>
    /// <para>
    /// Stack trace filtering proceeds as follows:
    /// <list type="bullet">
    /// <item>Stack frames that have an associated <see cref="DebuggerHiddenAttribute" />
    /// or <see cref="TestFrameworkInternalAttribute"/> are omitted.</item>
    /// <item>The stack trace is chopped just above the top-most stack frame with
    /// an associated <see cref="TestEntryPointAttribute" />.</item>
    /// </list>
    /// </para>
    /// </summary>
    public static class StackTraceFilter
    {
        private static readonly Regex StackFrameRegex = new Regex(@" (?<typeFullName>[^ ]+)\.(?<methodName>[^ .[(]+)(?<genericParams>(?:\[[^(]*\])?)\((?<methodParams>[^)]*)\)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        /// <summary>
        /// Filters a stack trace.
        /// </summary>
        /// <param name="stackTrace">The stack trace</param>
        /// <returns>The filtered stack trace</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stackTrace"/>
        /// is null</exception>
        public static string FilterStackTrace(string stackTrace)
        {
            if (stackTrace == null)
                throw new ArgumentNullException("stackTrace");

            return FilterStackFrames(EnumerateStackFrameInfo(stackTrace));
        }

        /// <summary>
        /// Filters a stack trace.
        /// </summary>
        /// <param name="stackTrace">The stack trace</param>
        /// <returns>The filtered stack trace string</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stackTrace"/>
        /// is null</exception>
        public static string FilterStackTraceToString(StackTrace stackTrace)
        {
            if (stackTrace == null)
                throw new ArgumentNullException("stackTrace");

            return FilterStackFrames(EnumerateStackFrameInfo(stackTrace));
        }

        /// <summary>
        /// Filters the stack trace information in an exception.
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <returns>The filtered exception data</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>
        /// is null</exception>
        public static ExceptionData FilterException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return FilterException(new ExceptionData(exception));
        }

        /// <summary>
        /// Filters the stack trace information about an exception.
        /// </summary>
        /// <param name="exception">The exception data</param>
        /// <returns>The filtered exception data</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/>
        /// is null</exception>
        public static ExceptionData FilterException(ExceptionData exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            return new ExceptionData(exception.Type, exception.Message,
                FilterStackFrames(EnumerateStackFrameInfo(exception.StackTrace)),
                exception.InnerException != null ? FilterException(exception.InnerException) : null);
        }

        /// <summary>
        /// Captures a filtered stack trace from the currently executing thread.
        /// </summary>
        /// <returns>The filtered stack trace</returns>
        [DebuggerHidden]
        public static string CaptureFilteredStackTrace()
        {
            return FilterStackTraceToString(new StackTrace(true));
        }

        private static IEnumerable<KeyValuePair<string, MethodBase>> EnumerateStackFrameInfo(StackTrace stackTrace)
        {
            StringReader unfilteredReader = new StringReader(stackTrace.ToString());
            foreach (StackFrame stackFrame in stackTrace.GetFrames())
                yield return new KeyValuePair<string, MethodBase>(unfilteredReader.ReadLine(), stackFrame.GetMethod());
        }

        // You may have noticed that StackTrace has a constructor that takes an Exception as
        // input.  We cannot use this since it will not correctly retrieve stack frames that
        // have been pre-encoded as strings within the exception such as when we use
        // ExceptionUtils.RethrowWithNoStackTraceLoss.
        //
        // The idea is to parse the exception's stack trace and to filter it on the fly.
        // Anything that we fail to parse, we print out verbatim.
        private static IEnumerable<KeyValuePair<string, MethodBase>> EnumerateStackFrameInfo(string unfilteredStackTrace)
        {
            StringReader unfilteredReader = new StringReader(unfilteredStackTrace);
            for (string line; (line = unfilteredReader.ReadLine()) != null; )
            {
                Match match = StackFrameRegex.Match(line);
                MethodBase method;
                if (match.Success)
                    method = FindMethod(match.Groups["typeFullName"].Value, match.Groups["methodName"].Value,
                        match.Groups["genericParams"].Value, match.Groups["methodParams"].Value);
                else
                    method = null;

                yield return new KeyValuePair<string, MethodBase>(line, method);
            }
        }

        private static string FilterStackFrames(IEnumerable<KeyValuePair<string, MethodBase>> frames)
        {
            StringBuilder filteredString = new StringBuilder();

            foreach (KeyValuePair<string, MethodBase> pair in frames)
            {
                string line = pair.Key;
                if (line != null)
                {
                    MethodBase method = pair.Value;
                    if (method != null)
                    {
                        if (ShouldFilterStopAboveFrame(method))
                            break;

                        if (ShouldFilterOmitFrame(method))
                            continue;
                    }

                    filteredString.AppendLine(line);
                }
            }

            return filteredString.ToString();
        }

        private static bool ShouldFilterStopAboveFrame(MethodBase method)
        {
            return method.IsDefined(typeof(TestEntryPointAttribute), true);
        }

        private static bool ShouldFilterOmitFrame(MethodBase method)
        {
            if (method.IsDefined(typeof(DebuggerHiddenAttribute), true)
                || method.IsDefined(typeof(TestFrameworkInternalAttribute), true))
                return true;

            Type declaringType = method.DeclaringType;
            while (declaringType != null)
            {
                if (declaringType.IsDefined(typeof(TestFrameworkInternalAttribute), true))
                    return true;

                declaringType = declaringType.DeclaringType;
            }

            return false;
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess=true)]
        private static MethodBase FindMethod(string typeFullName, string methodName, string genericParams, string methodParams)
        {
            // Look for a probable match for the method in each loaded assembly.
            // TODO: Consider overloads.  (Shouldn't matter much for most filtering purposes
            //       since overloads tend to have the same contract.)
            for (; ; )
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type type = assembly.GetType(typeFullName);
                    if (type != null)
                    {
                        MemberInfo[] members = type.GetMember(methodName,
                            MemberTypes.Constructor | MemberTypes.Method,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                        if (members.Length != 0)
                            return (MethodBase) members[0];
                    }
                }

                // Consider nested types.
                // Need to replace '.' with '+' on final segments.
                int lastDotPos = typeFullName.LastIndexOf('.');
                if (lastDotPos < 0)
                    return null;

                StringBuilder temp = new StringBuilder(typeFullName);
                temp[lastDotPos] = '+';
                typeFullName = temp.ToString();
            }
        }
    }
}