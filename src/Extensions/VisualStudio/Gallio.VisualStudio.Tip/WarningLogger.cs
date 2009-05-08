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
using Gallio.Runtime.Diagnostics;
using Gallio.Runtime.Logging;
using Microsoft.VisualStudio.TestTools.Common;

namespace Gallio.VisualStudio.Tip
{
    internal class WarningLogger : BaseLogger
    {
        private readonly IWarningHandler warningHandler;

        public WarningLogger(IWarningHandler warningHandler)
        {
            this.warningHandler = warningHandler;
        }

        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            if (severity >= LogSeverity.Warning)
            {
                string warning = exceptionData == null
                    ? message
                    : string.Concat(message, "\n", exceptionData.ToString());

                warningHandler.Write(this, new WarningEventArgs(warning));
            }
        }
    }
}
