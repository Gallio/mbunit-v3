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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Parses severity information from log messages encoded by <see cref="SeverityPrefixLogger" />.
    /// </summary>
    /// <seealso cref="SeverityPrefixLogger"/>
    public class SeverityPrefixParser
    {
        private LogSeverity previousSeverity = LogSeverity.Info;

        /// <summary>
        /// Parses a line of log data.
        /// </summary>
        /// <param name="line">The line of log data to parse.</param>
        /// <param name="severity">Set to the severity of the message extracted from the line.</param>
        /// <param name="message">The message extracted from the line.</param>
        /// <returns>True if the message included a severity, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="line"/> is null.</exception>
        public bool ParseLine(string line, out LogSeverity severity, out string message)
        {
            if (line == null)
                throw new ArgumentNullException("line");

            foreach (var pair in SeverityPrefixLogger.SeverityPrefixes)
            {
                if (line.StartsWith(pair.Value))
                {
                    severity = pair.Key;
                    previousSeverity = severity;
                    message = line.Substring(pair.Value.Length);
                    return true;
                }
            }

            severity = previousSeverity;
            message = line;
            return false;
        }
    }
}
