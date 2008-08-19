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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model.Logging;

namespace Gallio.Model.Logging
{
    /// <summary>
    /// <para>
    /// A structured log object is a stored and serializable form of a test log.
    /// </para>
    /// <para>
    /// Values of this type are immutable.  Use a <see cref="StructuredDocumentWriter" />
    /// to construct them.
    /// </para>
    /// </summary>
    [Serializable]
    public class StructuredDocument
    {
        private readonly Dictionary<string, StructuredText> streams;
        private readonly Dictionary<string, Attachment> attachments;

        /// <summary>
        /// Creates a structured document.
        /// </summary>
        /// <param name="streams">The dictionary of streams, indexed by name</param>
        /// <param name="attachments">The dictionary of attachments, indexed by name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streams"/>
        /// or <paramref name="attachments"/> is null</exception>
        public StructuredDocument(IDictionary<string, StructuredText> streams,
            IDictionary<string, Attachment> attachments)
        {
            if (streams == null)
                throw new ArgumentNullException("streams");
            if (attachments == null)
                throw new ArgumentNullException("attachments");

            this.streams = new Dictionary<string, StructuredText>(streams);
            this.attachments = new Dictionary<string, Attachment>(attachments);
        }

        /// <summary>
        /// Gets the dictionary of streams in the document, indexed by name.
        /// </summary>
        public IDictionary<string, StructuredText> Streams
        {
            get { return new ReadOnlyDictionary<string, StructuredText>(streams); }
        }

        /// <summary>
        /// Gets the dictionary of attachments in the document, indexed by name.
        /// </summary>
        public IDictionary<string, Attachment> Attachments
        {
            get { return new ReadOnlyDictionary<string, Attachment>(attachments); }
        }
    }
}
