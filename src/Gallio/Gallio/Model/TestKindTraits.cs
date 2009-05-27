using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Specifies traits for <see cref="ITestKind" />.
    /// </summary>
    public class TestKindTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates test kind traits.
        /// </summary>
        /// <param name="name">The test kind name</param>
        /// <param name="description">The test kind description</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="description" /> is null</exception>
        public TestKindTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the name of the test kind.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of the test kind.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets or sets the icon for the test kind, or null if none.
        /// </summary>
        /// <remarks>
        /// The icon should be available in a 16x16 size.
        /// </remarks>
        public Icon Icon { get; set; }
    }
}
