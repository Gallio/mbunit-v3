using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Collections;

namespace MbUnit.Core.Services.Report.Attachments
{
    /// <summary>
    /// A collection of <see cref="Attachment" /> objects.
    /// </summary>
    /// <remarks>
    /// The operations on this class are thread-safe, including enumeration.
    /// </remarks>
    [Serializable]
    public sealed class AttachmentCollection : SyncUnorderedKeyedCollection<string, Attachment>, IAttachmentCollection
    {
        protected override string GetKeyForItem(Attachment item)
        {
            return item.Name;
        }
    }
}
