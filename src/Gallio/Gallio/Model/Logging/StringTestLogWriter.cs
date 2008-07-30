using System;
using System.IO;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="TestLogStreamWriter" /> that appends its output to a string.
    /// </para>
    /// and discards unrepresentable formatting details.  Section names are printed
    /// simply as headers on their own line.
    /// </summary>
    public class StringTestLogWriter : TextualTestLogWriter
    {
        /// <summary>
        /// Creates a log stream writer for the <see cref="TestLogStreamNames.Default" /> stream.
        /// </summary>
        /// <param name="verbose">If true, prints detailed information about the location of
        /// attachments, sections, and markers, otherwise discards these formatting details
        /// and prints section headers as text on their own line</param>
        public StringTestLogWriter(bool verbose)
            : base(CreateStringWriter(), verbose)
        {
        }

        /// <summary>
        /// Gets the formatted log contents as a string.
        /// </summary>
        /// <returns>The contents as a string</returns>
        public override string ToString()
        {
            return Writer.ToString();
        }

        private static TextWriter CreateStringWriter()
        {
            StringWriter writer = new StringWriter();
            writer.NewLine = "\n";
            return writer;
        }
    }
}
