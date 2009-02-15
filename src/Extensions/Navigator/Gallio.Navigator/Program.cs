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
using System.Windows.Forms;

namespace Gallio.Navigator
{
    /// <summary>
    /// This program entry point enables the navigator library to function as an external
    /// application protocol handler for Windows and Firefox.  It accepts a single argument
    /// which is the Url to which navigation should take place.
    /// </summary>
    internal class Program : GallioNavigatorClient
    {
        public static int Main(string[] args)
        {
            return new Program().Run(args);
        }

        internal int Run(string[] args)
        {
            if (args.Length != 1)
            {
                ShowHelp();
                return 1;
            }

            return ProcessCommandUrl(args[0]) ? 0 : 1;
        }

        protected virtual void ShowHelp()
        {
            MessageBox.Show("This program is an application protocol handler for Gallio Urls.\nUsage: [url].",
                "Gallio Navigator");
        }
    }
}
