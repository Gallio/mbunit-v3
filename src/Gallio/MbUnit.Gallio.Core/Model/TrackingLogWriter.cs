// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Collections.Generic;
using MbUnit.Framework.Logging;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// A subclass of <see cref="LogWriter" /> that tracks whether an <see cref="Attachment" />
    /// exists for a given name.
    /// </summary>
    public abstract class TrackingLogWriter : LogWriter
    {
        private bool isClosed;
        private Dictionary<string, int> attachmentHashCodes;

        /// <summary>
        /// Returns true if the log writer has been closed.
        /// </summary>
        protected bool IsClosed
        {
            get { return isClosed; }
        }

        /// <summary>
        /// Adds the attachment to the internal bookkeeping table if there
        /// is not already an attachment with the same name.
        /// </summary>
        /// <param name="attachment">The attachment to add</param>
        /// <returns>True if the attachment was added, false if the same attachment
        /// had been previously added</returns>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment
        /// with the same name was previously added</exception>
        protected bool TrackAttachment(Attachment attachment)
        {
            string attachmentName = attachment.Name;
            int newAttachmentHashCode = attachment.GetHashCode();

            if (attachmentHashCodes != null)
            {
                int existingAttachmentHashCode;
                if (attachmentHashCodes.TryGetValue(attachmentName, out existingAttachmentHashCode))
                {
                    // Use the attachment's hashcode to detect likely attempts to attach
                    // different attachments using the same name without actually having to
                    // maintain a reference to the attachment itself (which could be rather large).
                    if (newAttachmentHashCode == existingAttachmentHashCode)
                        return false;

                    throw new InvalidOperationException(String.Format("The log already contains a different attachment with name '{0}'.",
                        attachmentName));
                }
            }
            else
            {
                attachmentHashCodes = new Dictionary<string, int>();
            }

            attachmentHashCodes.Add(attachmentName, newAttachmentHashCode);
            return true;
        }

        /// <summary>
        /// Verifies that an attachment with the specified name was added.
        /// </summary>
        /// <param name="attachmentName">The attachment name</param>
        /// <exception cref="InvalidOperationException">Thrown if no such attachment exists</exception>
        protected void VerifyAttachmentExists(string attachmentName)
        {
            if (attachmentHashCodes != null && attachmentHashCodes.ContainsKey(attachmentName))
                return;

            throw new InvalidOperationException(String.Format("The log does not contain an existing attachment with name '{0}'.",
                attachmentName));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            isClosed = true;
            attachmentHashCodes = null;
        }

        /// <summary>
        /// Throws an exception if <see cref="IsClosed" /> is true.
        /// </summary>
        protected void ThrowIfClosed()
        {
            if (isClosed)
                throw new InvalidOperationException("Cannot perform this operation because the log writer has been closed.");
        }
    }
}