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
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Gallio.Loader;
using Gallio.Model.Isolation;

namespace Gallio.AutoCAD.Plugin
{
    /// <summary>
    /// Contains commands for performing Gallio operations inside the AutoCAD process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class assumes that the Gallio.Loader.dll assembly is registered in the GAC.
    /// </para>
    /// </remarks>
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
            string ipcPortName;
            if (!GetIpcPortName(out ipcPortName))
                return;

            IGallioLoader loader = GallioLoader.Initialize();
            loader.SetupRuntime(); // note: after this point we can reference Gallio types.

            Shim.Run(ipcPortName);
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

        private static Editor ActiveEditor
        {
            get { return Application.DocumentManager.MdiActiveDocument.Editor; }
        }

        /// <summary>
        /// Within this class we can access Gallio types because the loader has been initialized.
        /// </summary>
        private static class Shim
        {
            public static void Run(string ipcPortName)
            {
                using (TestIsolationClient client = new TestIsolationClient(ipcPortName))
                {
                    client.Run();
                }
            }
        }
    }
}
