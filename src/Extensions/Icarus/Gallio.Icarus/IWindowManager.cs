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

using System.Windows.Forms;
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;
using Gallio.Common;

namespace Gallio.Icarus
{
    /// <summary>
    /// Shameless rip-off of Reflector add-in API.
    /// </summary>
    public interface IWindowManager
    {
        ToolStripItemCollection StatusStrip { get; }
        ToolStripContainer ToolStrip { get; }
        ToolStripItemCollection Menu { get; }

        void Add(string identifier, Control content, string caption);
        void Add(string identifier, Control content, string caption, Icon icon);
        void Show(string identifier);
        void Show(string identifier, DockState dockState);
        void Register(string identifer, Action action);
        void Remove(string identifier);
    }
}
