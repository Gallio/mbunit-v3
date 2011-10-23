// Copyright 2011 Gallio Project - http://www.gallio.org/
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

using Gallio.Icarus.Commands;
using Gallio.Icarus.WindowManager;
using Gallio.UI.Menus;

namespace Gallio.Icarus.RuntimeLog
{
    public class RuntimeLogPackage : IPackage
    {
        private readonly IWindowManager windowManager;
        private readonly IRuntimeLogController runtimeLogController;

        public const string WindowId = "Gallio.Icarus.RuntimeLog";

        public RuntimeLogPackage(IWindowManager windowManager, IRuntimeLogController runtimeLogController)
        {
            this.windowManager = windowManager;
            this.runtimeLogController = runtimeLogController;
        }

        public void Load()
        {
            RegisterWindow();
            AddMenuItem();
        }

        private void RegisterWindow()
        {
            windowManager.Register(WindowId, () => windowManager.Add(WindowId, new RuntimeLogWindow(runtimeLogController), 
                RuntimeLogResources.Runtime_Log, RuntimeLogResources.Icon), Location.Bottom);
        }

        private void AddMenuItem()
        {
            windowManager.MenuManager.Add("View", () => new MenuCommand
            {
                Command = new DelegateCommand(pm => windowManager.Show(WindowId)),
                Text = RuntimeLogResources.Runtime_Log,
                Image = RuntimeLogResources.Icon.ToBitmap()
            });
        }

        public void Dispose() { }         
    }
}