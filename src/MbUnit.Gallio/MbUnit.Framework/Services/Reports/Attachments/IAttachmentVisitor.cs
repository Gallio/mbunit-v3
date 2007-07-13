using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// Visits attachments of various types.
    /// </summary>
    public interface IAttachmentVisitor
    {
        /// <summary>
        /// Visits a <see cref="TextAttachment" />.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        void VisitTextAttachment(TextAttachment attachment);

        /// <summary>
        /// Visits a <see cref="XmlAttachment" />.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        void VisitXmlAttachment(XmlAttachment attachment);

        /// <summary>
        /// Visits a <see cref="BinaryAttachment" />.
        /// </summary>
        /// <param name="attachment">The attachment</param>
        void VisitBinaryAttachment(BinaryAttachment attachment);
    }
}