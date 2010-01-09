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
using Gallio.Ambience.Impl;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Ambience.Server
{
    /// <summary>
    /// Command-line arguments for the Ambience server.
    /// </summary>
    public class AmbienceServerArguments
    {
        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text.",
             Synonyms = new string[] { "?" }
             )]
        public bool Help;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "p",
            LongName = "port",
            Description = "Specifies the server port number.  Default is " + Constants.DefaultPortNumberString + ".",
            ValueLabel = "number"
            )]
        public int Port = Constants.DefaultPortNumber;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "db",
            LongName = "database",
            Description = "Specifies the database file name.  Default is a file called Default.db in the Gallio\\Gallio.Ambience subdirectory of Common Application Data.",
            ValueLabel = "path"
            )]
        public string DatabasePath;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "u",
            LongName = "username",
            Description = "Specifies the username that clients must authenticate with.  Default is '" + Constants.AnonymousUserName + "'.",
            ValueLabel = "username"
            )]
        public string UserName = Constants.AnonymousUserName;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "pw",
            LongName = "password",
            Description = "Specifies the username that clients must authenticate with.  Default is '" + Constants.AnonymousPassword + "'.",
            ValueLabel = "password"
            )]
        public string Password = Constants.AnonymousPassword;
    }
}
