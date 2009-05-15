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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Gallio.Common;

namespace Gallio.Icarus
{
    public class WindowManager : IWindowManager
    {
        private readonly DockPanel dockPanel;
        private readonly Dictionary<string, Window> windows = new Dictionary<string, Window>();
        private readonly Dictionary<string, Action> hooks = new Dictionary<string, Action>();

        public ToolStripItemCollection StatusStrip { get; private set; }
        public ToolStripContainer ToolStrip { get; private set; }
        public ToolStripItemCollection Menu { get; private set; }

        public WindowManager(DockPanel dockPanel, ToolStripItemCollection statusStrip,
            ToolStripContainer toolStrip, ToolStripItemCollection menu)
        {
            this.dockPanel = dockPanel;
            StatusStrip = statusStrip;
            ToolStrip = toolStrip;
            Menu = menu;
        }

        public void Add(string identifier, Control content, string caption)
        {
            Add(identifier, content, caption, null);
        }

        public void Add(string identifier, Control content, string caption, Icon icon)
        {
            if (windows.ContainsKey(identifier))
                throw new Exception("Identifier is not unique");

            var window = new Window(identifier, content, caption);
            if (icon != null)
                window.Icon = icon;

            window.FormClosed += (sender, e) => Remove(window.Id);

            windows.Add(identifier, window);
        }

        public void Remove(string identifier)
        {
            if (windows.ContainsKey(identifier))
                windows.Remove(identifier);
        }

        internal Window Get(string identifier)
        {
            // if we have the window stored, then return it
            if (windows.ContainsKey(identifier))
                return windows[identifier] as Window;

            // if we have an action registered, run it
            if (hooks.ContainsKey(identifier))
                hooks[identifier]();

            // check if we have the window now
            if (windows.ContainsKey(identifier))
                return windows[identifier] as Window;

            // otherwise give up!
            return null;
        }

        public void Show(string identifier)
        {
            var window = Get(identifier);
            if (window == null)
                throw new Exception("No window with that identifier exists");
               
            window.Show(dockPanel);
        }

        public void Show(string identifier, DockState dockState)
        {
            var window = Get(identifier);
            if (window == null)
                throw new Exception("No window with that identifier exists");

            window.Show(dockPanel, dockState);
        }

        public void Register(string identifier, Action action)
        {
            if (hooks.ContainsKey(identifier))
                throw new Exception("Identifier is not unique");

            hooks.Add(identifier, action);
        }
    }
}
