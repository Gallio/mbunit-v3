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
using System.Drawing;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Logging;

namespace Gallio.Icarus.Logging
{
    public class RuntimeLogger : BaseLogger
    {
        public LogSeverity MinLogSeverity
        {
            get; 
            set;
        }

        public event EventHandler<RuntimeLogEventArgs> LogMessage;

        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            if (severity < MinLogSeverity || LogMessage == null)
                return;

            Color color = Color.Black;
            switch (severity)
            {
                case LogSeverity.Error:
                    color = Color.Red;
                    break;

                case LogSeverity.Warning:
                    color = Color.Gold;
                    break;

                case LogSeverity.Important:
                    color = Color.Black;
                    break;

                case LogSeverity.Info:
                    color = Color.Gray;
                    break;

                case LogSeverity.Debug:
                    color = Color.DarkGray;
                    break;
            }

            LogMessage(this, new RuntimeLogEventArgs(message, color));

            if (exceptionData != null)
                LogMessage(this, new RuntimeLogEventArgs(exceptionData.ToString(), color));
        }
    }
}
