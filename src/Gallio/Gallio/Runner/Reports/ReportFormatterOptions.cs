using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Provides options to report formatters.
    /// </summary>
    [Serializable]
    public sealed class ReportFormatterOptions
    {
        private readonly NameValueCollection properties;

        /// <summary>
        /// Creates a default set of options.
        /// </summary>
        public ReportFormatterOptions()
        {
            properties = new NameValueCollection();
        }

        /// <summary>
        /// Gets a mutable collection of key/value pairs that specify configuration properties
        /// for the report formatter.
        /// </summary>
        public NameValueCollection Properties
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
            copy.properties.Add(properties);

            return copy;
        }
    }
}
