using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// Provides traits for <see cref="IUtilityCommand" />.
    /// </summary>
    public class UtilityCommandTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates utility command traits.
        /// </summary>
        /// <param name="name">The unique name of the utility command</param>
        /// <param name="description">The description of the utility command</param>
        public UtilityCommandTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the unique name of the utility command.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of the utility command.
        /// </summary>
        public string Description
        {
            get { return description; }
        }
    }
}
