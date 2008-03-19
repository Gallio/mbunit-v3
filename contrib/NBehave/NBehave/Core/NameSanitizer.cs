using System;
using System.Text;

namespace NBehave.Core
{
    /// <summary>
    /// Provides utilities for creating good names for concerns, contexts and specifications.
    /// </summary>
    public static class NameSanitizer
    {
        /// <summary>
        /// <para>
        /// Creates a sanitized name from an identifier.
        /// </para>
        /// <para>
        /// Supports the translation of underscore ('_') delimited and camel-case identifiers
        /// into space delimited phrases.
        /// </para>
        /// </summary>
        /// <param name="identifier">The class or method identifier</param>
        /// <returns>The name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="identifier"/> is null</exception>
        public static string MakeNameFromIdentifier(string identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException("identifier");

            StringBuilder name = new StringBuilder();

            foreach (char c in identifier)
            {
                if (c == '_')
                {
                    name.Append(' ');
                }
                else
                {
                    if (name.Length != 0 && char.IsUpper(c))
                        name.Append(' ');

                    name.Append(c);
                }
            }

            return name.ToString();
        }
    }
}
