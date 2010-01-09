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

using System.Drawing;
using System.Windows.Forms;
using Gallio.Common;
using WeifenLuo.WinFormsUI.Docking;

namespace Gallio.Icarus
{
    /// <summary>
    /// Window manager for application shell.
    /// </summary>
    public interface IWindowManager
    {
        ToolStripItemCollection StatusStrip { get; }
        ToolStripContainer ToolStrip { get; }
        ToolStripItemCollection Menu { get; }

        Window Add(string identifier, Control content, string caption);
        Window Add(string identifier, Control content, string caption, Icon icon);
        void Show(string identifier);
        void Show(string identifier, DockState dockState);
        void Register(string identifer, Action action);
        void Remove(string identifier);
    }
}
