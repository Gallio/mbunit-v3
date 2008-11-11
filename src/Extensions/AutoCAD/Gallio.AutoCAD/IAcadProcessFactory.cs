using System;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Creates <see cref="IAcadProcess"/> objects.
    /// </summary>
    public interface IAcadProcessFactory
    {
        /// <summary>
        /// Creates a new AutoCAD process.
        /// </summary>
        /// <returns>The new process.</returns>
        IAcadProcess CreateProcess();
    }
}
