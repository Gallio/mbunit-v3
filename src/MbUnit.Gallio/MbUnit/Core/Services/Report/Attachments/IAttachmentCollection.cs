using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Collections;

namespace MbUnit.Core.Services.Report.Attachments
{
    /// <summary>
    /// A collection of report attachments.
    /// </summary>
    public interface IAttachmentCollection : IKeyedCollection<string, Attachment>
    {
    }
}
