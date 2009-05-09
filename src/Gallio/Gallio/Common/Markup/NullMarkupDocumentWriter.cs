// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// A markup document writer that does nothing.
    /// </summary>
    public class NullMarkupDocumentWriter : MarkupDocumentWriter
    {
        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
        }

        /// <inheritdoc />
        protected override void StreamWriteImpl(string streamName, string text)
        {
        }

        /// <inheritdoc />
        protected override void StreamEmbedImpl(string streamName, string attachmentName)
        {
        }

        /// <inheritdoc />
        protected override void StreamBeginSectionImpl(string streamName, string sectionName)
        {
        }

        /// <inheritdoc />
        protected override void StreamBeginMarkerImpl(string streamName, Marker marker)
        {
        }

        /// <inheritdoc />
        protected override void StreamEndImpl(string streamName)
        {
        }
    }
}
