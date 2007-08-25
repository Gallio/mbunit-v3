// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Text;

namespace MbUnit.Framework.Kernel.ExecutionLogs
{
    /// <summary>
    /// Common execution log stream names.
    /// </summary>
    public static class ExecutionLogStreams
    {
        /// <summary>
        /// The name of the built-in log stream where the
        /// console input stream for the test is recorded.
        /// </summary>
        public const string ConsoleInput = "ConsoleInput";

        /// <summary>
        /// The name of the built-in log stream where the
        /// console output stream from the test is recorded.
        /// </summary>
        public const string ConsoleOutput = "ConsoleOutput";

        /// <summary>
        /// The name of the built-in log stream where the
        /// console error stream from the test is recorded.
        /// </summary>
        public const string ConsoleError = "ConsoleError";

        /// <summary>
        /// The name of the built-in log stream where debug information is recorded.
        /// </summary>
        public const string Debug = "Debug";

        /// <summary>
        /// The name of the built-in log stream where diagnostic trace information is recorded.
        /// </summary>
        public const string Trace = "Trace";

        /// <summary>
        /// The name of the built-in log stream where warnings are recorded.
        /// </summary>
        public const string Warnings = "Warnings";

        /// <summary>
        /// The name of the built-in log stream where assertion failures,
        /// exceptions and other failure data are recorded.
        /// </summary>
        public const string Failures = "Failures";
    }
}
