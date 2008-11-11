using System;
using System.IO;
using Microsoft.Win32;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Finds the AutoCAD install location.
    /// </summary>
    internal static class AcadLocator
    {
        private const string ExeName = "acad.exe";

        /// <summary>
        /// Gets a path to "acad.exe".
        /// </summary>
        /// <returns>The path to "acad.exe".</returns>
        /// <exception cref="FileNotFoundException">If "acad.exe" is not found.</exception>
        public static string GetAcadLocation()
        {
            string path = GetMostRecentlyUsed();
            if (!File.Exists(path))
                throw new FileNotFoundException("Unable to find AutoCAD.");
            return path;
        }

        private static string GetMostRecentlyUsed()
        {
            using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Autodesk\DWGCommon\shellex\Apps"))
            {
                if (regKey == null)
                    return null;

                string subKeyName = regKey.GetValue(null) as string;
                if (subKeyName == null)
                    return null;

                using (RegistryKey subKey = regKey.OpenSubKey(subKeyName))
                {
                    if (subKey == null)
                        return null;

                    string launchCommand = subKey.GetValue("OpenLaunch") as string;
                    if (launchCommand == null)
                        return null;

                    int acadIndex = launchCommand.IndexOf(ExeName, StringComparison.OrdinalIgnoreCase);
                    if (acadIndex < 0)
                        return null;

                    int startIndex = (launchCommand[0] == '"') ? 1 : 0;
                    int length = acadIndex + ExeName.Length - startIndex;

                    return launchCommand.Substring(startIndex, length);
                }
            }
        }
    }
}
