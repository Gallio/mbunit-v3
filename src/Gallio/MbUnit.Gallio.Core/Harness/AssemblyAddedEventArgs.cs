using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// Event arguments for <see cref="ITestHarness.AssemblyAdded" />.
    /// </summary>
    [Serializable]
    public class AssemblyAddedEventArgs : EventArgs
    {
        private Assembly assembly;

        /// <summary>
        /// Creates event arguments.
        /// </summary>
        /// <param name="assembly">The assembly that was added</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        public AssemblyAddedEventArgs(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.assembly = assembly;
        }

        /// <summary>
        /// Gets the assembly that was added.
        /// </summary>
        public Assembly Assembly
        {
            get { return assembly; }
        }
    }
}
