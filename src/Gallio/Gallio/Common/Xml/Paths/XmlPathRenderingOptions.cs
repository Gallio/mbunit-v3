using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml.Paths
{
    /// <summary>
    /// Rendering options for the XML path renderer.
    /// </summary>
    [Flags]
    internal enum XmlPathRenderingOptions
    {
        /// <summary>
        /// No options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Prints each element in its own indented line.
        /// </summary>
        UseIndentation = 1,
    }
}
