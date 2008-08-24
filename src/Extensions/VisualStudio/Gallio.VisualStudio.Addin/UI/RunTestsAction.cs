using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Gallio.VisualStudio.Toolkit.Actions;

namespace Gallio.VisualStudio.Addin.UI
{
    [ActionButton("HelloWorld", @"MenuBar\Tools", Caption="Hello World!", Tooltip="Blah")]
    public class RunTestsAction : Action
    {
        public override void Execute(ActionButton button)
        {
            TestConsole console = new TestConsole(button.Shell);
            console.Show();
        }
    }
}
