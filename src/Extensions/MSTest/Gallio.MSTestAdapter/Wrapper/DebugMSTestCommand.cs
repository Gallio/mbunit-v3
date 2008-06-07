using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Gallio.Reflection;
using Gallio.Runtime.Hosting;
using Gallio.Utilities;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// An MSTest command implementation that is designed to allow MSTest tests
    /// to run in the debugger with no additional process isolationd.
    /// </summary>
    internal class DebugMSTestCommand : IMSTestCommand
    {
        public static readonly DebugMSTestCommand Instance = new DebugMSTestCommand();

        private DebugMSTestCommand()
        {
        }

        /// <inheritdoc />
        public int Run(string workingDirectory, MSTestCommandArguments args,
            TextWriter outputWriter, TextWriter errorWriter)
        {
            string executable = MSTestResolver.FindDefaultMSTestPath();
            if (executable == null)
                return -1;
            string baseDir = Path.GetDirectoryName(executable);

            using (new CurrentDirectorySwitcher(workingDirectory))
            {
                AppDomain appDomain = null;
                try
                {
                    appDomain = AppDomainUtils.CreateAppDomain("MSTest", baseDir, executable + @".config", false);

                    var extendedArgs = args.Copy();
                    extendedArgs.NoIsolation = true;

                    Type launcherType = typeof(Launcher);
                    Launcher launcher = (Launcher) appDomain.CreateInstanceFromAndUnwrap(
                        AssemblyUtils.GetFriendlyAssemblyLocation(launcherType.Assembly),
                        launcherType.FullName);
                    return launcher.Run(outputWriter, executable, extendedArgs.ToStringArray());
                }
                finally
                {
                    if (appDomain != null)
                        AppDomain.Unload(appDomain);
                }
            }
        }

        // MSBuild uses Console.OpenStandardOutput() to get a reference to the
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
            
            public int Run(TextWriter outputWriter, string executable, string[] args)
            {
                string outputTempFile = null;
                FileStream outputStream = null;
                try
                {
                    outputTempFile = Path.GetTempFileName();
                    outputStream = new FileStream(outputTempFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete);

                    bool wasRedirected = false;
                    IntPtr oldHandle = GetStdHandle(StdOut);
                    try
                    {
                        wasRedirected = SetStdHandle(StdOut, outputStream.SafeFileHandle.DangerousGetHandle());

                        if (!wasRedirected)
                            outputWriter.WriteLine("MSTest output not available because the output stream could not be redirected.");

                        // Unfortunately we cannot use AppDomain.ExecuteAssembly because it has
                        // the nasty side-effect of causing the redirected console streams to
                        // be reset to defaults.  So instead we simply call the entrypoint directly.
                        Assembly assembly = Assembly.LoadFrom(executable);
                        return (int)assembly.EntryPoint.Invoke(null, new object[] { args });
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

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern IntPtr GetStdHandle(int nStdHandle);
        }
    }
}
