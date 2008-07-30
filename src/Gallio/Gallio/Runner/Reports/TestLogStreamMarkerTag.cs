using System;
using System.Xml.Serialization;
using Gallio.Model.Logging;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// An Xml-serializable container for a marker with a semantic class.
    /// This tag is used to mark regions of the log with semantic content
    /// that may be consumed by tools to achieve various effects.  A tool
    /// such as a report formatter may ignore the class and attributes of
    /// marker tags that it does not recognize.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public sealed class TestLogStreamMarkerTag : TestLogStreamContainerTag
    {
        private string @class;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestLogStreamMarkerTag()
        {
        }

        /// <summary>
        /// Creates an initialized tag.
        /// </summary>
        /// <param name="class">The marker class</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="class"/> is not a valid identifier.  <seealso cref="MarkerClasses.Validate"/></exception>
        public TestLogStreamMarkerTag(string @class)
        {
            MarkerClasses.Validate(@class);
            this.@class = @class;
        }

        /// <summary>
        /// Gets or sets the marker class, not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="value"/> is not a valid identifier.  <seealso cref="MarkerClasses.Validate"/></exception>
        [XmlAttribute("class")]
        public string Class
        {
            get { return @class; }
            set
            {
                MarkerClasses.Validate(@class);
                @class = value;
            }
        }

        /// <inheritdoc />
        public override void Accept(ITestLogStreamTagVisitor visitor)
        {
            visitor.VisitMarkerTag(this);
        }
    }
}