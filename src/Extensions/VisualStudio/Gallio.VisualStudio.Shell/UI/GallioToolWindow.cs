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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Gallio.VisualStudio.Shell.UI
{
    /// <summary>
    /// General purpose container for Gallio tool window.
    /// </summary>
    [Guid("9C9191A4-CDFB-4931-9B96-6CC7CD0BC203")]
    public class GallioToolWindow : ToolWindowPane, IShellComponent
    {
    	/// <summary>
		/// Default constructor. Calls base class with Package Instance
		/// </summary>
        public GallioToolWindow()
            : base(ShellAccessor.Shell.Package as IServiceProvider)
		{
        }

        private readonly IShell shell = ShellAccessor.Shell;

        /// <summary>
        /// Gets the shell associated with the component.
        /// </summary>
        public IShell Shell
        {
            get
            {
                return shell;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public void SetContent(GallioToolWindowContent content)
        {
            Control.SetContent(content);
        }

        private GallioToolWindowControl control;

        /// <summary>
        /// Gets the window that is associated with this window pane.
        /// </summary>
        public GallioToolWindowControl Control
        {
            get
            {
                if (control == null)
                {
                    control = new GallioToolWindowControl();
                    control.Visible = true;
                    control.ToolWindow = this;
                }

                return control;
            }
        }

        /// <summary>
        /// Gets the window that is associated with this window pane.
        /// </summary>
        public override IWin32Window Window
        {
            get
            {
                return Control;
            }
        }

        /// <summary>
        /// Gets/sets the window caption.
        /// Due to a bug in the VS2008 SDK, <see cref="ToolWindowPane.Caption"/> does not work properly.
        /// </summary>
        public new string Caption
        {
            get
            {
                object caption;
                Frame.GetProperty((int)__VSFPROPID.VSFPROPID_Caption, out caption);
                return (string)caption;
            }
            
            set
            {
                Frame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, value);
            }
        }

        /// <summary>
        /// Gets/sets the frame hosting the window.
        /// </summary>
        public new IVsWindowFrame Frame
        {
            get
            {
                return (IVsWindowFrame)base.Frame;
            }

            set
            {
                base.Frame = value;
            }
        }

        /// <summary>
        /// Shows the window frame.
        /// </summary>
        public void Show()
        {
            Frame.Show();
        }

        /// <summary>
        /// Hides the window frame.
        /// </summary>
        public void Hide()
        {
            Frame.Hide();
        }

        /// <summary>
        /// Closes the window frame.
        /// </summary>
        public void Close()
        {
            Frame.CloseFrame(0);
        }
    }
}
