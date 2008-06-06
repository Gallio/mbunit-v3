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

using System;
using System.Collections.Generic;
using System.IO;
using Gallio.Concurrency;
using Gallio.Runtime.Hosting;
using Gallio.Utilities;
using Microsoft.Win32;

namespace Gallio.MSTestAdapter.Model
{
    [Serializable]
    public class MSTestProcess : MarshalByRefObject, IMSTestProcess
    {
        private readonly static string msTestPath;
        private readonly static string msTestDirectory;
        public static readonly MSTestProcess Instance;

        static MSTestProcess()
        {
            msTestPath = FindMSTestPath("9.0");
            if (String.IsNullOrEmpty(msTestPath))
            {
                msTestPath = FindMSTestPath("8.0");
            }
            msTestDirectory = Path.GetDirectoryName(msTestPath);

            Instance = new MSTestProcess();
        }

        private static string FindMSTestPath(string visualStudioVersion)
        {
            using (RegistryKey key =
                Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\VisualStudio\" + visualStudioVersion))
            {
                if (key != null)
                {
                    string visualStudioInstallDir = (string)key.GetValue("InstallDir");
                    if (visualStudioInstallDir != null)
                    {
                        string msTestExecutablePath = Path.Combine(visualStudioInstallDir, "MSTest.exe");
                        if (File.Exists(msTestExecutablePath))
                        {
                            return msTestExecutablePath;
                        }
                    }
                }
            }

            return null;
        }

        public bool Run(MSTestAssembly assemblyTest)
        {
            if (String.IsNullOrEmpty(msTestPath))
            {
                return false;
            }
            
            List<string> arguments = new List<string>();
            arguments.Add(@"/nologo");
            arguments.Add(@"/resultsfile:" + QuoteFilename(assemblyTest.DirectoryName + assemblyTest.ResultsFileName));
            arguments.Add(@"/testlist:" + assemblyTest.TestListName);
            arguments.Add(@"/testmetadata:" + QuoteFilename(assemblyTest.TestMetadataFileName));
            arguments.Add(@"/noisolation");

            AppDomain msTetsAppDomain = AppDomainUtils.CreateAppDomain("MSTest", msTestDirectory, msTestPath + ".config", false);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Running this code will prevent the result file from being generated :(
                //MSTestProcess proxy = (MSTestProcess)msTetsAppDomain.CreateInstanceFromAndUnwrap(new Uri(typeof(MSTestProcess).Assembly.GetName().CodeBase).LocalPath, typeof(MSTestProcess).FullName);
                //proxy.DisableConsoleOutput();
                using (new CurrentDirectorySwitcher(assemblyTest.DirectoryName))
                {
                    // A System.UriFormatException is thrown here for some reason
                    msTetsAppDomain.ExecuteAssembly(msTestPath, null, arguments.ToArray());
                }
                return true;
            }
            else
            {
                ProcessTask msTestProcessTask = new ProcessTask(
                msTestPath,
                @" /nologo"
                + " /resultsfile:"
                + QuoteFilename(assemblyTest.ResultsFileName)
                + " /testlist:"
                + assemblyTest.TestListName
                + " /testmetadata:"
                + QuoteFilename(assemblyTest.TestMetadataFileName),
                assemblyTest.DirectoryName);
                return msTestProcessTask.Run(null);
            }            
        }

        private static string QuoteFilename(string filename)
        {
            return "\"" + filename + "\"";
        }

        public void DisableConsoleOutput()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
            Console.SetIn(TextReader.Null);
        }
    }
}
