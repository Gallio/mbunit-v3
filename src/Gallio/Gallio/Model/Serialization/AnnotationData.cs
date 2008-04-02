using System;
using System.Xml.Serialization;
using Gallio.Reflection;
using Gallio.Utilities;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// Describes an annotation in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="Annotation"/>
    [Serializable]
    [XmlRoot("annotation", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public class AnnotationData
    {
        private AnnotationType type;
        private CodeLocation codeLocation;
        private CodeReference codeReference;
        private string message;
        private string details;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private AnnotationData()
        {
        }

        /// <summary>
        /// Creates an annotation.
        /// </summary>
        /// <param name="type">The annotation type</param>
        /// <param name="codeLocation">The code location</param>
        /// <param name="codeReference">The code reference</param>
        /// <param name="message">The annotation message</param>
        /// <param name="details">Additional details such as exception text or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null</exception>
        public AnnotationData(AnnotationType type, CodeLocation codeLocation, CodeReference codeReference, string message, string details)
        {
            this.type = type;
            this.codeLocation = codeLocation;
            this.codeReference = codeReference;
            this.message = message;
            this.details = details;
        }

        /// <summary>
        /// Copies the contents of an annotation.
        /// </summary>
        /// <param name="source">The source annotation</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public AnnotationData(Annotation source)
            : this(source.Type,
                source.CodeElement != null ? source.CodeElement.GetCodeLocation() : CodeLocation.Unknown,
                source.CodeElement != null ? source.CodeElement.CodeReference : CodeReference.Unknown,
                source.Message, source.Details)
        {
        }

        /// <summary>
        /// Gets or sets the annotation type.
        /// </summary>
        [XmlAttribute("type")]
        public AnnotationType Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Gets or sets the code location associated with the annotation.
        /// </summary>
        [XmlElement("codeLocation")]
        public CodeLocation CodeLocation
        {
            get { return codeLocation; }
            set { codeLocation = value; }
        }

        /// <summary>
        /// Gets or sets the code reference associated with the annotation.
        /// </summary>
        [XmlElement("codeReference")]
        public CodeReference CodeReference
        {
            get { return codeReference; }
            set { codeReference = value; }
        }

        /// <summary>
        /// Gets or sets the annotation message.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("message")]
        public string Message
        {
            get { return message; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                message = value;
            }
        }

        /// <summary>
        /// Gets or sets additional details such as exception text, or null if none.
        /// </summary>
        [XmlAttribute("details")]
        public string Details
        {
            get { return details; }
            set { details = value; }
        }
    }
}
