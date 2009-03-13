using System;
using System.Xml.Serialization;
using Gallio.Runtime.Logging;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Describes a log entry for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the log message severity.
        /// </summary>
        [XmlAttribute("severity")]
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        [XmlAttribute("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the log message details, such as an exception.
        /// </summary>
        [XmlAttribute("details")]
        public string Details { get; set; }
    }
}
