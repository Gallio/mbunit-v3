using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Toolkit.Actions
{
    public class ActionButtonAttribute : Attribute
    {
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

        public string CommandName { get; private set; }
        public string CommandPath { get; private set; }

        public string Caption { get; set; }
        public string Tooltip { get; set; }
        public ActionButtonStatus ButtonStatus { get; set; }
        public ActionButtonStyle ButtonStyle { get; set; }
        public ActionButtonType ButtonType { get; set; }

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
