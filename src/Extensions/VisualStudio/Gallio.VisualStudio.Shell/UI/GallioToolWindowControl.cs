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
    /// Abstract base control class for tool windows.
    /// </summary>
    [ComVisible(true)]
    public class GallioToolWindowControl : UserControl
    {
        private GallioToolWindow toolWindow;

        /// <summary>
        /// Gets the tool window.
        /// </summary>
        public GallioToolWindow ToolWindow
        {
            get
            {
                return toolWindow;
            }

            internal set
            {
                toolWindow = value;
            }
        }

        /// <summary>
        /// Sets the content of the control.
        /// </summary>
        public void SetContent(GallioToolWindowContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            content.Dock = DockStyle.Fill;
            content.Visible = true;
            Controls.Clear();
            Controls.Add(content);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Name = "GallioToolWindowControl";
            this.ResumeLayout(false);
        }
    }
}
