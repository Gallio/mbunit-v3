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
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

namespace Gallio.VisualStudio.Shell.Actions
{
    /// <summary>
    /// Manages installation and removal of actions as well as action command dispatch.
    /// </summary>
    internal class ShellActionManager : BaseShellExtension, IActionManager
    {
        private readonly Dictionary<Type, Action> installedActions;
        private readonly Dictionary<string, ActionButton> installedActionButtons;

        public ShellActionManager()
        {
            installedActions = new Dictionary<Type, Action>();
            installedActionButtons = new Dictionary<string, ActionButton>();
        }

        /// <summary>
        /// Installs an action button specified by the provided descriptor.
        /// </summary>
        /// <param name="descriptor">The action button descriptor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="descriptor"/> is null</exception>
        public void InstallActionButton(ActionButtonDescriptor descriptor)
        {
            if (descriptor == null)
                throw new ArgumentNullException("descriptor");

            Action action;
            if (!installedActions.TryGetValue(descriptor.ActionType, out action))
            {
                action = (Action) Activator.CreateInstance(descriptor.ActionType);
                installedActions.Add(descriptor.ActionType, action);
            }

            object[] contextGUIDS = null;
            Commands2 commands = (Commands2)Shell.DTE.Commands;

            Command command = commands.AddNamedCommand2(Shell.AddIn, descriptor.CommandName,
                descriptor.Caption, descriptor.Tooltip, true, 59, ref contextGUIDS,
                (int)GetCommandStatus(descriptor.ButtonStatus),
                (int)GetCommandStyle(descriptor.ButtonStyle),
                GetCommandControlType(descriptor.ButtonType));

            CommandBar commandBar = GetCommandBar(descriptor.CommandPath);
            CommandBarButton control = (CommandBarButton)command.AddControl(commandBar, 1);

            ActionButton actionButton = new ActionButton(Shell, action, command, control);
            installedActionButtons.Add(command.Name, actionButton);
        }

        protected override void InitializeImpl()
        {
            foreach (ActionButtonDescriptor descriptor in GetActionButtonDescriptors())
                InstallActionButton(descriptor);
        }

        protected override void ShutdownImpl()
        {
            foreach (ActionButton actionButton in installedActionButtons.Values)
                actionButton.VSCommand.Delete();

            installedActionButtons.Clear();
            installedActions.Clear();
        }

        private CommandBar GetCommandBar(string commandPath)
        {
            string[] segments = commandPath.Split('\\');
            
            string commandBarName = segments[0];
            CommandBar commandBar = ((CommandBars)Shell.DTE.CommandBars)[commandBarName];

            for (int i = 1; i < segments.Length; i++)
                commandBar = ((CommandBarPopup)commandBar.Controls[segments[i]]).CommandBar;

            return commandBar;
        }

        private IEnumerable<ActionButtonDescriptor> GetActionButtonDescriptors()
        {
            foreach (Type type in Shell.GetType().Assembly.GetTypes())
            {
                foreach (ActionButtonAttribute attrib in type.GetCustomAttributes(typeof(ActionButtonAttribute), true))
                {
                    yield return attrib.GetDescriptor(type);
                }
            }
        }

        internal void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                ActionButton actionButton;
                if (installedActionButtons.TryGetValue(commandName, out actionButton))
                {
                    actionButton.Update();

                    statusOption = GetCommandStatus(actionButton.Status);
                }
                else
                {
                    statusOption = vsCommandStatus.vsCommandStatusUnsupported;
                }
            }
        }

        internal void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            handled = false;

            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                ActionButton actionButton;
                if (installedActionButtons.TryGetValue(commandName, out actionButton))
                {
                    actionButton.Execute();
                    handled = true;
                }
            }
        }

        private static vsCommandStatus GetCommandStatus(ActionButtonStatus buttonStatus)
        {
            switch (buttonStatus)
            {
                case ActionButtonStatus.Enabled:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                case ActionButtonStatus.Invisible:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusInvisible;
                case ActionButtonStatus.Latched:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusLatched;
                case ActionButtonStatus.Ninched:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusNinched;
                default:
                    throw new ArgumentOutOfRangeException("buttonStatus");
            }
        }

        private static vsCommandStyle GetCommandStyle(ActionButtonStyle buttonStyle)
        {
            switch (buttonStyle)
            {
                case ActionButtonStyle.PictureAndText:
                    return vsCommandStyle.vsCommandStylePictAndText;
                case ActionButtonStyle.Picture:
                    return vsCommandStyle.vsCommandStylePict;
                case ActionButtonStyle.Text:
                    return vsCommandStyle.vsCommandStyleText;
                default:
                    throw new ArgumentOutOfRangeException("buttonStyle");
            }
        }

        private static vsCommandControlType GetCommandControlType(ActionButtonType buttonType)
        {
            switch (buttonType)
            {
                case ActionButtonType.Button:
                    return vsCommandControlType.vsCommandControlTypeButton;
                case ActionButtonType.DropDown:
                    return vsCommandControlType.vsCommandControlTypeDropDownCombo;
                default:
                    throw new ArgumentOutOfRangeException("buttonType");
            }
        }
    }
}
