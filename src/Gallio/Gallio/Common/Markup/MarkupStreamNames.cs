// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common.Markup
{
    /// <summary>
    /// Common markup stream names.
    /// </summary>
    public static class MarkupStreamNames
    {
        /// <summary>
        /// The name of the standard stream for captured <see cref="Console.In" /> messages.
        /// </summary>
        public const string ConsoleInput = "ConsoleInput";

        /// <summary>
        /// The name of the standard stream for captured <see cref="Console.Out" /> messages.
        /// </summary>
        public const string ConsoleOutput = "ConsoleOutput";

        /// <summary>
        /// The name of the standard stream for captured <see cref="Console.Error" /> messages.
        /// </summary>
        public const string ConsoleError = "ConsoleError";

        /// <summary>
        /// The name of the standard stream for diagnostic <see cref="Debug" /> and <see cref="Trace" /> messages.
        /// </summary>
        public const string DebugTrace = "DebugTrace";

        /// <summary>
        /// The name of the standard stream for reporting warning messages.
        /// </summary>
        public const string Warnings = "Warnings";

        /// <summary>
        /// The name of the standard stream for reporting failure messages and exceptions.
        /// </summary>
        public const string Failures = "Failures";

        /// <summary>
        /// The name of the standard stream for generic log messages.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is preferable to write log messages to this stream in place of the console
        /// output and error streams.
        /// </para>
        /// </remarks>
        public const string Default = "Log";
    }
}