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
using Gallio.Icarus.WindowManager;
using Gallio.UI.Common.Synchronization;

namespace Gallio.Icarus.Packages
{
    public class CodeViewerPackage : IPackage
    {
        private readonly ISourceCodeController sourceCodeController;
        private readonly IWindowManager windowManager;
        private readonly List<string> openWindows = new List<string>();

        public CodeViewerPackage(ISourceCodeController sourceCodeController, 
            IWindowManager windowManager)
        {
            this.sourceCodeController = sourceCodeController;
            this.windowManager = windowManager;
        }

        public void Load()
        {
            sourceCodeController.ShowSourceCode += (s, e) => 
                ShowSourceCode(e.CodeLocation);
        }

        private void ShowSourceCode(CodeLocation codeLocation)
        {
            var identifier = codeLocation.Path ?? "(unknown)";

            if (WindowIsOpen(codeLocation))
            {
                AddWindow(codeLocation, identifier);
            }
            else
            {
                ActivateWindow(codeLocation, identifier);
            }
        }

        private bool WindowIsOpen(CodeLocation codeLocation)
        {
            return openWindows.Contains(codeLocation.Path) == false;
        }

        private void ActivateWindow(CodeLocation codeLocation, string identifier)
        {
			SynchronizationContext.Post(cb => 
			{
                var window = windowManager.Get(identifier);
                var codeViewer = (CodeViewer)window.Content;

                if (codeLocation != CodeLocation.Unknown)
                    codeViewer.JumpTo(codeLocation.Line, codeLocation.Column);

                windowManager.Show(identifier);
			}, null);
        }

        private void AddWindow(CodeLocation codeLocation, string identifier)
        {
            string caption;
            if (codeLocation == CodeLocation.Unknown)
            {
                caption = "(unknown)";
            }
            else
            {
                caption = Path.GetFileName(codeLocation.Path) 
                    ?? "(unknown)";
            }

            SynchronizationContext.Post(cb => CreateWindow(codeLocation, 
                identifier, caption), null);
        }

        private void CreateWindow(CodeLocation codeLocation, string identifier, 
            string caption)
        {
            var codeViewer = new CodeViewer(codeLocation);
            
            var window = windowManager.Add(identifier, codeViewer, caption);
            window.FormClosed += (s1, e1) => openWindows.Remove(identifier);
            
            windowManager.Show(identifier);
            
            openWindows.Add(identifier);
        }

        public void Dispose() { }
    }
}
