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
using Gallio.Model.Execution;

namespace Gallio.Framework
{
    /// <summary>
    /// Represents a binary-encoded attachments.
    /// </summary>
    [Serializable]
    public sealed class BinaryAttachment : Attachment
    {
        private readonly byte[] data;

        /// <summary>
        /// Creates an attachment.
        /// </summary>
        /// <param name="name">The attachment name, not null</param>
        /// <param name="contentType">The content type, not null</param>
        /// <param name="data">The binary data, not null</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contentType"/> or <paramref name="data"/> is null</exception>
        public BinaryAttachment(string name, string contentType, byte[] data)
            : base(name, contentType)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;
        }

        /// <summary>
        /// Gets the binary content of the attachment, not null.
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }

        /// <inheritdoc />
        internal override void Attach(ITestLogWriter logWriter)
        {
            logWriter.AttachBytes(Name, ContentType, data);
        }
    }
}
