using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// A preference store implementation based on storing preference sets as files within a directory.
    /// </summary>
    public class FilePreferenceStore : IPreferenceStore
    {
        private readonly Dictionary<string, IPreferenceSet> cachedPreferenceSets;
        private readonly DirectoryInfo preferenceStoreDir;

        /// <summary>
        /// Creates a file preference store.
        /// </summary>
        /// <param name="preferenceStoreDir">The preference store directory</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="preferenceStoreDir"/> is null</exception>
        public FilePreferenceStore(DirectoryInfo preferenceStoreDir)
        {
            if (preferenceStoreDir == null)
                throw new ArgumentNullException("preferenceStoreDir");

            this.preferenceStoreDir = preferenceStoreDir;

            cachedPreferenceSets = new Dictionary<string, IPreferenceSet>();
        }

        /// <summary>
        /// Gets the preference store directory.
        /// </summary>
        public DirectoryInfo PreferenceStoreDir
        {
            get { return new DirectoryInfo(preferenceStoreDir.ToString()); }
        }

        /// <inheritdoc />
        public IPreferenceSet this[string preferenceSetName]
        {
            get
            {
                if (preferenceSetName == null)
                    throw new ArgumentNullException("name");

                return GetPreferenceSet(preferenceSetName);
            }
        }

        private IPreferenceSet GetPreferenceSet(string preferenceSetName)
        {
            lock (cachedPreferenceSets)
            {
                IPreferenceSet result;
                if (!cachedPreferenceSets.TryGetValue(preferenceSetName, out result))
                {
                    FileInfo preferenceSetFile = new FileInfo(Path.Combine(preferenceStoreDir.FullName, preferenceSetName + ".gallioprefs"));
                    result = new FilePreferenceSet(preferenceSetFile);
                    cachedPreferenceSets.Add(preferenceSetName, result);
                }

                return result;
            }
        }
    }
}
