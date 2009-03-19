using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Provides support for finding and launching instances of VisualStudio.
    /// </summary>
    public interface IVisualStudioManager
    {
        /// <summary>
        /// Gets the active instance of Visual Studio, or spawns one if requested.
        /// </summary>
        /// <param name="version">The version of Visual Studio to find, or <see cref="VisualStudioVersion.Any"/> for any version</param>
        /// <param name="launchIfNoActiveInstance">If true, launches an instance if there is no active Visual Studio yet</param>
        /// <returns>The Visual Studio instance, or null on failure</returns>
        IVisualStudio GetVisualStudio(VisualStudioVersion version, bool launchIfNoActiveInstance);

        /// <summary>
        /// Launches the latest installed version of Visual Studio.
        /// </summary>
        /// <param name="version">The version of Visual Studio to launch, or <see cref="VisualStudioVersion.Any"/> for the most recent version</param>
        /// <returns>The Visual Studio instance, or null on failure</returns>
        IVisualStudio LaunchVisualStudio(VisualStudioVersion version);
    }
}
