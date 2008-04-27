// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Host
{
    /// <summary>
    /// Command-line arguments class for the host process.
    /// </summary>
    public class HostArguments
    {
        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            Description = "The name of the IPC port to create and listen on for requests.",
            ValueLabel = "portName",
            LongName="ipc-port",
            ShortName = "ipcp"
            )]
        public string IpcPortName;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            Description = "The TCP port number to listen on for requests.",
            ValueLabel = "portNumber",
            LongName = "tcp-port",
            ShortName = "tcpp"
            )]
        public int TcpPortNumber = -1;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            Description = "The number of seconds to wait before shutting down the host application automatically if no Ping messages are received.  May be 0 to disable the timeout altogether.  This timeout mechanism constitutes a watchdog timer that is intended to protect the test runner apparatus from connection drop-outs.",
            ValueLabel = "seconds",
            LongName = "timeout",
            ShortName = "t"
            )]
        public int TimeoutSeconds = 15;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            Description = "When specified, automatically exits when the process specified id terminates.",
            ValueLabel = "processId",
            LongName = "owner-process",
            ShortName = "op"
            )]
        public int OwnerProcessId = -1;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "abd",
            LongName = "application-base-directory",
            Description = "The application base directory.",
            ValueLabel = "dir"
            )]
        public string ApplicationBaseDirectory;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "cf",
            LongName = "configuration-file",
            Description = "The XML configuration file.",
            ValueLabel = "file"
            )]
        public string ConfigurationFile;

        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "sc",
             LongName = "shadow-copy",
             Description = "Enable shadow copying of assemblies."
             )]
        public bool ShadowCopy;
        
        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text.",
             Synonyms = new string[] { "?" }
             )]
        public bool Help;
    }
}
