using System;

namespace Gallio.ReSharperRunner.Provider.Facade
{
    /// <summary>
    /// Specifies task execution configuration information.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the ReSharper interfaces.
    /// </remarks>
    [Serializable]
    public class FacadeTaskExecutorConfiguration
    {
        /// <summary>
        /// Gets or sets whether task assemblies should be shadow-copied.
        /// </summary>
        public bool ShadowCopy { get; set; }

        /// <summary>
        /// Gets or sets the folder to use as the base directory for task assemblies.
        /// Or null if the project's assembly should be used.
        /// </summary>
        public string AssemblyFolder { get; set; }
    }
}
