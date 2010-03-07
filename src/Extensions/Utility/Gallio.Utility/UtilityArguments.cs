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
using System.IO;
using Gallio.Common.Collections;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Logging;

namespace Gallio.Utility
{
    public class UtilityArguments
    {
        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "pd",
            LongName = "plugin-directory",
            Description = "Additional plugin directories to search recursively",
            ValueLabel = "dir"
            )]
        public string[] PluginDirectories = EmptyArray<string>.Instance;

        #region Misc arguments

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text.",
             Synonyms = new[] { "?" })]
        public bool Help;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "v",
             LongName = "verbosity",
             Description = "Controls the level of detail of the information to display.",
             ValueLabel = "level")]
        public Verbosity Verbosity = Verbosity.Normal;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "np",
             LongName = "no-progress",
             Description = "Do not display progress messages during execution.")]
        public bool NoProgress;

        #endregion

        #region Commands

        [DefaultCommandLineArgument(
            CommandLineArgumentFlags.Multiple,
            Description = "Specifies the utility command to perform and its arguments.  The list of available utility commands follows.  Specify a command name and /? for help about its options.",
            ValueLabel = "command and args...",
            ConsumeUnrecognizedSwitches = true)]
        public string[] CommandAndArguments;

        #endregion
    }
}