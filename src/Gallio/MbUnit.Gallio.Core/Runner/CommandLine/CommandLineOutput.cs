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
using System.IO;

namespace MbUnit.Core.Runner.CommandLine
{
    ///<summary>
    /// Responsible for creating output.
    ///</summary>
    public class CommandLineOutput
    {
        private readonly TextWriter _output;

        ///<summary>
        /// Initializes new instance of CommandLineOutput.
        ///</summary>
        public CommandLineOutput()
            : this(Console.Out)
        { }

        ///<summary>
        /// Initializes new instance of CommandLineOutput that outputs to specified stream.
        ///</summary>
        ///<param name="output"></param>
        public CommandLineOutput(TextWriter output)
        {
            _output = output;
        }

        ///<summary>
        /// Outpus argument long and short names.
        ///</summary>
        ///<param name="longName">Long name.</param>
        ///<param name="shortName">Short name.</param>
        public void PrintArgumentName(string longName, string shortName)
        {
            _output.Write(Space(2));
            _output.Write("[/");
            _output.Write(longName);
            _output.Write("|/");
            _output.Write(shortName);
            _output.Write("]");
        }

        ///<summary>
        /// Outputs argument type.
        ///</summary>
        ///<param name="type">Argument Type.</param>
        public void PrintArgumentType(Type type)
        {
            switch (type.Name)
            {
                case "String":
                    _output.Write(":<string>");
                    break;
                case "Int32":
                    _output.Write(":<int>");
                    break;
                case "UInt32":
                    _output.Write(":<uint>");
                    break;
                case "Boolean":
                    _output.Write("[+|-]");
                    break;
                default:
                    if (type.IsEnum)
                        PrintEnumArgumentType(type);
                    else
                        throw new ArgumentException("Unexpected type.");
                    break;
            }
        }

        ///<summary>
        /// Prints out a new line.
        ///</summary>
        public void NewLine()
        {
            _output.WriteLine();
        }

        ///<summary>
        /// Outputs argument description.
        ///</summary>
        ///<param name="description">Argument description.</param>
        public void PrintDescription(string description)
        {
            PrintText(description, 4);
        }

        ///<summary>
        /// Outputs text with specified indentation.
        ///</summary>
        ///<param name="text">Text to output.</param>
        ///<param name="indentation">Number of blank spaces before the start of the text.</param>
        public void PrintText(string text, int indentation)
        {
            int maxLength = 80 - indentation;
            while (text.Length > maxLength)
            {
                int pos = text.LastIndexOf(' ', maxLength + 1);
                _output.Write(Space(indentation));
                _output.Write(text.Substring(0, pos));
                NewLine();
                text = text.Substring(pos + 1);
            }
            _output.Write(Space(indentation));
            _output.Write(text);
        }

        private void PrintEnumArgumentType(Type type)
        {
            string[] enumValues = Enum.GetNames(type);
            _output.Write(":{");
            for (int ndx = 0; ndx < enumValues.Length; ndx++)
            {
                if (ndx > 0)
                    _output.Write("|");
                _output.Write(enumValues[ndx]);
            }
            _output.Write("}");
        }

        private static string Space(int spaceCount)
        {
            return new string(' ', spaceCount);
        }
    }
}