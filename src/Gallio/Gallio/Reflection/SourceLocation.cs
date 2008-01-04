using System;
using System.Xml.Serialization;
using Gallio.Model.Serialization;

namespace Gallio.Reflection
{
    /// <summary>
    /// Specifies a position within a source document.
    /// </summary>
    /// <remarks>
    /// Lines and columns are numbered starting from 1.
    /// </remarks>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class SourceLocation : IEquatable<SourceLocation>
    {
        private string filename;
        private int line;
        private int column;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private SourceLocation()
        {
            filename = "";
        }

        /// <summary>
        /// Creates a document range.
        /// </summary>
        /// <param name="filename">The document file name</param>
        /// <param name="line">The line number, or 0 if unknown</param>
        /// <param name="column">The column number, or 0 if unknown</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filename"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="line"/> or <paramref name="column"/>
        /// is less than 0</exception>
        public SourceLocation(string filename, int line, int column)
        {
            Filename = filename;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Gets or sets the document file name.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlAttribute("filename")]
        public string Filename
        {
            get { return filename; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                filename = value;
            }
        }

        /// <summary>
        /// Gets the line number, or 0 if unknown.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        [XmlAttribute("line")]
        public int Line
        {
            get { return line; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                line = value;
            }
        }

        /// <summary>
        /// Gets or sets the column number, or 0 if unknown.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        [XmlAttribute("column")]
        public int Column
        {
            get { return column; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                column = value;
            }
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as SourceLocation);
        }

        /// <inheritdoc />
        public bool Equals(SourceLocation other)
        {
            return other != null
                && filename == other.filename
                && line == other.line
                && column == other.column;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return filename.GetHashCode() ^ line * 29 ^ column;
        }
    }
}