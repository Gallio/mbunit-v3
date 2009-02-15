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

namespace Gallio.MSTestAdapter.Wrapper
{
    internal class MSTestCommandArguments
    {
        public bool NoLogo { get; set; }
        public bool NoIsolation { get; set; }
        public string ResultsFile { get; set; }
        public string TestList { get; set; }
        public string TestMetadata { get; set; }
        public string RunConfig { get; set; }

        public MSTestCommandArguments Copy()
        {
            MSTestCommandArguments copy = new MSTestCommandArguments();
            copy.NoLogo = NoLogo;
            copy.NoIsolation = NoIsolation;
            copy.ResultsFile = ResultsFile;
            copy.TestList = TestList;
            copy.TestMetadata = TestMetadata;
            copy.RunConfig = RunConfig;
            return copy;
        }

        public string[] ToStringArray()
        {
            List<string> args = new List<string>();

            if (NoLogo)
                AddArgument(args, "/nologo", null);
            if (NoIsolation)
                AddArgument(args, "/noisolation", null);
            if (ResultsFile != null)
                AddArgument(args, "/resultsfile", ResultsFile);
            if (TestList != null)
                AddArgument(args, "/testlist", TestList);
            if (TestMetadata != null)
                AddArgument(args, "/testmetadata", TestMetadata);
            if (TestMetadata != null)
                AddArgument(args, "/runconfig", RunConfig);

            return args.ToArray();
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (string arg in ToStringArray())
            {
                if (result.Length != 0)
                    result.Append(' ');
                result.Append('"').Append(arg).Append('"');
            }

            return result.ToString();
        }

        private static void AddArgument(ICollection<string> args, string argName, string argValue)
        {
            if (argValue != null)
                args.Add(argName + ":" + argValue);
            else
                args.Add(argName);
        }
    }
}
