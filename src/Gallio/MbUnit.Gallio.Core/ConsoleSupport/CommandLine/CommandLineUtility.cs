// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System;

namespace MbUnit.Core.ConsoleSupport.CommandLine
{
    /// <summary>
    /// Useful Stuff.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Command line parsing code from Peter Halam, 
    /// http://www.gotdotnet.com/community/usersamples/details.aspx?sampleguid=62a0f27e-274e-4228-ba7f-bc0118ecc41e
    /// </para>
    /// </remarks>
    public static class CommandLineUtility
    {
        /// <summary>
        /// The System Defined new line string.
        /// </summary>
        public const string NewLine = "\r\n";
        
        /// <summary>
        /// Parses Command Line Arguments. 
        /// Errors are output on SharedConsole.Error.
        /// Use CommandLineArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="arguments"> The actual arguments. </param>
        /// <param name="destination"> The resulting parsed arguments. </param>
        /// <param name="console">The console</param>
        public static void ParseCommandLineArguments(string [] arguments, object destination, IRichConsole console)
        {
            ParseCommandLineArguments(arguments, destination, console.Error.WriteLine);
        }
        
        /// <summary>
        /// Parses Command Line Arguments. 
        /// Use CommandLineArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="arguments"> The actual arguments. </param>
        /// <param name="destination"> The resulting parsed arguments. </param>
        /// <param name="reporter"> The destination for parse errors. </param>
        public static void ParseCommandLineArguments(string[] arguments, object destination, ErrorReporter reporter)
        {
            CommandLineArgumentParser parser = new CommandLineArgumentParser(destination.GetType(), reporter);
            if (!parser.Parse(arguments, destination))
				throw new Exception("Parsing failed");
        }

        /// <summary>
        /// Returns a Usage string for command line argument parsing.
        /// Use CommandLineArgumentAttributes to control parsing behaviour.
        /// </summary>
        /// <param name="argumentType"> The type of the arguments to display usage for. </param>
        /// <param name="console">The console</param>
        public static void CommandLineArgumentsUsage(Type argumentType, IRichConsole console)
        {
			new CommandLineArgumentParser(argumentType, null).ShowUsage(new CommandLineOutput(console));
        }
    }
}
