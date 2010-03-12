using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Text aggregator used to format an XML path against an existing fragment.
    /// </summary>
    public class XmlPathFormatAggregator
    {
        private readonly IList<string> lines = new List<string>();
        private string pendingAttribute;
        private string pendingContent;

        /// <summary>
        /// Ellipsis text.
        /// </summary>
        public const string Ellipsis = "…";

        /// <summary>
        /// Adds a line of text.
        /// </summary>
        /// <param name="line">The line of text to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="line"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="line"/> is empty.</exception>
        public void Add(string line)
        {
            if (line == null)
                throw new ArgumentNullException("line");
            if (line.Length == 0)
                throw new ArgumentException("Cannot be empty", "line");

            lines.Add(line);
            ResetPendingItems();
        }

        private void ResetPendingItems()
        {
            pendingAttribute = null;
            pendingContent = null;
        }

        /// <summary>
        /// Stores the representation of an attribute in its parent element for later use.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="isFirst">Determines whether the attribute is the first in the parent collection.</param>
        /// <param name="isLast">Determines whether the attribute is the last in the parent collection.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
        public void HangAttribute(string name, string value, bool isFirst, bool isLast)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Cannot be empty", "name");
            if (value == null)
                throw new ArgumentNullException("value");

            pendingAttribute = String.Format(" {0}{1}='{2}'{3}", (isFirst ? String.Empty : Ellipsis + " "), name, value, (isLast ? String.Empty : " " + Ellipsis));
        }

        /// <summary>
        /// Gets the representation of the pending attribute, or null if none.
        /// </summary>
        public string PendingAttribute
        {
            get
            {
                return pendingAttribute;
            }
        }

        /// <summary>
        /// Stores the textual content of an element for later use.
        /// </summary>
        /// <param name="text">The textual content.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public void HangContent(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            pendingContent = text;
        }


        /// <summary>
        /// Gets the textual content of the pending element, or null if none.
        /// </summary>
        public string PendingContent
        {
            get
            {
                return pendingContent;
            }
        }

        /// <summary>
        /// Returns the output result.
        /// </summary>
        public string GetResult()
        {
            var output = new StringBuilder();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                output.Append(lines[i]);
            }

            return output.ToString();
        }
    }
}
