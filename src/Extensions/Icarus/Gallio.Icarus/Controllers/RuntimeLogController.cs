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
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Logging;
using Gallio.Runtime.Logging;

namespace Gallio.Icarus.Controllers
{
    internal class RuntimeLogController : IRuntimeLogController
    {
        private readonly IOptionsController optionsController;
        private IRuntimeLogger runtimeLogger;

        public LogSeverity MinLogSeverity
        {
            get
            {
                return runtimeLogger.MinLogSeverity;
            }
            set
            {
                runtimeLogger.MinLogSeverity = value;
                optionsController.MinLogSeverity = value;
                optionsController.Save();
            }
        }

        public event EventHandler<RuntimeLogEventArgs> LogMessage;
        
        public void SetLogger(IRuntimeLogger runtimeLogger)
        {
            this.runtimeLogger = runtimeLogger;
            runtimeLogger.LogMessage += (sender, e) => LogMessage(this, e);
            runtimeLogger.MinLogSeverity = optionsController.MinLogSeverity;
        }

        public RuntimeLogController(IOptionsController optionsController)
        {
            this.optionsController = optionsController;
        }
    }
}
