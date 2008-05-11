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
using Gallio.Runtime.Logging;
using JetBrains.Util;

namespace Gallio.ReSharperRunner.Runtime
{
    internal class ReSharperLogger : BaseLogger
    {
        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            string fullMessage = message;
            if (exception != null)
                fullMessage += exception;

            Logger.LogMessage(GetLoggingLevel(severity), string.Concat(GetCaption(severity), ": ", fullMessage));

            if (exception != null)
            {
                if (severity == LogSeverity.Error)
                    Logger.LogException(message, exception);
                else
                    Logger.LogExceptionSilently(exception);
            }
        }

        private static LoggingLevel GetLoggingLevel(LogSeverity severity)
        {
            return severity == LogSeverity.Debug ? LoggingLevel.VERBOSE : LoggingLevel.NORMAL;
        }

        private static string GetCaption(LogSeverity severity)
        {
            return severity.ToString().ToUpperInvariant();
        }
    }
}
