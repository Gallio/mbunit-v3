using System;
using System.IO;
using Gallio.Common.Policies;
using Gallio.Common.Xml;

namespace Gallio.Icarus.Options
{
    internal class OptionsManager
    {
        private static Settings settings = new Settings();

        public static Settings Settings
        {
            get { return settings; }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(Paths.SettingsFile))
                    settings = XmlSerializationUtils.LoadFromXml<Settings>(Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while loading Icarus settings file.", ex);
            }
        }

        public static void Save()
        {
            try
            {
                // create folder, if necessary
                if (!Directory.Exists(Paths.IcarusAppDataFolder))
                    Directory.CreateDirectory(Paths.IcarusAppDataFolder);

                XmlSerializationUtils.SaveToXml(Settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while saving Icarus settings file.", ex);
            }
        }
    }
}
