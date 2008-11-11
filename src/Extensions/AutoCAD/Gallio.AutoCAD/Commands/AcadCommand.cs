using System;
using System.Collections.Generic;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// The base class for commands that can be sent to the AutoCAD process.
    /// </summary>
    internal abstract class AcadCommand
    {
        private readonly string globalName;

        /// <summary>
        /// Initializes a new <see cref="AcadCommand"/> object.
        /// </summary>
        /// <param name="globalName">The global name for the command.</param>
        protected AcadCommand(string globalName)
        {
            if (String.IsNullOrEmpty(globalName))
                throw new ArgumentException("Must not be null or empty.", "globalName");
            this.globalName = globalName;
        }

        /// <summary>Gets the command's arguments.</summary>
        public abstract IEnumerable<string> Arguments
        {
            get;
        }

        /// <summary>Gets the global name for this command.</summary>
        public string GlobalName
        {
            get { return globalName; }
        }

        /// <summary><c>true</c> if this command should be sent asynchonously; otherwise, <c>false</c>.</summary>
        public bool SendAsynchronously
        {
            get;
            set;
        }
    }
}
