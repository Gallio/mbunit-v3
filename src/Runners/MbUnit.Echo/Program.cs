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
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.IO;

namespace MbUnit.Echo
{
    /// <summary>
    /// The MbUnit console test runner program.
    /// </summary>
    public class Program
    {
        [STAThread]
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static int Main(string[] args)
        {
            IRichConsole console = NativeConsole.Instance;

            try
            {
                using (MainClass main = new MainClass(console))
                {
                    return main.Run(args);
                }
            }
            finally
            {
                console.ResetColor();
            }
        }
    }
}
