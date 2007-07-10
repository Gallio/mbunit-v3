using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;

namespace MbUnit.Core.Services.Assert
{
    /// <summary>
    /// Describes an assertion that is to be performed.  The product of
    /// an assertion is an <see cref="AssertionResult" /> which may compose
    /// results to handle scenarios where multiple independent verifications
    /// have taken place but are logically just parts of one larger assertion.
    /// </summary>
    [Serializable]
    [XmlType]
    [XmlRoot("assertion")]
    public sealed class Assertion
    {
        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public Assertion()
        {
        }

        /// <summary>
        /// Creates an assertion.
        /// </summary>
        /// <param name="id">The assertion id</param>
        /// <param name="description">The assertion description</param>
        /// <param name="detailMessage">The assertion detail message supplied by the client or "" if none</param>
        /// <param name="argNamesAndValues">An alternating sequence of name / value pairs
        /// describing the assertion's arguments, or null or empty if none</param>
        public Assertion(string id, string description, string detailMessage, params object[] argNamesAndValues)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (description == null)
                throw new ArgumentNullException("description");
            if (detailMessage == null)
                throw new ArgumentNullException("detailMessage");
            if (argNamesAndValues != null && argNamesAndValues.Length % 2 == 1)
                throw new ArgumentException("The array of argument name and value pairs must have an even number of elements.", "argNamesAndValues");

            this.id = id;
            this.description = description;
            this.detailMessage = detailMessage;
            this.argNamesAndValues = argNamesAndValues;
        }

        /// <summary>
        /// Gets or sets the assertion id.
        /// </summary>
        [XmlAttribute("id")]
        public string Id
        {
            get { return id; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                id = value;
            }
        }
        private string id = "Unknown";

        /// <summary>
        /// Gets or sets a description of the assertion.
        /// </summary>
        [XmlAttribute("description")]
        public string Description
        {
            get { return description; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                description = value;
            }
        }
        private string description = "Verifies some unspecified condition.";

        /// <summary>
        /// Gets the formatted detail message supplied by the client.
        /// May be blank but never null.
        /// </summary>
        [XmlAttribute("detailMessage")]
        public string DetailMessage
        {
            get { return detailMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                detailMessage = value;
            }
        }
        private string detailMessage = "";

        /// <summary>
        /// Gets the dictionary of assertion argument values by name.
        /// </summary>
        public IDictionary<string, object> Args
        {
            get
            {
                if (args == null)
                {
                    IDictionary<string, object> newArgs = new Dictionary<string, object>(argNamesAndValues.Length / 2);

                    if (argNamesAndValues != null)
                    {
                        for (int i = 0; i < argNamesAndValues.Length; i += 2)
                        {
                            newArgs[(string)argNamesAndValues[i]] = argNamesAndValues[i + 1];
                        }
                    }

                    // Use Interlocked.CompareExchange for safety with other threads that may
                    // concurrently populate the field: the first one to do so wins which is just fine.
                    Interlocked.CompareExchange(ref args, newArgs, null);
                }

                return args;
            }
        }
        private IDictionary<string, object> args;
        private object[] argNamesAndValues;
    }
}
