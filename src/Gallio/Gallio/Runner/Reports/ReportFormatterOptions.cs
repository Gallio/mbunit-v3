using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Gallio.Collections;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Provides options to report formatters.
    /// </summary>
    [Serializable]
    public sealed class ReportFormatterOptions
    {
        private readonly PropertySet properties;

        /// <summary>
        /// Creates a default set of options.
        /// </summary>
        public ReportFormatterOptions()
        {
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets a mutable collection of key/value pairs that specify configuration properties
        /// for the report formatter.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy</returns>
        public ReportFormatterOptions Copy()
        {
            ReportFormatterOptions copy = new ReportFormatterOptions();
            copy.properties.AddAll(properties);

            return copy;
        }
    }
}
