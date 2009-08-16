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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Gallio.Common.IO;
using Gallio.Common.Policies;
using Gallio.Common.Reflection;
using Gallio.Runtime.Hosting;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Runs MSTest in-process in a separate AppDomain (without process isolation)
    /// to enable debugging and code coverage.
    /// </summary>
    internal sealed class EmbeddedMSTestCommand : MSTestCommand
    {
        private EmbeddedMSTestCommand()
        {
        }

        public static readonly EmbeddedMSTestCommand Instance = new EmbeddedMSTestCommand();

        public override int Run(string executablePath, string workingDirectory,
            MSTestCommandArguments args, TextWriter writer)
        {
            string baseDir = Path.GetDirectoryName(executablePath);

            using (new CurrentDirectorySwitcher(workingDirectory))
            {
                AppDomain appDomain = null;
                try
                {
                    appDomain = AppDomainUtils.CreateAppDomain("MSTest", baseDir, executablePath + @".config", false);

                    var extendedArgs = args.Copy();
                    extendedArgs.NoIsolation = true;

                    writer.WriteLine("\"{0}\" {1}\n", executablePath, extendedArgs);

                    // The eventual working directory to use should be the MSTest TestDir's Out
                    // directory which is what will ordinarily happen when the '/noisolation'
                    // switch is not used.  Unfortunately we have to hack it when we do use the switch.
                    string workingDirectoryToUseWhenCreated = null;
                    if (args.ResultsFile != null)
                        workingDirectoryToUseWhenCreated = Path.Combine(Path.Combine(Path.GetDirectoryName(args.ResultsFile), MSTestRunner.PreferredTestDir), "Out");

                    Type launcherType = typeof(Launcher);
                    Launcher launcher = (Launcher) appDomain.CreateInstanceFromAndUnwrap(
                        AssemblyUtils.GetFriendlyAssemblyLocation(launcherType.Assembly),
                        launcherType.FullName);
                    return launcher.Run(writer, executablePath, extendedArgs.ToStringArray(), workingDirectoryToUseWhenCreated);
                }
                finally
                {
                    if (appDomain != null)
                        AppDomain.Unload(appDomain);
                }
            }
        }

        // MSTest uses Console.OpenStandardOutput() to get a reference to the
        // real standard output stream.  This makes it difficult to redirect output
        // elsewhere during debugging.  Annoyingly it also causes a console
        // window to appear and it may cause other output to become garbled.
        //
        // There isn't much we can do about this without messing around with Win32
        // file handles.
        [Serializable]
        private sealed class Launcher : MarshalByRefObject
        {
            private const int StdOut = -11;
            
            public int Run(TextWriter outputWriter, string executable, string[] args, string workingDirectoryToUseWhenCreated)
            {
                string outputTempFile = null;
                FileStream outputStream = null;
                try
                {
                    outputTempFile = SpecialPathPolicy.For<EmbeddedMSTestCommand>().CreateTempFileWithUniqueName().FullName;
                    outputStream = new FileStream(outputTempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete);

                    bool wasRedirected = false;
                    IntPtr oldHandle = GetStdHandle(StdOut);
                    try
                    {
                        wasRedirected = SetStdHandle(StdOut, outputStream.SafeFileHandle.DangerousGetHandle());

                        if (!wasRedirected)
                            outputWriter.WriteLine("MSTest output not available because the output stream could not be redirected.");

                        SetWorkingDirectoryWhenCreated(workingDirectoryToUseWhenCreated);
                        return InvokeEntryPoint(executable, args);
                    }
                    finally
                    {
                        if (wasRedirected)
                            SetStdHandle(StdOut, oldHandle);
                    }
                }
                finally
                {
                    if (outputStream != null)
                    {
                        outputStream.Position = 0;

                        char[] buffer = new char[4096];
                        using (StreamReader outputReader = new StreamReader(outputStream))
                        {
                            int count;
                            while ((count = outputReader.Read(buffer, 0, buffer.Length)) > 0)
                                outputWriter.Write(buffer, 0, count);
                        }

                        outputStream.Close();
                    }

                    if (outputTempFile != null)
                        File.Delete(outputTempFile);
                }
            }

            private static void SetWorkingDirectoryWhenCreated(string workingDirectoryToUseWhenCreated)
            {
                if (workingDirectoryToUseWhenCreated == null)
                    return;

                // Install a hook that will automatically set the current working
                // directory shortly after it is created around the time MSTest loads the
                // test assembly.  We do this because MSTest does not configure the current
                // working directory to point to the TestDir\Out folder when the '/noisolation'
                // switch is provided unlike when isolation is used.  Consequently tests will
                // have difficulty accessing any deployment items because the relative paths
                // will be resolved incorrectly.
                AssemblyLoadEventHandler handler = null;
                handler = (sender, e) =>
                {
                    if (Directory.Exists(workingDirectoryToUseWhenCreated))
                    {
                        Environment.CurrentDirectory = workingDirectoryToUseWhenCreated;
                        AppDomain.CurrentDomain.AssemblyLoad -= handler;
                    }
                };

                AppDomain.CurrentDomain.AssemblyLoad += handler;
            }

            private static int InvokeEntryPoint(string executable, string[] args)
            {
                // Unfortunately we cannot use AppDomain.ExecuteAssembly because it has
                // the nasty side-effect of causing the redirected console streams to
                // be reset to defaults.  So instead we simply call the entrypoint directly.
                Assembly assembly = Assembly.LoadFrom(executable);
                return (int)assembly.EntryPoint.Invoke(null, new object[] { args });
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetStdHandle(int nStdHandle);
        }
    }
}
