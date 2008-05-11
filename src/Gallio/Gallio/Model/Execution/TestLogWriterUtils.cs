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
using System.Text;
using Gallio.Utilities;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Utilities for <see cref="ITestLogWriter" />.
    /// </summary>
    public static class TestLogWriterUtils
    {
        /// <summary>
        /// Writes an exception to the log within its own section with the specified name.
        /// </summary>
        /// <param name="writer">The log writer</param>
        /// <param name="streamName">The stream name</param>
        /// <param name="exception">The exception to write</param>
        /// <param name="sectionName">The section name</param>
        public static void WriteException(ITestLogWriter writer, string streamName, Exception exception, string sectionName)
        {
            writer.BeginSection(streamName, sectionName);
            writer.Write(streamName, ExceptionUtils.SafeToString(exception));
            writer.EndSection(streamName);
        }
    }
}
