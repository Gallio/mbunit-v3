using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Describes traits of an <see cref="IReportFormatter"/> component.
    /// </summary>
    public class ReportFormatterTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates report formatter traits.
        /// </summary>
        /// <param name="name">The unique name of the report format</param>
        /// <param name="description">The description of report format</param>
        public ReportFormatterTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the unique name of the report format.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of the report format.
        /// </summary>
        public string Description
        {
            get { return description; }
        }
    }
}
