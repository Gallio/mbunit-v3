using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gallio.VisualStudio.Shell.UI.Commands;

namespace Gallio.VisualStudio.Sail.UI.Commands
{
    public class RunTestsCommand : BaseCommand
    {
        public override void Execute(ICommandPresentation presentation)
        {
            MessageBox.Show("Run tests clicked!");
        }
    }
}
