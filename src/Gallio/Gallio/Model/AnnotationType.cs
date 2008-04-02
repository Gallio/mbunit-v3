using System;
using System.Xml.Serialization;

namespace Gallio.Model
{
    /// <summary>
    /// Specifies the type of an annotation.
    /// </summary>
    public enum AnnotationType
    {
        /// <summary>
        /// An informational annotation.
        /// </summary>
        [XmlEnum("info")]
        Info,

        /// <summary>
        /// A warning annotation.
        /// </summary>
        [XmlEnum("warning")]
        Warning,

        /// <summary>
        /// An error annotation.
        /// </summary>
        [XmlEnum("error")]
        Error
    }
}
