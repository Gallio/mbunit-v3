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
using System.Drawing;
using EnvDTE;
using Gallio.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.CommandBars;
using stdole;

namespace Gallio.VisualStudio.Shell.Actions
{
    /// <summary>
    /// Wraps a <see cref="CommandBarButton"/> for use with <see cref="Action" />.
    /// </summary>
    public class ActionButton : ShellComponent
    {
        private readonly Action action;
        private readonly Command vsCommand;
        private readonly CommandBarButton vsCommandBarButton;
        private Image picture;
        private ActionButtonStatus status;

        /// <summary>
        /// Creates the command button wrapper.
        /// </summary>
        public ActionButton(IShell shell, Action action, Command vsCommand, CommandBarButton vsCommandBarButton)
            : base(shell)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (vsCommand == null)
                throw new ArgumentNullException("vsCommand");
            if (vsCommandBarButton == null)
                throw new ArgumentNullException("vsCommandBarButton");

            this.action = action;
            this.vsCommand = vsCommand;
            this.vsCommandBarButton = vsCommandBarButton;
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        public Action Action
        {
            get { return action; }
        }

        /// <summary>
        /// Gets the Visual Studio command object.
        /// </summary>
        public Command VSCommand
        {
            get { return vsCommand; }
        }

        /// <summary>
        /// Gets the Visual Studio command bar button object.
        /// </summary>
        public CommandBarButton VSCommandBarButton
        {
            get { return vsCommandBarButton; }
        }

        /// <summary>
        /// Gets or sets the caption for the action.
        /// </summary>
        public string Caption
        {
            get { return vsCommandBarButton.Caption; }
            set { vsCommandBarButton.Caption = value; }
        }

        /// <summary>
        /// Gets or sets the tooltip for the action.
        /// </summary>
        public string Tooltip
        {
            get { return vsCommandBarButton.TooltipText; }
            set { vsCommandBarButton.TooltipText = value; }
        }

        /// <summary>
        /// Gets or sets the picture on the button.
        /// </summary>
        public Image Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                vsCommandBarButton.Picture = (StdPicture) ImageConversionUtils.GetIPictureDispFromImage(value);
            }
        }

        /// <summary>
        /// Gets or sets the status of the action.
        /// </summary>
        public ActionButtonStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        /// <summary>
        /// Executes the associated action.
        /// </summary>
        public void Execute()
        {
            action.Execute(this);
        }

        /// <summary>
        /// Updates the action button's state.
        /// </summary>
        public void Update()
        {
            action.Update(this);
        }
    }
}
