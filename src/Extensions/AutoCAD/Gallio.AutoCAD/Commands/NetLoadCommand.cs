using System.Collections.Generic;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Maps to the <c>NETLOAD</c> command.
    /// </summary>
    internal class NetLoadCommand : AcadCommand
    {
        /// <summary>
        /// Initializes a new <see cref="NetLoadCommand"/> object.
        /// </summary>
        public NetLoadCommand()
            : base("NETLOAD")
        {
        }

        /// <inheritdoc/>
        public override IEnumerable<string> Arguments
        {
            get
            {
                yield return AssemblyPath;
            }
        }

        /// <summary>
        /// Gets or sets the path to the assembly to load.
        /// </summary>
        public string AssemblyPath { get; set; }
    }
}
