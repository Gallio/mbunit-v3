using System;
using System.Collections.Generic;
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The abstract base type for MbUnit attributes that contribute values to data sources
    /// along with metadata such a description or expected exception type.
    /// </para>
    /// </summary>
    /// <seealso cref="DataPatternAttribute"/> for more information about data binding attributes in general.
    public abstract class DataAttribute : DataPatternAttribute
    {
        private string description;
        private Type expectedException;

        /// <summary>
        /// Gets or sets a description of the values provided by the data source.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets or sets the type of exception that should be thrown when the
        /// values provided by the data source are consumed by test.
        /// </summary>
        public Type ExpectedException
        {
            get { return expectedException; }
            set { expectedException = value; }
        }

        /// <summary>
        /// Gets the metadata for the data source.
        /// </summary>
        /// <returns>The metadata keys and values</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            if (description != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.Description, description);
            if (expectedException != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.ExpectedException, expectedException.FullName);
        }
    }
}
