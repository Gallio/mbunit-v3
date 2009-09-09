using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// Abstract implementation of an command that does nothing.
    /// </summary>
    public abstract class BaseCommand : ICommand
    {
        /// <inheritdoc />
        public virtual void Execute(ICommandPresentation presentation)
        {
        }

        /// <inheritdoc />
        public virtual void Update(ICommandPresentation presentation)
        {
        }
    }
}
