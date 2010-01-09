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

using System.Collections.Generic;
using Gallio.Common.Concurrency;

namespace Gallio.NCoverIntegration.Tools
{
    public class NCoverV2Tool : NCoverTool
    {
        public static readonly NCoverV2Tool Instance = new NCoverV2Tool();

        public override string Name
        {
            get { return "NCover v2"; }
        }

        public override string GetInstallDir()
        {
            string version, installDir;
            return GetNCoverInstallInfoFromRegistry("2.", out version, out installDir) ? installDir : null;
        }

        protected override bool RequiresDotNet20()
        {
            return true;
        }

        protected override ProcessTask CreateMergeTask(IList<string> sources, string destination)
        {
            return CreateNCoverExplorerConsoleMergeTask("", sources, destination);
        }
    }
}
