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

using System.Collections.Generic;
using System.IO;
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Views.CodeViewer;
using Gallio.Runtime.Extensibility;
using Gallio.Common.Concurrency;

namespace Gallio.Icarus.Packages
{
    internal class CodeViewerPackage : IPackage
    {
        private ISourceCodeController sourceCodeController;
        private WindowManager windowManager;
        private List<string> openWindows = new List<string>();

        public void Load(IServiceLocator serviceLocator)
        {
            sourceCodeController = serviceLocator.Resolve<ISourceCodeController>();
            windowManager = (WindowManager)serviceLocator.Resolve<IWindowManager>();

            sourceCodeController.ShowSourceCode += (sender, e) =>
            {
                string identifier = e.CodeLocation.Path ?? "(unknown)";

                if (!openWindows.Contains(e.CodeLocation.Path))
                {
                    string caption;
                    if (e.CodeLocation == CodeLocation.Unknown)
                        caption = "(unknown)";
                    else
                        caption = Path.GetFileName(e.CodeLocation.Path) ?? "(unknown)";

                    Sync.Invoke(windowManager.DockPanel, () =>
                    {
                        var window = windowManager.Add(identifier, new CodeViewer(e.CodeLocation), caption);
                        window.FormClosed += (sender2, e2) => { openWindows.Remove(identifier); };
                        windowManager.Show(identifier);
                        openWindows.Add(identifier);
                    });
                }
                else
                {
                    Sync.Invoke(windowManager.DockPanel, () =>
                    {
                        var window = windowManager.Get(identifier);
                        var codeViewer = (CodeViewer)window.Content;
                        if (e.CodeLocation != CodeLocation.Unknown)
                            codeViewer.JumpTo(e.CodeLocation.Line, e.CodeLocation.Column);
                        windowManager.Show(identifier);
                    });
                }
            };
        }

        public void Unload()
        { }
    }
}
