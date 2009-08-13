using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Common.Concurrency;
using Gallio.Common.Platform;
using Gallio.Common.Reflection;
using Microsoft.Win32;

namespace Gallio.NCoverIntegration.Tools
{
    public class NCoverV1Tool : NCoverTool
    {
        private const string NCover1ProfilerKey = @"Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}";
        private const string NCover1ProfilerKey64Bit = @"Software\Wow6432Node\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}";

        public static readonly NCoverV1Tool Instance = new NCoverV1Tool();

        public override string Name
        {
            get { return "NCover v1.5.8"; }
        }

        public override string GetInstallDir()
        {
            string pluginDir = Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(NCoverV1Tool).Assembly));
            string ncoverDir = Path.Combine(pluginDir, @"libs\NCover");
            if (Directory.Exists(ncoverDir))
                return ncoverDir;

#if DEBUG
            ncoverDir = Path.GetFullPath(Path.Combine(pluginDir, @"..\..\libs\NCover"));
            if (Directory.Exists(ncoverDir))
                return ncoverDir;
#endif

            return null;
        }

        protected override void BuildNCoverConsoleArguments(StringBuilder result, string executablePath, string arguments, string workingDirectory, string ncoverArguments, string ncoverCoverageFile)
        {
            base.BuildNCoverConsoleArguments(result, executablePath, arguments, workingDirectory, ncoverArguments, ncoverCoverageFile);

            if (!ncoverArguments.Contains("//l"))
                result.Append(" //q");
        }

        protected override void RegisterNCoverIfNecessary()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(ProcessSupport.Is64BitProcess ? NCover1ProfilerKey64Bit : NCover1ProfilerKey))
            {
                using (RegistryKey subKey = key.CreateSubKey("InprocServer32"))
                {
                    subKey.SetValue(null, Path.Combine(GetInstallDir(), "CoverLib.dll"));
                    subKey.SetValue("ThreadingModel", "Both");
                }
                key.SetValue(null, "NCover Profiler");
            }
        }

        protected override bool RequiresX86
        {
            get { return true; }
        }

        protected override bool RequiresDotNet20
        {
            get { return true; }
        }

        protected override ProcessTask CreateMergeTask(IList<string> sources, string destination)
        {
            return CreateNCoverExplorerConsoleMergeTask(@"..\NCoverExplorer", sources, destination);
        }
    }
}
