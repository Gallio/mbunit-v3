using System;
using System.IO;
using Microsoft.Win32;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Provides services for resolving the path of the MSTest installation.
    /// </summary>
    public static class MSTestResolver
    {
        /// <summary>
        /// Finds the default (most recent version of MSTest).
        /// </summary>
        /// <returns>The full path of the MSTest.exe program, or null if not found</returns>
        public static string FindDefaultMSTestPath()
        {
            return FindMSTestPath("9.0") ?? FindMSTestPath("8.0");
        }

        /// <summary>
        /// Finds the path of a particular version of MSTest.
        /// </summary>
        /// <param name="visualStudioVersion">The visual studio version
        /// (eg. "8.0" or "9.0")</param>
        /// <returns>The full path of the MSTest.exe program, or null if not found</returns>
        public static string FindMSTestPath(string visualStudioVersion)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\VisualStudio\" + visualStudioVersion))
            {
                if (key != null)
                {
                    string visualStudioInstallDir = (string)key.GetValue("InstallDir");
                    if (visualStudioInstallDir != null)
                    {
                        string msTestExecutablePath = Path.Combine(visualStudioInstallDir, "MSTest.exe");
                        if (File.Exists(msTestExecutablePath))
                            return msTestExecutablePath;
                    }
                }
            }

            return null;
        }
    }
}
