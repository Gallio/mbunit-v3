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
using System.Drawing;
using EnvDTE;
using EnvDTE80;
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;
using Gallio.VisualStudio.Shell.Core;
using Microsoft.VisualStudio.CommandBars;
using stdole;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// Installs, removes and executes Visual Studio commands.
    /// </summary>
    public class DefaultCommandManager : ICommandManager
    {
        private readonly ComponentHandle<ICommand, CommandTraits>[] commandHandles;
        private readonly DefaultShell shell;

        private Dictionary<string, CommandPresentation> commandPresentations;

        /// <summary>
        /// Initializes the command manager.
        /// </summary>
        /// <param name="commandHandles">The command handles.</param>
        /// <param name="shell">The shell.</param>
        public DefaultCommandManager(ComponentHandle<ICommand, CommandTraits>[] commandHandles, IShell shell)
        {
            this.commandHandles = commandHandles;
            this.shell = (DefaultShell) shell;

            commandPresentations = new Dictionary<string, CommandPresentation>();
        }

        internal void Initialize()
        {
            shell.ShellHooks.Exec += Exec;
            shell.ShellHooks.QueryStatus += QueryStatus;

            InstallActions();
        }

        internal void Shutdown()
        {
            shell.ShellHooks.Exec -= Exec;
            shell.ShellHooks.QueryStatus -= QueryStatus;
        }

        private void InstallActions()
        {
            if (commandPresentations != null)
                return;

            commandPresentations = new Dictionary<string, CommandPresentation>();

            foreach (var commandHandle in commandHandles)
            {
                CommandTraits traits = commandHandle.GetTraits();

                Commands2 commands = (Commands2) shell.DTE.Commands;

                Command command;
                try
                {
                    command = commands.Item(traits.CommandName, 0);
                }
                catch
                {
                    object[] contextGuids = null;
                    command = commands.AddNamedCommand2(shell.ShellAddIn,
                        traits.CommandName,
                        traits.Caption,
                        traits.Tooltip,
                        true, 59, ref contextGuids,
                        (int) ToVsCommandStatus(traits.Status),
                        (int) ToVsCommandStyle(traits.Style),
                        ToVsCommandControlType(traits.ControlType));
                }

                CommandBarButton[] controls = GenericCollectionUtils.ConvertAllToArray(traits.CommandBarPaths,
                    commandBarPath =>
                    {
                        CommandBar commandBar = GetCommandBar(commandBarPath);
                        CommandBarButton commandBarButton = GetOrCreateCommandBarButton(commandBar, command, commands);
                        return commandBarButton;
                    });

                CommandPresentation presentation = new CommandPresentation(commandHandle, command, controls);
                presentation.Icon = traits.Icon;
                presentation.Caption = traits.Caption;
                presentation.Tooltip = traits.Tooltip;
                presentation.Status = traits.Status;

                commandPresentations.Add(traits.CommandName, presentation);
            }
        }

        private CommandBarButton GetOrCreateCommandBarButton(CommandBar commandBar, Command command, Commands2 commands)
        {
            foreach (CommandBarControl control in commandBar.Controls)
            {
                CommandBarButton button = control as CommandBarButton;
                if (button != null)
                {
                    string guid;
                    int id;
                    commands.CommandInfo(control, out guid, out id);

                    if (command.Guid == guid)
                        return button;
                }
            }

            return (CommandBarButton)command.AddControl(commandBar, 1);
        }

        private CommandBar GetCommandBar(string commandPath)
        {
            string[] segments = commandPath.Split('\\');
            
            string commandBarName = segments[0];
            CommandBar commandBar = ((CommandBars)shell.DTE.CommandBars)[commandBarName];

            for (int i = 1; i < segments.Length; i++)
                commandBar = ((CommandBarPopup)commandBar.Controls[segments[i]]).CommandBar;

            return commandBar;
        }

        private void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus statusOption, ref object commandText)
        {
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                CommandPresentation commandPresentation;
                if (commandPresentations.TryGetValue(commandName, out commandPresentation))
                {
                    commandPresentation.UpdateCommandStatus();

                    statusOption = ToVsCommandStatus(commandPresentation.Status);
                }
                else
                {
                    statusOption = vsCommandStatus.vsCommandStatusUnsupported;
                }
            }
        }

        private void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            handled = false;

            if (executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                CommandPresentation commandPresentation;
                if (commandPresentations.TryGetValue(commandName, out commandPresentation))
                {
                    commandPresentation.ExecuteCommand();
                    handled = true;
                }
            }
        }

        private static vsCommandStatus ToVsCommandStatus(CommandStatus status)
        {
            switch (status)
            {
                case CommandStatus.Enabled:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                case CommandStatus.Invisible:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusInvisible;
                case CommandStatus.Latched:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusLatched;
                case CommandStatus.Ninched:
                    return vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusNinched;
                default:
                    throw new ArgumentOutOfRangeException("status");
            }
        }

        private static vsCommandStyle ToVsCommandStyle(CommandStyle style)
        {
            switch (style)
            {
                case CommandStyle.PictureAndText:
                    return vsCommandStyle.vsCommandStylePictAndText;
                case CommandStyle.Picture:
                    return vsCommandStyle.vsCommandStylePict;
                case CommandStyle.Text:
                    return vsCommandStyle.vsCommandStyleText;
                default:
                    throw new ArgumentOutOfRangeException("style");
            }
        }

        private static vsCommandControlType ToVsCommandControlType(CommandControlType controlType)
        {
            switch (controlType)
            {
                case CommandControlType.Button:
                    return vsCommandControlType.vsCommandControlTypeButton;
                case CommandControlType.DropDown:
                    return vsCommandControlType.vsCommandControlTypeDropDownCombo;
                default:
                    throw new ArgumentOutOfRangeException("controlType");
            }
        }

        private class CommandPresentation : ICommandPresentation
        {
            private readonly ComponentHandle<ICommand, CommandTraits> commandHandle;
            private readonly Command vsCommand;
            private readonly CommandBarButton[] vsCommandBarButtons;

            private Icon icon;
            private string caption;
            private string tooltip;
            private CommandStatus status;

            public CommandPresentation(ComponentHandle<ICommand, CommandTraits> commandHandle,
                Command vsCommand, CommandBarButton[] vsCommandBarButtons)
            {
                this.commandHandle = commandHandle;
                this.vsCommand = vsCommand;
                this.vsCommandBarButtons = vsCommandBarButtons;
            }

            public ComponentHandle<ICommand, CommandTraits> CommandHandle
            {
                get { return commandHandle; }
            }

            public Command VsCommand
            {
                get { return vsCommand; }
            }

            public CommandBarButton[] VsCommandBarButtons
            {
                get { return vsCommandBarButtons; }
            }

            public string Caption
            {
                get { return caption; }
                set
                {
                    caption = value;

                    foreach (var vsCommandBarButton in vsCommandBarButtons)
                        vsCommandBarButton.Caption = value;
                }
            }

            public string Tooltip
            {
                get { return tooltip; }
                set
                {
                    tooltip = value;

                    foreach (var vsCommandBarButton in vsCommandBarButtons)
                        vsCommandBarButton.TooltipText = value;
                }
            }

            public Icon Icon
            {
                get { return icon; }
                set
                {
                    icon = value;

                    StdPicture picture = (StdPicture)ImageConversionUtils.GetIPictureDispFromImage(value.ToBitmap());
                    foreach (var vsCommandBarButton in vsCommandBarButtons)
                        vsCommandBarButton.Picture = picture;
                }
            }

            public CommandStatus Status
            {
                get { return status; }
                set { status = value; }
            }

            public void ExecuteCommand()
            {
                commandHandle.GetComponent().Execute(this);
            }

            public void UpdateCommandStatus()
            {
                commandHandle.GetComponent().Update(this);
            }
        }
    }
}
