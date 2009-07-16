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
using System.IO;
using Gallio.DLRIntegration.Model;
using Gallio.Model;
using Gallio.Runtime.Logging;
using Microsoft.Scripting.Hosting;

namespace Gallio.RSpecAdapter.Model
{
    internal class RSpecTestDriver : DLRTestDriver
    {
        private readonly DirectoryInfo pluginBaseDirectory;

        public RSpecTestDriver(ILogger logger, DirectoryInfo pluginBaseDirectory)
            : base(logger)
        {
            this.pluginBaseDirectory = pluginBaseDirectory;
        }

        protected override FileInfo GetTestDriverScriptFile(TestPackage testPackage)
        {
            return new FileInfo(Path.Combine(pluginBaseDirectory.FullName, @"Scripts\driver.rb"));
        }

        protected override void ConfigureIronRuby(LanguageSetup languageSetup, IList<string> libraryPaths)
        {
            libraryPaths.Add(Path.Combine(pluginBaseDirectory.FullName, "Scripts"));
            libraryPaths.Add(Path.Combine(pluginBaseDirectory.FullName, @"libs\rspec-1.2.7\lib"));
#if DEBUG
            libraryPaths.Add(Path.Combine(pluginBaseDirectory.FullName, @"..\libs\rspec-1.2.7\lib"));
#endif
        }
    }
}
