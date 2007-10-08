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
using MbUnit.Properties;

namespace MbUnit.Core.ConsoleSupport.CommandLine
{
    ///<summary>
    /// Responsible for creating output.
    ///</summary>
    public class CommandLineOutput
    {
        private const int LeftMargin = 2;
        private const int HangingIndent = 19;
        private const int Gutter = 2;

        private readonly TextWriter output;
        private int lineLength;

        ///<summary>
        /// Initializes new instance of CommandLineOutput.
        ///</summary>
        ///<param name="console">The console</param>
        public CommandLineOutput(IRichConsole console)
            : this(console.Out, console.Width)
        {
        }

        ///<summary>
        /// Initializes new instance of CommandLineOutput that outputs to specified stream.
        ///</summary>
        ///<param name="output"></param>
        public CommandLineOutput(TextWriter output)
        {
            this.output = output;
            lineLength = 80;
        }

        ///<summary>
        ///</summary>
        ///<param name="output"></param>
        ///<param name="width"></param>
        public CommandLineOutput(TextWriter output, int width)
        {
            this.output = output;
            lineLength = width;
        }

        ///<summary>
        /// Output Stream
        ///</summary>
        public TextWriter Output
        {
            get { return output; }
        }

        ///<summary>
        /// Maximum line length allowed before the text will be wraped.
        ///</summary>
        public int LineLength
        {
            get { return lineLength; }
            set { lineLength = value; }
        }

        ///<summary>
        /// Prints out a new line.
        ///</summary>
        public void NewLine()
        {
            output.WriteLine();
        }

        /// <summary>
        /// Outputs text with specified indentation.
        /// </summary>
        /// <param name="text">Text to output possibly including newlines.</param>
        /// <param name="indentation">Number of blank spaces to indent the first line.</param>
        public void PrintText(string text, int indentation)
        {
            PrintText(text, indentation, indentation);
        }

        /// <summary>
        /// Outputs text with specified indentation.
        /// </summary>
        /// <param name="text">Text to output possibly including newlines.</param>
        /// <param name="indentation">Number of blank spaces to indent all but the first line.</param>
        /// <param name="firstLineIndent">Number of blank spaces to indent the first line.</param>
        public void PrintText(string text, int indentation, int firstLineIndent)
        {
            int currentIndentation = firstLineIndent;

            text = text.Trim();
            while (text.Length != 0)
            {
                int maxLength = lineLength - currentIndentation - 1;

                int pos = text.IndexOf('\n');
                if (pos < 0)
                {
                    if (text.Length <= maxLength)
                    {
                        output.Write(Space(currentIndentation));
                        output.WriteLine(text);
                        break;
                    }
                    else
                    {
                        pos = text.LastIndexOf(' ', maxLength);
                        if (pos < 0)
                            pos = maxLength;
                    }
                }

                output.Write(Space(currentIndentation));
                output.WriteLine(text.Substring(0, pos).TrimEnd());

                if (pos == text.Length - 1)
                    break;

                text = text.Substring(pos + 1).TrimStart();
                currentIndentation = indentation;
            }
        }

        /// <summary>
        /// Prints help for a specified argument.
        /// </summary>
        /// <param name="prefix">The argument prefix, such as "/", or null or empty if none.</param>
        /// <param name="longName">The argument's long name, or null or empty if none.</param>
        /// <param name="shortName">The argument's short short name, or null or empty if none.</param>
        /// <param name="description">The argument's description, or null or empty if none.</param>
        /// <param name="valueLabel">The argument's value label such as "path", or null or empty if none.</param>
        /// <param name="valueType">The argument's value type, or null if none.</param>
        public void PrintArgumentHelp(string prefix, string longName, string shortName, string description, string valueLabel, Type valueType)
        {
            StringBuilder argumentHelp = new StringBuilder();

            if (!string.IsNullOrEmpty(prefix))
                argumentHelp.Append(prefix);

            if (! string.IsNullOrEmpty(longName))
            {
                argumentHelp.Append(longName);

                if (! string.IsNullOrEmpty(valueLabel))
                    argumentHelp.Append(':');
            }

            if (! string.IsNullOrEmpty(valueLabel))
            {
                argumentHelp.Append('<');
                argumentHelp.Append(valueLabel);
                argumentHelp.Append('>');
            }

            if (argumentHelp.Length > HangingIndent - Gutter)
                argumentHelp.Append('\n');
            else
                argumentHelp.Append(Space(HangingIndent - argumentHelp.Length));

            if (! string.IsNullOrEmpty(description))
                argumentHelp.Append(description);

            if (valueType != null && valueType.IsEnum)
            {
                argumentHelp.Append(@"  ");
                AppendEnumerationValues(argumentHelp, valueType);
            }

            if (!string.IsNullOrEmpty(shortName))
            {
                argumentHelp.Append(@"  ");
                argumentHelp.AppendFormat(Resources.CommandLineOutput_ShortForm, (prefix ?? @"") + shortName);
            }

            PrintText(argumentHelp.ToString(), HangingIndent + LeftMargin, LeftMargin);
        }

        private static void AppendEnumerationValues(StringBuilder builder, Type valueType)
        {
            builder.Append(Resources.CommandLineOutput_AvailableOptions);

            string[] values = Enum.GetNames(valueType);
            for (int i = 0; i < values.Length; i++)
            {
                if (i != 0)
                    builder.Append(@", ");

                builder.Append(@"'");
                builder.Append(values[i]);
                builder.Append(@"'");
            }

            builder.Append('.');
        }

        private static string Space(int spaceCount)
        {
            return new string(' ', spaceCount);
        }
    }
}