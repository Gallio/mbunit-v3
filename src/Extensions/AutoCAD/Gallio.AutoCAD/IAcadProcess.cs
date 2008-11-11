using System;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Represents the AutoCAD process.
    /// </summary>
    public interface IAcadProcess : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="IRemoteTestDriver"/> instance.
        /// </summary>
        /// <returns>The <see cref="IRemoteTestDriver"/> instance.</returns>
        IRemoteTestDriver GetRemoteTestDriver();
    }
}
