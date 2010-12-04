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
using System.IO;
using Gallio.Common.Concurrency;
using Gallio.Common.Policies;

namespace Gallio.Icarus.Logging
{
    internal class BlackBoxLogger
    {
        private static readonly LockBox<BlackBoxLogger> Logger = new LockBox<BlackBoxLogger>(new BlackBoxLogger());

        public static void Initialize()
        {
            try
            {
                if (File.Exists(Paths.BlackBoxLogFile))
                    File.Delete(Paths.BlackBoxLogFile);
            }
            catch
            {
                // eat any exceptions
            }
        }

        public static void Log(CorrelatedExceptionEventArgs e)
        {
            Logger.Write(bl => LogImpl(e));
        }

        private static void LogImpl(CorrelatedExceptionEventArgs e)
        {
            try
            {
                using (var sw = new StreamWriter(Paths.BlackBoxLogFile, true))
                {
                    sw.WriteLine(string.Format("Unhandled exception reported at: {0}", DateTime.Now));
                    sw.WriteLine(e.Message);
                    sw.WriteLine();
                    sw.WriteLine(e.GetDescription());
                    sw.WriteLine();
                }
            }
            catch
            { 
                // eat any exceptions
            }
        }
    }
}
