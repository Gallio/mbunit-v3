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
using System.Text;

namespace Gallio.VisualStudio.Shell.Actions
{
    /// <summary>
    /// Indicates that an action class should be presented by an action button.
    /// </summary>
    public class ActionButtonAttribute : Attribute
    {
        /// <summary>
        /// Creates an action button registration.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="commandPath">The command path.</param>
        public ActionButtonAttribute(string commandName, string commandPath)
        {
            if (commandName == null)
                throw new ArgumentNullException("commandName");
            if (commandPath == null)
                throw new ArgumentNullException("commandPath");

            CommandName = commandName;
            CommandPath = commandPath;

            Caption = "";
            Tooltip = "";
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        public string CommandName { get; private set; }

        /// <summary>
        /// Gets the command path.
        /// </summary>
        public string CommandPath { get; private set; }

        /// <summary>
        /// Gets or sets the text label to display on the button.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the tooltip for the button.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the initial button status.
        /// </summary>
        public ActionButtonStatus ButtonStatus { get; set; }

        /// <summary>
        /// Gets or sets the button style.
        /// </summary>
        public ActionButtonStyle ButtonStyle { get; set; }

        /// <summary>
        /// Gets or sets the button state.
        /// </summary>
        public ActionButtonType ButtonType { get; set; }

        /// <summary>
        /// Gets a descriptor object formed from the attribute.
        /// </summary>
        /// <param name="actionType">The action type on which the attribute appears.</param>
        /// <returns>The descriptor.</returns>
        public ActionButtonDescriptor GetDescriptor(Type actionType)
        {
            return new ActionButtonDescriptor()
            {
                ActionType = actionType,
                CommandPath = CommandPath,
                CommandName = CommandName,
                Caption = Caption,
                Tooltip = Tooltip,
                ButtonStatus = ButtonStatus,
                ButtonStyle = ButtonStyle,
                ButtonType = ButtonType
            };
        }
    }
}
