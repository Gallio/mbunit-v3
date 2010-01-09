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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gallio.VisualStudio.Shell.UI.ToolWindows
{
    /// <summary>
    /// Container window for tool window contents.
    /// </summary>
    [ComVisible(true)]
    public sealed class ToolWindowContainer : UserControl
    {
        private readonly IToolWindowPane toolWindowPane;

        private ToolWindow toolWindow;

        /// <summary>
        /// Creates a container for a tool window.
        /// </summary>
        /// <param name="toolWindowPane">The tool window pane.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="toolWindowPane"/> is null.</exception>
        public ToolWindowContainer(IToolWindowPane toolWindowPane)
        {
            this.toolWindowPane = toolWindowPane;

            InitializeComponent();
        }

        /// <summary>
        /// Gets the tool window pane.
        /// </summary>
        public IToolWindowPane ToolWindowPane
        {
            get { return toolWindowPane; }
        }

        /// <summary>
        /// Gets or sets the tool window within the container, or null if none.
        /// </summary>
        public ToolWindow ToolWindow
        {
            get { return toolWindow; }
            set
            {
                if (toolWindow != value)
                {
                    toolWindow = value;

                    SuspendLayout();

                    Controls.Clear();
                    toolWindow.Dock = DockStyle.Fill;
                    Controls.Add(value);

                    ResumeLayout();
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ToolWindowContainer";
            this.Size = new System.Drawing.Size(0, 0);
            this.ResumeLayout(false);
        }
    }
}
