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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE80;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides access to the Visual Studio debugger.
    /// </summary>
    public class VisualStudioDebugger : IDebugger
    {
        private const int S_OK = 0;

        private static readonly Guid ManagedDebugEngineGuid = new Guid("{449EC4CC-30D2-4032-9256-EE18EB41B62B}");
        private const string ManagedDebugEngineName = "Managed";
        private static readonly Guid NativeDebugEngineGuid = new Guid("{3B476D35-A401-11D2-AAD4-00C04F990171}");
        private const string NativeDebugEngineName = "Native";

        private static readonly Guid CLSID_ComPlusOnlyDebugEngine = new Guid("449EC4CC-30D2-4032-9256-EE18EB41B62B");


        // TODO: Make the engines that are attached configurable
        // Some other engines are "Script", "T-SQL" and "Workflow"
        private static readonly Guid[] EngineGuids = new[] { ManagedDebugEngineGuid, NativeDebugEngineGuid };
        private static readonly string[] EngineNames = new[] { ManagedDebugEngineName, NativeDebugEngineName };

        private readonly DebuggerSetup debuggerSetup;
        private readonly ILogger logger;
        private readonly IVisualStudio visualStudio;

        /// <summary>
        /// Creates a Visual Studio debugger wrapper.
        /// </summary>
        /// <param name="debuggerSetup">The debugger setup options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="visualStudio">The instance of Visual Studio, or null if it should automatically find or
        /// launch instances of Visual Studio as required.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="debuggerSetup"/>
        /// or <paramref name="logger"/> is null.</exception>
        public VisualStudioDebugger(DebuggerSetup debuggerSetup, ILogger logger, IVisualStudio visualStudio)
        {
            if (debuggerSetup == null)
                throw new ArgumentNullException("debuggerSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.debuggerSetup = debuggerSetup;
            this.logger = logger;
            this.visualStudio = visualStudio;
        }

        /// <inheritdoc />
        public bool IsAttachedToProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            bool result = false;
            try
            {
                // TODO: Consider all instances of Visual Studio if not attached to a particular one.
                IVisualStudio visualStudio = GetVisualStudio(false, logger);
                if (visualStudio == null)
                    return false;

                visualStudio.Call(dte =>
                {
                    EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                    if (dteProcess != null)
                        result = true;
                });
            }
            catch (VisualStudioException ex)
            {
                logger.Log(LogSeverity.Debug, "Failed to detect whether Visual Studio debugger was attached to process.", ex);
            }

            return result;
        }

        /// <inheritdoc />
        public AttachDebuggerResult AttachToProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            AttachDebuggerResult result = AttachDebuggerResult.CouldNotAttach;
            try
            {
                IVisualStudio visualStudio = GetVisualStudio(true, logger);
                if (visualStudio != null)
                {
                    visualStudio.Call(dte =>
                    {
                        EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                        if (dteProcess != null)
                        {
                            result = AttachDebuggerResult.AlreadyAttached;
                        }
                        else
                        {
                            dteProcess = FindProcess(dte.Debugger.LocalProcesses, process);
                            if (dteProcess != null)
                            {
                                Process2 dteProcess2 = dteProcess as Process2;
                                Debugger2 dteDebugger2 = dte.Debugger as Debugger2;
                                if (dteProcess2 != null && dteDebugger2 != null)
                                {
                                    dteProcess2.Attach2(GetEngines(dteDebugger2));
                                }
                                else
                                {
                                    dteProcess.Attach();
                                }

                                result = AttachDebuggerResult.Attached;
                            }
                            else
                            {
                                logger.Log(LogSeverity.Debug, "Failed to attach Visual Studio debugger to process because it was not found in the LocalProcesses list.");
                            }
                        }
                    });
                }
            }
            catch (VisualStudioException ex)
            {
                logger.Log(LogSeverity.Debug, "Failed to attach Visual Studio debugger to process.", ex);
            }

            return result;
        }

        private static Engine[] GetEngines(Debugger2 debugger)
        {
            Transport transport = debugger.Transports.Item("Default");
            Engines engines = transport.Engines;            

            List<Engine> selectedEngines = new List<Engine>();

            // Note: The MSDN docs say we should be able to access Engines by GUID but it
            //       does not work!  I tried the example for the T-SQL engine verbatim
            //       and it threw an exception.  Not sure what the deal is so we will use
            //       the engine name for now.  -- Jeff.
            foreach (string engineName in EngineNames)
            {
                try
                {
                    Engine engine = engines.Item(engineName);
                    if (engine != null)
                        selectedEngines.Add(engine);
                }
                catch
                {
                    // Ignore error getting engine.
                }
            }

            return selectedEngines.ToArray();
        }

        /// <inheritdoc />
        public DetachDebuggerResult DetachFromProcess(Process process)
        {
            if (process == null)
                throw new ArgumentNullException("process");

            DetachDebuggerResult result = DetachDebuggerResult.AlreadyDetached;
            try
            {
                IVisualStudio visualStudio = GetVisualStudio(false, logger);
                if (visualStudio != null)
                {
                    visualStudio.Call(dte =>
                    {
                        EnvDTE.Process dteProcess = FindProcess(dte.Debugger.DebuggedProcesses, process);
                        if (dteProcess != null)
                        {
                            dteProcess.Detach(false);
                            result = DetachDebuggerResult.Detached;
                        }
                    });
                }
            }
            catch (VisualStudioException ex)
            {
                result = DetachDebuggerResult.CouldNotDetach;

                logger.Log(LogSeverity.Debug, "Failed to detach Visual Studio debugger from process.", ex);
            }

            return result;
        }

        /// <inheritdoc />
        public Process LaunchProcess(ProcessStartInfo processStartInfo)
        {
            if (processStartInfo == null)
                throw new ArgumentNullException("processStartInfo");

            Process foundProcess = null;
            try
            {
                IVisualStudio visualStudio = GetVisualStudio(true, logger);
                if (visualStudio != null)
                {
                    visualStudio.Call(dte =>
                    {
                        List<int> currentDebuggedProcesses = new List<int>();
                        if (dte.Debugger.DebuggedProcesses != null)
                        {
                            foreach (EnvDTE.Process dteProcess in dte.Debugger.DebuggedProcesses)
                                currentDebuggedProcesses.Add(dteProcess.ProcessID);
                        }

                        var serviceProvider = new VisualStudioServiceProvider(dte);
                        var debugger = serviceProvider.GetService<SVsShellDebugger, IVsDebugger2>();

                        int debugTargetInfoSize = Marshal.SizeOf(typeof(VsDebugTargetInfo2));
                        int guidSize = Marshal.SizeOf(typeof(Guid));

                        IntPtr debugTargetInfoPtr = Marshal.AllocCoTaskMem(debugTargetInfoSize + guidSize * EngineGuids.Length);
                        try
                        {
                            IntPtr engineGuidsPtr = new IntPtr(debugTargetInfoPtr.ToInt64() + debugTargetInfoSize);

                            var environment = new StringBuilder();
                            foreach (DictionaryEntry variable in processStartInfo.EnvironmentVariables)
                                environment.Append(variable.Key).Append('=').Append(variable.Value).Append('\0');

                            var debugTargetInfo = new VsDebugTargetInfo2()
                            {
                                cbSize = (uint) Marshal.SizeOf(typeof(VsDebugTargetInfo2)),
                                bstrExe = Path.GetFullPath(processStartInfo.FileName),
                                bstrArg = processStartInfo.Arguments,
                                bstrCurDir = string.IsNullOrEmpty(processStartInfo.WorkingDirectory) ? Environment.CurrentDirectory : Path.GetFullPath(processStartInfo.WorkingDirectory),
                                bstrEnv = environment.Length != 0 ? environment.ToString() : null,
                                // Visual Studio seems to hang when specifying engines explicitly.  Don't know
                                // why yet.  -- Jeff.
                                //dwDebugEngineCount = (uint) EngineGuids.Length,
                                //pDebugEngines = engineGuidsPtr,
                                fSendToOutputWindow = 1,
                                guidLaunchDebugEngine = CLSID_ComPlusOnlyDebugEngine,
                                dlo = (int) DEBUG_LAUNCH_OPERATION.DLO_CreateProcess,
                                LaunchFlags = (int)(__VSDBGLAUNCHFLAGS.DBGLAUNCH_WaitForAttachComplete | __VSDBGLAUNCHFLAGS.DBGLAUNCH_Silent)
                            };

                            Marshal.StructureToPtr(debugTargetInfo, debugTargetInfoPtr, false);

                            for (int i = 0; i < EngineGuids.Length; i++)
                                Marshal.StructureToPtr(EngineGuids[i], new IntPtr(engineGuidsPtr.ToInt64() + i * guidSize), false);

                            int hresult = debugger.LaunchDebugTargets2(1, debugTargetInfoPtr);
                            if (hresult != S_OK)
                                Marshal.ThrowExceptionForHR(hresult);
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(debugTargetInfoPtr);
                        }

                        if (dte.Debugger.DebuggedProcesses != null)
                        {
                            foreach (EnvDTE.Process dteProcess in dte.Debugger.DebuggedProcesses)
                            {
                                if (!currentDebuggedProcesses.Contains(dteProcess.ProcessID))
                                {
                                    foundProcess = Process.GetProcessById(dteProcess.ProcessID);
                                    break;
                                }
                            }
                        }

                        if (foundProcess == null)
                        {
                            logger.Log(LogSeverity.Debug, "Could not find the newly launched process.");
                        }
                    });
                }
            }
            catch (VisualStudioException ex)
            {
                logger.Log(LogSeverity.Debug, "Failed to attach Visual Studio debugger to process.", ex);
            }

            return foundProcess;
        }

        private IVisualStudio GetVisualStudio(bool launchIfNoActiveInstance, ILogger logger)
        {
            if (visualStudio != null)
                return visualStudio;
            return VisualStudioManager.Instance.GetVisualStudio(VisualStudioVersion.Any, launchIfNoActiveInstance, logger);
        }

        private static EnvDTE.Process FindProcess(EnvDTE.Processes dteProcesses, Process process)
        {
            if (dteProcesses != null)
            {
                foreach (EnvDTE.Process dteProcess in dteProcesses)
                {
                    if (dteProcess.ProcessID == process.Id)
                        return dteProcess;
                }
            }

            return null;
        }
    }
}
