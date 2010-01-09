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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using Gallio.Common;
using Gallio.Common.Concurrency;
using Gallio.Common.Platform;
using Gallio.Runtime.Logging;
using Microsoft.Win32;
using Thread = System.Threading.Thread;
using Gallio.VisualStudio.Interop.Native;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides support for finding and launching instances of VisualStudio.
    /// </summary>
    public class VisualStudioManager : IVisualStudioManager
    {
        private static readonly VisualStudioManager instance = new VisualStudioManager();

        private const int VisualStudioAttachTimeoutMilliseconds = 60000;

        private static readonly Dictionary<VisualStudioVersion, VisualStudioVersionInfo> versionInfos =
            new Dictionary<VisualStudioVersion, VisualStudioVersionInfo>()
            {
                { VisualStudioVersion.VS2005, new VisualStudioVersionInfo() { Name = "8.0", ProgID = "VisualStudio.DTE.8.0" } },
                { VisualStudioVersion.VS2008, new VisualStudioVersionInfo() { Name = "9.0", ProgID = "VisualStudio.DTE.9.0" } },
                { VisualStudioVersion.VS2010, new VisualStudioVersionInfo() { Name = "10.0", ProgID = "VisualStudio.DTE.10.0" } }
            };

        /// <summary>
        /// Gets the singleton instance of the manager.
        /// </summary>
        public static IVisualStudioManager Instance
        {
            get { return instance; }
        }

        /// <inheritdoc />
        public IVisualStudio GetVisualStudio(VisualStudioVersion version, bool launchIfNoActiveInstance, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            IVisualStudio visualStudio = GetActiveVisualStudio(version, logger);
            if (visualStudio == null && launchIfNoActiveInstance)
                visualStudio = LaunchVisualStudio(version, logger);

            return visualStudio;
        }

        /// <inheritdoc />
        public IVisualStudio LaunchVisualStudio(VisualStudioVersion version, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            Pair<string, VisualStudioVersion>? installDirAndVersion = GetVisualStudioInstallDirAndVersion(version);
            if (!installDirAndVersion.HasValue)
            {
                logger.Log(LogSeverity.Debug, string.Format("Could not find Visual Studio version '{0}'.", version));
                return null;
            }

            string devenvPath = Path.Combine(installDirAndVersion.Value.First, "devenv.exe");
            ProcessTask devenvProcessTask = new ProcessTask(devenvPath, "", Environment.CurrentDirectory);

            logger.Log(LogSeverity.Debug, string.Format("Launching Visual Studio using path: '{0}'.", devenvProcessTask.ExecutablePath));
            devenvProcessTask.Start();

            System.Diagnostics.Process devenvProcess = devenvProcessTask.Process;
            if (devenvProcess != null)
            {
                int processId = devenvProcess.Id;

                Stopwatch stopwatch = Stopwatch.StartNew();
                for (;;)
                {
                    IVisualStudio visualStudio = GetVisualStudioFromProcess(processId, installDirAndVersion.Value.Second, true, logger);
                    if (visualStudio != null)
                        return visualStudio;

                    if (stopwatch.ElapsedMilliseconds > VisualStudioAttachTimeoutMilliseconds)
                    {
                        logger.Log(LogSeverity.Debug, string.Format("Stopped waiting for Visual Studio to launch after {0} milliseconds.", VisualStudioAttachTimeoutMilliseconds));
                        break;
                    }

                    if (!devenvProcessTask.IsRunning)
                        break;

                    Thread.Sleep(500);
                }
            }

            if (devenvProcessTask.IsTerminated && devenvProcessTask.Result != null)
            {
                if (! devenvProcessTask.Result.HasValue)
                    logger.Log(LogSeverity.Debug, "Failed to launch Visual Studio.", devenvProcessTask.Result.Exception);
            }

            return null;
        }

        private static IVisualStudio GetActiveVisualStudio(VisualStudioVersion version, ILogger logger)
        {
            IVisualStudio visualStudio = null;
            ForEachApplicableVersion(version, aVersion =>
            {
                object obj = GetActiveObject(versionInfos[aVersion].ProgID);
                if (obj != null)
                {
                    visualStudio = new VisualStudio((DTE) obj, aVersion, false, logger);
                    return true;
                }

                return false;
            });

            return visualStudio;
        }

        private static IVisualStudio GetVisualStudioFromProcess(int processId, VisualStudioVersion version, bool wasLaunched, ILogger logger)
        {
            // This code inspired from a forum post by Leonard Jiang.
            // http://social.msdn.microsoft.com/Forums/en-US/vsx/thread/3120db69-a89c-4545-874f-2d61c9317c8a/
            string monikerDisplayName = string.Concat("!", versionInfos[version].ProgID, ":", processId.ToString());

            object obj = GetActiveObjectWithMonikerDisplayName(monikerDisplayName);
            if (obj != null)
                return new VisualStudio((DTE)obj, version, wasLaunched, logger);

            return null;
        }

        private static Pair<string, VisualStudioVersion>? GetVisualStudioInstallDirAndVersion(VisualStudioVersion version)
        {
            Pair<string, VisualStudioVersion>? result = null;
            ForEachApplicableVersion(version, aVersion =>
            {
                string path = GetVisualStudioInstallDir(versionInfos[aVersion].Name);
                if (path != null)
                {
                    result = new Pair<string, VisualStudioVersion>(path, aVersion);
                    return true;
                }

                return false;
            });

            return result;
        }

        private static string GetVisualStudioInstallDir(string version)
        {
            string result = null;

            RegistryUtils.TryActionOnOpenSubKeyWithBitness(Registry.LocalMachine,
                @"SOFTWARE\Microsoft\VisualStudio\" + version,
                @"SOFTWARE\Wow6432Node\Microsoft\VisualStudio\" + version,
                key =>
                    {
                        result = key.GetValue("InstallDir") as string;
                        return result != null;
                    });

            return result;
        }

        private static object GetActiveObject(string progId)
        {
            try
            {
                return Marshal.GetActiveObject(progId);
            }
            catch (COMException)
            {
                return null;
            }
        }

        private static object GetActiveObjectWithMonikerDisplayName(string monikerDisplayName)
        {
            IRunningObjectTable rot = null;
            IEnumMoniker enumMoniker = null;
            try
            {
                int hresult = NativeMethods.GetRunningObjectTable(0, out rot);
                if (hresult != NativeConstants.S_OK || rot == null)
                    throw new VisualStudioException("Could not access COM Running Object Table.");

                rot.EnumRunning(out enumMoniker);
                enumMoniker.Reset();

                IMoniker[] monikers = new IMoniker[1];
                while (enumMoniker.Next(1, monikers, IntPtr.Zero) == NativeConstants.S_OK)
                {
                    IBindCtx context = null;
                    string displayName;
                    try
                    {
                        if (NativeMethods.CreateBindCtx(0, out context) != NativeConstants.S_OK)
                            continue;

                        monikers[0].GetDisplayName(context, null, out displayName);
                    }
                    finally
                    {
                        if (context != null)
                            Marshal.ReleaseComObject(context);
                    }

                    if (monikerDisplayName == displayName)
                    {
                        object obj;
                        if (rot.GetObject(monikers[0], out obj) == NativeConstants.S_OK)
                            return obj;
                    }
                }
            }
            finally
            {
                if (rot != null)
                    Marshal.ReleaseComObject(rot);
                if (rot != null)
                    Marshal.ReleaseComObject(enumMoniker);
            }

            return null;
        }

        private static bool ForEachApplicableVersion(VisualStudioVersion version, Func<VisualStudioVersion, bool> action)
        {
            if (version == VisualStudioVersion.Any)
            {
                return action(VisualStudioVersion.VS2010)
                    || action(VisualStudioVersion.VS2008)
                    || action(VisualStudioVersion.VS2005);
            }

            return action(version);
        }

        private sealed class VisualStudioVersionInfo
        {
            public string ProgID { get; set; }
            public string Name { get; set; }
        }
    }
}
