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

namespace Gallio.Model.Logging
{
    /// <summary>
    /// A test log writer that falls back to a different test log writer for certain
    /// operations when its primary
    /// test log writer is closed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Not all operations are delegated to the fallback log writer.  Some, such as
    /// embedding attachments and entering or exiting nested scopes, are excluded
    /// due to potential errors that may occur because the fallback test writer may not
    /// have seen the previous operation to add the attachment or enter the scope.
    /// </para>
    /// </remarks>
    public class FallbackTestLogWriter : TestLogWriter
    {
        private readonly TestLogWriter primary;
        private readonly TestLogWriter fallback;

        /// <summary>
        /// Creates a fallback test log writer with the specified provider.
        /// </summary>
        /// <param name="primary">The test log writer to call by default</param>
        /// <param name="fallback">The test log writer to call as a fallback</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="primary"/>
        /// or <paramref name="fallback"/> is null</exception>
        public FallbackTestLogWriter(TestLogWriter primary, TestLogWriter fallback)
        {
            if (primary == null)
                throw new ArgumentNullException("primary");
            if (fallback == null)
                throw new ArgumentNullException("fallback");

            this.primary = primary;
            this.fallback = fallback;
        }

        /// <inheritdoc />
        protected override void AttachImpl(Attachment attachment)
        {
            if (!primary.IsClosed)
                primary.Attach(attachment);
        }

        /// <inheritdoc />
        protected override void StreamWriteImpl(string streamName, string text)
        {
            if (!primary.IsClosed)
                primary.StreamWrite(streamName, text);
            else
                fallback.StreamWrite(streamName, text);
        }

        /// <inheritdoc />
        protected override void StreamEmbedImpl(string streamName, string attachmentName)
        {
            if (!primary.IsClosed)
                primary.StreamEmbed(streamName, attachmentName);
        }

        /// <inheritdoc />
        protected override void StreamBeginSectionImpl(string streamName, string sectionName)
        {
            if (!primary.IsClosed)
                primary.StreamBeginSection(streamName, sectionName);
        }

        /// <inheritdoc />
        protected override void StreamBeginMarkerImpl(string streamName, Marker marker)
        {
            if (!primary.IsClosed)
                primary.StreamBeginMarker(streamName, marker);
        }

        /// <inheritdoc />
        protected override void StreamEndImpl(string streamName)
        {
            if (!primary.IsClosed)
                primary.StreamEnd(streamName);
        }
    }
}
