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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Web;

namespace Gallio.Navigator
{
    /// <summary>
    /// Abstract class for a service that uses <see cref="IGallioNavigator"/>.
    /// </summary>
    public abstract class GallioNavigatorClient
    {
        /// <summary>
        /// The Gallio protocol sheme name.
        /// </summary>
        protected const string ProtocolScheme = "gallio";

        private const string NavigateToCommandName = "navigateTo";

        private IGallioNavigator navigator;

        /// <summary>
        /// Gets the navigator service.
        /// </summary>
        protected internal IGallioNavigator Navigator
        {
            get
            {
                lock (this)
                {
                    if (navigator == null)
                        navigator = new GallioNavigatorImpl();
                    return navigator;
                }
            }
            set
            {
                lock (this)
                    navigator = value;
            }
        }

        /// <summary>
        /// Performs the command requested by the url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The true if the command completed successfull.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is null.</exception>
        protected internal bool ProcessCommandUrl(string url)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            try
            {
                Uri uri = new Uri(url);
                if (uri.Scheme == ProtocolScheme)
                {
                    string commandName = uri.AbsolutePath;
                    NameValueCollection args = HttpUtility.ParseQueryString(uri.Query);

                    switch (commandName)
                    {
                        case NavigateToCommandName:
                            string path = args["path"];
                            int lineNumber = GetValueOrDefault(args, "line", 0);
                            int columnNumber = GetValueOrDefault(args, "column", 0);

                            return HandleNavigateToCommand(path, lineNumber, columnNumber);
                    }
                }

                Debug.WriteLine(String.Format("Gallio did not understand the url: {0}.", url));
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Format("Gallio could not perform requested service due to an exception: {0}.", ex));
                return false;
            }
        }

        /// <summary>
        /// Performs the navigation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Subclasses may override to do additional processing.
        /// </para>
        /// </remarks>
        protected virtual bool HandleNavigateToCommand(string path, int lineNumber, int columnNumber)
        {
            return Navigator.NavigateTo(path, lineNumber, columnNumber);
        }

        private static int GetValueOrDefault(NameValueCollection collection, string key, int defaultValue)
        {
            string value = collection[key];
            if (value == null)
                return defaultValue;

            return int.Parse(value, NumberStyles.None, CultureInfo.InvariantCulture);
        }
    }
}
