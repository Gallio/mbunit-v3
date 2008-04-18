using System;
using System.IO;

namespace Gallio.Utilities
{
    /// <summary>
    /// Sets <see cref="Environment.CurrentDirectory" /> when created, then
    /// restores it when disposed.
    /// </summary>
    public class CurrentDirectorySwitcher : IDisposable
    {
        private string oldDirectory;

        /// <summary>
        /// Saves the current directory then changes it to the specified value.
        /// </summary>
        /// <param name="directory">The new directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="directory"/> is null</exception>
        /// <exception cref="IOException">Thrown if the current directory could not be set</exception>
        public CurrentDirectorySwitcher(string directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            oldDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = directory;
        }

        /// <summary>
        /// Resets the current directory to its original saved value.
        /// </summary>
        /// <exception cref="IOException">Thrown if the current directory could not be reset</exception>
        public void Dispose()
        {
            if (oldDirectory != null)
            {
                Environment.CurrentDirectory = oldDirectory;
                oldDirectory = null;
            }
        }
    }
}
