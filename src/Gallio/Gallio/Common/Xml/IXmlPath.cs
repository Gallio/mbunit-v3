using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Path to an element or an attribute in an XML fragment. 
    /// </summary>
    public interface IXmlPath
    {
    }

    /// <summary>
    /// Closed path to an element or an attribute in an XML fragment. 
    /// </summary>
    public interface IXmlPathClosed : IXmlPath
    {
    }

    /// <summary>
    /// Open path to an element or an attribute in an XML fragment. 
    /// </summary>
    public interface IXmlPathOpen : IXmlPath
    {
        /// <summary>
        /// Extends the path to the specified element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <returns>The extended path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementName"/> is null.</exception>
        IXmlPathOpen Element(string elementName);

        /// <summary>
        /// Extends the path to the specified element.
        /// </summary>
        /// <param name="elementName">The name of the element.</param>
        /// <param name="declarative">A value indicating whether the element is declarative.</param>
        /// <returns>The extended path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="elementName"/> is null.</exception>
        IXmlPathOpen Element(string elementName, bool declarative);

        /// <summary>
        /// Extends the path to the specified attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The extended path.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attributeName"/> is null.</exception>
        IXmlPathClosed Attribute(string attributeName);
    }
}
