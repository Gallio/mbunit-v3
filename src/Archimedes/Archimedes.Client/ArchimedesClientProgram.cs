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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Archimedes.Client.Properties;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Logging;

namespace Archimedes.Client
{
    public class ArchimedesClientProgram : ConsoleProgram<ArchimedesClientArguments>
    {
        private readonly string runtimePath;

        public ArchimedesClientProgram(string runtimePath)
        {
            this.runtimePath = runtimePath;

            ApplicationName = Resources.ApplicationName;
        }

        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return 1;
            }

            if (Arguments.Help)
            {
                ShowHelp();
                return 0;
            }

            RuntimeSetup runtimeSetup = new RuntimeSetup();
            runtimeSetup.RuntimePath = runtimePath;
            foreach (string pluginDirectory in Arguments.PluginDirectories)
                runtimeSetup.AddPluginDirectory(pluginDirectory);

            RichConsoleLogger runtimeLogger = new RichConsoleLogger(Console);
            FilteredLogger filteredLogger = new FilteredLogger(runtimeLogger, Arguments.Verbosity);

            RuntimeBootstrap.Initialize(runtimeSetup, filteredLogger);

            runtimeLogger.Log(LogSeverity.Important, "This program is a stub...");
            return 0;
        }
    }
}
