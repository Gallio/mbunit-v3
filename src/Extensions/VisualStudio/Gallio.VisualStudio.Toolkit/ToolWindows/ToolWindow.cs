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
using System.Drawing;
using EnvDTE80;
using Gallio.VisualStudio.Toolkit.Interop;

namespace Gallio.VisualStudio.Toolkit.ToolWindows
{
    /// <summary>
    /// Abstract base class for a tool window controller.
    /// </summary>
    public abstract class ToolWindow : Component
    {
        private readonly Type controlType;
        private readonly Guid guid;

        private ToolWindowControl control;
        private Window2 vsWindow;

        private string caption;
        private Image tabPicture;

        /// <summary>
        /// Creates a tool window.
        /// </summary>
        /// <param name="shell">The shell that owns the tool window</param>
        /// <param name="controlType">The user control type that presents the contents of the tool
        /// window.  The type must inherit from <see cref="ToolWindowControl"/> and be ComVisible.</param>
        /// <param name="guid">The guid to uniquely identify the window</param>
        /// <param name="caption"></param>
        public ToolWindow(Shell shell, Type controlType, Guid guid, string caption)
            : base(shell)
        {
            if (controlType == null)
                throw new ArgumentNullException("controlType");
            if (! controlType.IsSubclassOf(typeof(ToolWindowControl)))
                throw new ArgumentException("Control type must be a subclass of ToolWindowControl", "controlType");
            if (caption == null)
                throw new ArgumentNullException("caption");

            this.controlType = controlType;
            this.guid = guid;
            this.caption = caption;
        }

        /// <summary>
        /// Gets the Visual Studio window object.
        /// </summary>
        public Window2 VSWindow
        {
            get { return vsWindow; }
        }

        /// <summary>
        /// Gets the control that presents the contents of the tool window, or null if not initialized.
        /// </summary>
        public ToolWindowControl Control
        {
            get { return control; }
        }

        /// <summary>
        /// Gets or sets the caption of the window.
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set
            {
                if (vsWindow != null)
                    vsWindow.Caption = value;
                caption = value;
            }
        }

        /// <summary>
        /// Gets or sets the image that appears on the tab of the tool window.
        /// </summary>
        public Image TabPicture
        {
            get { return tabPicture; }
            set
            {
                if (vsWindow != null)
                    vsWindow.SetTabPicture(ImageConversionUtils.GetIPictureDispFromImage(value));
                tabPicture = value;
            }
        }

        /// <summary>
        /// Shows the tool window.
        /// </summary>
        public void Show()
        {
            CreateAndInitializeWindow();

            vsWindow.Visible = true;
        }

        /// <summary>
        /// Hides the tool window.
        /// </summary>
        public void Hide()
        {
            if (vsWindow != null)
                vsWindow.Visible = false;
        }

        /// <summary>
        /// Provides an opportunity for subclasses to initialize a window that has just been created
        /// before it is made visible.
        /// </summary>
        protected virtual void InitializeWindow()
        {
        }

        private void CreateAndInitializeWindow()
        {
            if (vsWindow == null)
            {
                string indexGuid = "{" + guid + "}";

                object obj = null;
                vsWindow = (Window2)((Windows2)Shell.DTE.Windows).CreateToolWindow2(Shell.AddIn,
                    controlType.Assembly.Location,
                    controlType.FullName,
                    caption, indexGuid, ref obj);

                if (tabPicture != null)
                    TabPicture = tabPicture;

                control = (ToolWindowControl) obj;
                control.ToolWindow = this;

                InitializeWindow();
            }
        }
    }
}
