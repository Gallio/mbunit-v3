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

namespace Gallio.Model.Logging
{
    /// <summary>
    /// Common test log stream names.
    /// </summary>
    public static class TestLogStreamNames
    {
        /// <summary>
        /// The name of the built-in log stream where the <see cref="Console.In" />
        /// stream for the test is recorded.
        /// </summary>
        public const string ConsoleInput = "ConsoleInput";

        /// <summary>
        /// The name of the built-in log stream where the <see cref="Console.Out" />
        /// stream from the test is recorded.
        /// </summary>
        public const string ConsoleOutput = "ConsoleOutput";

        /// <summary>
        /// The name of the built-in log stream where the <see cref="Console.Error" />
        /// stream from the test is recorded.
        /// </summary>
        public const string ConsoleError = "ConsoleError";

        /// <summary>
        /// The name of the built-in log stream log stream where diagnostic <see cref="Debug" />
        /// and <see cref="Trace" /> information is recorded.
        /// </summary>
        public const string DebugTrace = "DebugTrace";

        /// <summary>
        /// The name of the built-in log stream where warnings are recorded.
        /// </summary>
        public const string Warnings = "Warnings";

        /// <summary>
        /// The name of the built-in log stream where assertion failures,
        /// exceptions and other failure data are recorded.
        /// </summary>
        public const string Failures = "Failures";

        /// <summary>
        /// The name of the built-in log stream where additional log output is recorded.
        /// In new test frameworks, this stream should be used instead of the console output
        /// and error streams for logging test output.
        /// </summary>
        public const string Default = "Log";
    }
}