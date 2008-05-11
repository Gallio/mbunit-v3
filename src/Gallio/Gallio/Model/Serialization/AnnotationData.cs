// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text;
using System.Xml.Serialization;
using Gallio.Reflection;
using Gallio.Runtime.Logging;
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

        /// <summary>
        /// Writes the annotation to a logger for presentation.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="includePrefix">If true, includes an identifying prefix to describe
        /// the annotation type, otherwise we assume that the logger will do its own
        /// thing based on the log severity</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public void Log(ILogger logger, bool includePrefix)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            StringBuilder message = new StringBuilder();
            if (includePrefix)
            {
                message.Append('[');
                message.Append(GetPrefixForAnnotation(Type));
                message.Append("] ");
            }

            message.Append(Message);

            if (CodeLocation != CodeLocation.Unknown)
            {
                message.Append("\n\tLocation: ");
                message.Append(CodeLocation);
            }

            if (CodeLocation.Line == 0 && CodeReference != CodeReference.Unknown)
            {
                message.Append("\n\tReference: ");
                message.Append(CodeReference);
            }

            if (!string.IsNullOrEmpty(Details))
            {
                message.Append("\n\tDetails: ");
                message.Append(Details);
            }

            LogSeverity severity = GetLogSeverityForAnnotation(Type);
            logger.Log(severity, message.ToString());
        }

        private static LogSeverity GetLogSeverityForAnnotation(AnnotationType type)
        {
            switch (type)
            {
                case AnnotationType.Error:
                    return LogSeverity.Error;

                case AnnotationType.Warning:
                    return LogSeverity.Warning;

                case AnnotationType.Info:
                    return LogSeverity.Info;

                default:
                    throw new ArgumentException("type");
            }
        }

        private static string GetPrefixForAnnotation(AnnotationType type)
        {
            switch (type)
            {
                case AnnotationType.Error:
                    return "error";

                case AnnotationType.Warning:
                    return "warning";

                case AnnotationType.Info:
                    return "info";

                default:
                    throw new ArgumentException("type");
            }
        }
    }
}
