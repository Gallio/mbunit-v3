using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Toolkit.Actions
{
    /// <summary>
    /// Describes an action button to be installed.
    /// </summary>
    public class ActionButtonDescriptor
    {
        public Type ActionType { get; set; }
        public string CommandName { get; set; }
        public string CommandPath { get; set; }

        public string Caption { get; set; }
        public string Tooltip { get; set; }
        public ActionButtonStatus ButtonStatus { get; set; }
        public ActionButtonStyle ButtonStyle { get; set; }
        public ActionButtonType ButtonType { get; set; }
    }
}
