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
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Web;
using System.Diagnostics;

namespace Gallio.Navigator
{
    /// <summary>
    /// Specifies the name of a command and its arguments to execute.
    /// </summary>
    public sealed class GallioNavigatorCommand
    {
        /// <summary>
        /// The Gallio protocol sheme name.
        /// </summary>
        public const string ProtocolScheme = "gallio";

        /// <summary>
        /// The "navigateTo" command name.
        /// </summary>
        public const string NavigateToCommandName = "navigateTo";

        private readonly string name;
        private readonly NameValueCollection arguments;

        /// <summary>
        /// Creates a command request.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="arguments">The command arguments as key/value pairs.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="arguments"/> is null</exception>
        public GallioNavigatorCommand(string name, NameValueCollection arguments)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (arguments == null)
                throw new ArgumentNullException("args");

            this.name = name;
            this.arguments = arguments;
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the command arguments.
        /// </summary>
        public NameValueCollection Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Converts the command to a Uri.
        /// </summary>
        public string ToUri()
        {
            StringBuilder result = new StringBuilder();
            result.Append(ProtocolScheme);
            result.Append(':');
            result.Append(name);

            for (int i = 0; i < arguments.Count; i++)
            {
                result.Append(i == 0 ? '?' : '&');
                result.Append(Uri.EscapeDataString(arguments.GetKey(i)));
                result.Append('=');
                result.Append(Uri.EscapeDataString(arguments[i]));
            }

            return result.ToString();
        }

        /// <summary>
        /// Parses a command request from a Uri.
        /// </summary>
        /// <param name="uriString">The uri.</param>
        /// <returns>The command request or null if it could not be parsed.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uriString"/> is null.</exception>
        public static GallioNavigatorCommand ParseUri(string uriString)
        {
            if (uriString == null)
                throw new ArgumentNullException("uriString");

            try
            {
                Uri uri = new Uri(uriString);
                if (uri.Scheme == ProtocolScheme)
                {
                    string commandName = uri.AbsolutePath;
                    NameValueCollection commandArguments = HttpUtility.ParseQueryString(uri.Query);
                    return new GallioNavigatorCommand(commandName, commandArguments);
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <returns>True if the command succeeded.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="navigator"/> is null.</exception>
        public bool Execute(IGallioNavigator navigator)
        {
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            try
            {
                switch (name)
                {
                    case NavigateToCommandName:
                        string path = arguments["path"];
                        int lineNumber = GetIntArgument("line", 0);
                        int columnNumber = GetIntArgument("column", 0);

                        return navigator.NavigateTo(path, lineNumber, columnNumber);
                }

                Debug.WriteLine(String.Format("Gallio did not understand the command: {0}.", name));
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Gallio could not perform requested service due to an exception: {0}.", ex));
                return false;
            }
        }

        /// <summary>
        /// Gets the value of an integer argument.
        /// </summary>
        /// <param name="key">The argument key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value.</returns>
        public int GetIntArgument(string key, int defaultValue)
        {
            string value = arguments[key];
            if (value == null)
                return defaultValue;

            return int.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Creates a navigateTo command.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lineNumber">The line number, or 0 if unspecified.</param>
        /// <param name="columnNumber">The column number, or 0 if unspecified.</param>
        /// <returns>The command.</returns>
        public static GallioNavigatorCommand CreateNavigateToCommand(string path, int lineNumber, int columnNumber)
        {
            return new GallioNavigatorCommand(NavigateToCommandName, new NameValueCollection()
            {
                { "path", path },
                { "line", lineNumber.ToString() },
                { "column", columnNumber.ToString() }
            });
        }
    }
}
