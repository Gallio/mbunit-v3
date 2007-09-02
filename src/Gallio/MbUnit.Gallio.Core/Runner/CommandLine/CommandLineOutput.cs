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
using System.Text;

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
        {}

        ///<summary>
        /// Initializes new instance of CommandLineOutput that outputs to specified stream.
        ///</summary>
        ///<param name="output"></param>
        public CommandLineOutput(TextWriter output)
        {
            _output = output;
        }

        ///<summary>
        /// Output Stream
        ///</summary>
        public TextWriter Output
        {
            get { return _output; }
        }

        ///<summary>
        /// Prints out a new line.
        ///</summary>
        public void NewLine()
        {
            _output.WriteLine();
        }

        ///<summary>
        /// Outputs text with specified indentation.
        ///</summary>
        ///<param name="text">Text to output.</param>
        ///<param name="indentation">Number of blank spaces before the start of the text.</param>
        public void PrintText(string text, int indentation)
        {
            int maxLength = 79 - indentation;
            while (text.Length > maxLength)
            {
                int pos = text.LastIndexOf(' ', maxLength + 1);
                _output.Write(Space(indentation));
                _output.Write(text.Substring(0, pos));
                NewLine();
                text = text.Substring(pos + 1);
            }
            _output.Write(Space(indentation));
            _output.WriteLine(text);
        }

        ///<summary>
        /// Outputs text with specified indentation.
        ///</summary>
        ///<param name="text">Text to output.</param>
        ///<param name="firstLineIndent">Number of blank spaces before the start of the text.</param>
        ///<param name="indentation">Number of blank spaces before the start of the text.</param>
        private void PrintText(string text, int firstLineIndent, int indentation)
        {
            int maxLength = 80 - firstLineIndent;
            if (text.Length > maxLength)
            {
                int pos = text.LastIndexOf(' ', maxLength + 1);
                _output.Write(Space(firstLineIndent));
                _output.Write(text.Substring(0, pos));
                _output.WriteLine();
                text = text.Substring(pos + 1);
                PrintText(text, indentation);
            }
            else
                PrintText(text, firstLineIndent);
        }

        ///<summary>
        /// Output help for a specified argument.
        ///</summary>
        ///<param name="longName">Argument long name.</param>
        ///<param name="shortName">Argument short name.</param>
        ///<param name="description">Argument description.</param>
        ///<param name="valueType">Argument value type.</param>
        public void PrintArgumentHelp(string longName, string shortName, string description, string valueType)
        {
            StringBuilder argumentHelp = new StringBuilder("/");
            argumentHelp.Append(longName);
            if (!string.IsNullOrEmpty(valueType))
            {
                argumentHelp.Append(":<");
                argumentHelp.Append(valueType);
                argumentHelp.Append(">");
            }
            if (argumentHelp.Length > 17)
            {
                PrintText(argumentHelp.ToString(), 2);
                PrintText(CreateDescriptionWithShortName(description, shortName), 21);
            }
            else
            {
                argumentHelp.Append(Space(18 - longName.Length));
                argumentHelp.Append(CreateDescriptionWithShortName(description, shortName));
                PrintText(argumentHelp.ToString(), 2, 21);
            }
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

        private static string CreateDescriptionWithShortName(string description, string shortName)
        {
            return string.Format("{0} (Short form: /{1})", description, shortName);
        }
    }
}