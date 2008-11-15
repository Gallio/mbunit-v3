// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace Gallio.AutoCAD.Plugin
{
    /// <summary>
    /// Contains commands for performing Gallio operations inside the AutoCAD process.
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Creates an <c>AcadTestDriver</c> instance and registers it as a service.
        /// </summary>
        /// <remarks>
        /// This command blocks on the calling thread until <c>Shutdown</c> is called.
        /// </remarks>
        [CommandMethod("CREATEENDPOINTANDWAIT")]
        public static void CreateEndPointAndWait()
        {
            string runtimePath;
            if (!GetRuntimePath(out runtimePath))
                return;

            string extensionPath;
            if (!GetExtensionPath(out extensionPath))
                return;

            Process ownerProcess;
            if (!GetOwnerProcess(out ownerProcess))
                return;

            string ipcPortName;
            if (!GetIpcPortName(out ipcPortName))
                return;

            TimeSpan timeout;
            if (!GetTimeout(out timeout))
                return;

            ReflectGallioLoader.Initialize(runtimePath).SetupRuntime();

            using (ReflectAcadEndpoint endpoint = new ReflectAcadEndpoint(extensionPath, ipcPortName, ownerProcess))
            {
                endpoint.Run(timeout);
            }
        }

        private static bool GetRuntimePath(out string runtimePath)
        {
            runtimePath = String.Empty;

            PromptResult prompt = ActiveEditor.GetString("Gallio runtime path:");
            if (prompt.Status != PromptStatus.OK)
                return false;

            if (!Directory.Exists(prompt.StringResult))
            {
                ActiveEditor.WriteMessage("Directory \"{0}\" not found.\n", prompt.StringResult);
                return false;
            }

            runtimePath = prompt.StringResult;
            return true;
        }

        private static bool GetExtensionPath(out string extensionPath)
        {
            extensionPath = String.Empty;

            PromptResult prompt = ActiveEditor.GetString("Path to Gallio.AutoCAD.dll:");
            if (prompt.Status != PromptStatus.OK)
                return false;

            if (!File.Exists(prompt.StringResult))
            {
                ActiveEditor.WriteMessage("File \"{0}\" not found.\n", prompt.StringResult);
                return false;
            }

            extensionPath = prompt.StringResult;
            return true;
        }

        private static bool GetOwnerProcess(out Process ownerProcess)
        {
            ownerProcess = null;

            PromptIntegerResult prompt = ActiveEditor.GetInteger(new PromptIntegerOptions("Owner process ID:") { LowerLimit = 1, UpperLimit = Int32.MaxValue });
            if (prompt.Status != PromptStatus.OK)
                return false;
            try
            {
                ownerProcess = Process.GetProcessById(prompt.Value);
            }
            catch (ArgumentException)
            {
                ActiveEditor.WriteMessage("Unable to find process with ID \"{0}\".\n", prompt.Value);
                return false;
            }

            return true;
        }

        private static bool GetIpcPortName(out string ipcPortName)
        {
            ipcPortName = String.Empty;

            PromptResult prompt = ActiveEditor.GetString("IPC port name:");
            if (prompt.Status != PromptStatus.OK)
                return false;

            ipcPortName = prompt.StringResult;
            return true;
        }

        private static bool GetTimeout(out TimeSpan timeout)
        {
            timeout = TimeSpan.Zero;

            PromptDoubleResult prompt = ActiveEditor.GetDouble(new PromptDoubleOptions("Timeout length (seconds):") { AllowNegative = false });
            if (prompt.Status != PromptStatus.OK)
                return false;

            timeout = TimeSpan.FromSeconds(prompt.Value);
            return true;
        }

        private static Editor ActiveEditor
        {
            get { return Application.DocumentManager.MdiActiveDocument.Editor; }
        }
    }
}
