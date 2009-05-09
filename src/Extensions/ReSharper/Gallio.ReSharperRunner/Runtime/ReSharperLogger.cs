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
using System.Text;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Logging;
using JetBrains.Util;

namespace Gallio.ReSharperRunner.Runtime
{
    internal class ReSharperLogger : BaseLogger
    {
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            StringBuilder fullMessage = new StringBuilder();
            fullMessage.Append(GetCaption(severity));
            fullMessage.Append(": ");
            fullMessage.Append(message);

            if (exceptionData != null)
                fullMessage.Append('\n').Append(exceptionData);

            switch (severity)
            {
                case LogSeverity.Debug:
                    Logger.LogMessage(LoggingLevel.VERBOSE, fullMessage.ToString());
                    break;
                case LogSeverity.Info:
                case LogSeverity.Important:
                case LogSeverity.Warning:
                    Logger.LogMessage(LoggingLevel.NORMAL, fullMessage.ToString());
                    break;
                case LogSeverity.Error:
                    Logger.LogError(fullMessage.ToString());
                    break;
            }
        }

        private static string GetCaption(LogSeverity severity)
        {
            return severity.ToString().ToUpperInvariant();
        }
    }
}
