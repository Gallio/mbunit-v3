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
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Logging;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Execution;

namespace Gallio.VisualStudio.Tip
{
    internal class RunContextLogger : BaseLogger
    {
        private readonly IRunContext runContext;

        public RunContextLogger(IRunContext runContext)
        {
            this.runContext = runContext;
        }

        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            TestOutcome outcome;
            if (severity == LogSeverity.Warning)
                outcome = TestOutcome.Warning;
            else if (severity == LogSeverity.Error)
                outcome = TestOutcome.Error;
            else
                return;

            if (exceptionData != null)
                message = string.Concat(message, "\n", exceptionData.ToString());

            TestRunTextResultMessage resultMessage = new TestRunTextResultMessage(runContext.RunConfig.TestRun.Id, message);
            resultMessage.Outcome = outcome;

            runContext.ResultSink.AddResult(resultMessage);
        }
    }
}
