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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Gallio.VisualStudio.Shell.UI
{
    /// <summary>
    /// Container window for tool window contents.
    /// </summary>
    [ComVisible(true)]
    public class ShellToolWindowContainer : UserControl
    {
        private ShellToolWindow toolWindow;

        /// <summary>
        /// Creates a container for a tool window.
        /// </summary>
        public ShellToolWindowContainer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the tool window pane.
        /// </summary>
        public ShellToolWindowPane ToolWindowPane { get; internal set; }

        /// <summary>
        /// Gets the tool window within the container.
        /// </summary>
        public ShellToolWindow ToolWindow
        {
            get { return toolWindow; }
            internal set
            {
                Controls.Clear();
                toolWindow = value;

                if (toolWindow != null)
                {
                    toolWindow.Dock = DockStyle.Fill;
                    Controls.Add(toolWindow);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ShellToolWindowContainer
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ShellToolWindowContainer";
            this.Size = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);

        }
    }
}
