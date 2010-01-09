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
using System.Reflection;
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
        /// <para>
        /// This command blocks on the calling thread until <c>Shutdown</c> is called.
        /// </para>
        /// </remarks>
        [CommandMethod("CREATEENDPOINTANDWAIT")]
        public static void CreateEndPointAndWait()
        {
            string ipcPortName;
            if (!GetIpcPortName(out ipcPortName))
                return;

            Guid linkId;
            if (!GetLinkId(out linkId))
                return;

            string gallioLoaderAssemblyPath;
            if (!GetGallioLoaderAssemblyPath(out gallioLoaderAssemblyPath))
                return;

            if (!string.IsNullOrEmpty(gallioLoaderAssemblyPath))
                Assembly.LoadFrom(gallioLoaderAssemblyPath);

            ShimWithLoader.Run(ipcPortName, linkId);
        }

        private static bool GetIpcPortName(out string ipcPortName)
        {
            return Prompt("IPC port name:", out ipcPortName);
        }

        private static bool GetLinkId(out Guid linkId)
        {
            string result;
            if (! Prompt("Link Id:", out result))
            {
                linkId = Guid.Empty;
                return false;
            }

            linkId = new Guid(result);
            return true;
        }

        private static bool GetGallioLoaderAssemblyPath(out string gallioLoaderAssemblyPath)
        {
            return Prompt("Gallio.Loader Assembly Path:", out gallioLoaderAssemblyPath);
        }

        private static bool Prompt(string promptMessage, out string result)
        {
            PromptResult prompt = ActiveEditor.GetString(promptMessage);
            if (prompt.Status != PromptStatus.OK)
            {
                result = String.Empty;
                return false;
            }

            result = prompt.StringResult;
            return true;
        }

        private static Editor ActiveEditor
        {
            get { return Application.DocumentManager.MdiActiveDocument.Editor; }
        }

        /// <summary>
        /// Within this class we can access Gallio.Loader types because the loader has been loaded.
        /// </summary>
        private static class ShimWithLoader
        {
            public static void Run(string ipcPortName, Guid linkId)
            {
                IGallioLoader loader = GallioLoader.Initialize();
                loader.SetupRuntime(); // note: after this point we can reference Gallio types.

                ShimWithRuntime.Run(ipcPortName, linkId);
            }
        }

        /// <summary>
        /// Within this class we can access Gallio types because the runtime has been initialized.
        /// </summary>
        private static class ShimWithRuntime
        {
            public static void Run(string ipcPortName, Guid linkId)
            {
                using (var client = new TestIsolationClient(ipcPortName, linkId))
                {
                    client.Run();
                }
            }
        }
    }
}
