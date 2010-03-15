using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Type of XML Node.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// A valid XML fragment.
        /// </summary>
        Fragment,

        /// <summary>
        /// An XML element.
        /// </summary>
        Element,

        /// <summary>
        /// A comment element.
        /// </summary>
        Comment,

        /// <summary>
        /// A textual content inside an element.
        /// </summary>
        Content,

        /// <summary>
        /// A declaration element at the beginning of an XML document.
        /// </summary>
        Declaration,

        /// <summary>
        /// An attribute.
        /// </summary>
        Attribute
    }
}
