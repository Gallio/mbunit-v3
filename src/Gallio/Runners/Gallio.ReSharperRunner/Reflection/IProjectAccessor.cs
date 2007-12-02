using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.ProjectModel;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Gets the project to which a code element belongs.
    /// </summary>
    public interface IProjectAccessor
    {
        /// <summary>
        /// Gets the project to which a code element belongs, or null if none.
        /// </summary>
        IProject Project { get; }
    }
}
