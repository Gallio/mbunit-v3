using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// Path to a node (element or attribute) in an XML fragment. 
    /// </summary>
    public sealed class Path
    {
        private readonly Path innerPath;
        private readonly string elementName;
        private readonly bool isDeclaration;

        /// <summary>
        /// Empty path.
        /// </summary>
        public readonly static Path Empty = new Path();

        private Path()
        {
        }

        /// <summary>
        /// Extends the current path by appending the specified child element.
        /// </summary>
        /// <param name="elementName">The name of the child element.</param>
        /// <returns>A new path instance that extends the current one.</returns>
        public Path Extend(string elementName)
        {
            return Extend(elementName, false);
        }

        /// <summary>
        /// Extends the current path by appending the specified child element.
        /// </summary>
        /// <param name="elementName">The name of the child element.</param>
        /// <param name="isDeclaration">Determines whether the element is the declaration element of an XML document.</param>
        /// <returns>A new path instance that extends the current one.</returns>
        public Path Extend(string elementName, bool isDeclaration)
        {
            if (elementName == null)
                throw new ArgumentNullException("elementName");

            return new Path(this, elementName, isDeclaration);
        }

        private Path(Path innerPath, string elementName, bool isDeclaration)
        {
            this.innerPath = innerPath;
            this.elementName = elementName;
            this.isDeclaration = isDeclaration;
        }

        /// <summary>
        /// Resolves the entire path.
        /// </summary>
        /// <returns>A string describing the path to the XML element.</returns>
        public override string ToString()
        {
            return ToString(String.Empty);
        }

        /// <summary>
        /// Resolves the entire path.
        /// </summary>
        /// <param name="attributeName">The name of the targeted attribute in the last child element.</param>
        /// <returns>A string describing the path to the XML element.</returns>
        public string ToString(string attributeName)
        {
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");

            var output = new StringBuilder();

            if (innerPath != null)
                output.Append(innerPath.ToString());

            if (elementName != null)
            {
                if (attributeName.Length > 0)
                {
                    output.AppendFormat("<{0}{1} {2}='...'{3}>", isDeclaration ? "?" : String.Empty, elementName, attributeName, isDeclaration ? " ?" : String.Empty);
                }
                else
                {
                    output.AppendFormat("<{0}{1}{0}>", isDeclaration ? "?" : String.Empty, elementName);
                }
            }

            return output.ToString();
        }
    }
}
