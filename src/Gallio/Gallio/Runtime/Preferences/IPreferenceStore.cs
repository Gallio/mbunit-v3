using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// Stores and retrieves the contents of preference sets.
    /// </summary>
    /// <seealso cref="IPreferenceSet"/>
    /// <seealso cref="IPreferenceManager"/>
    public interface IPreferenceStore
    {
        /// <summary>
        /// Gets the preference set with the specified name.
        /// </summary>
        /// <param name="preferenceSetName">The name of the preference set</param>
        /// <returns>The preference set</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="preferenceSetName"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="preferenceSetName"/> is empty</exception>
        IPreferenceSet this[string preferenceSetName] { get; }
    }
}
