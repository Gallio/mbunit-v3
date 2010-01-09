// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Normalization;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// Utilities for normalizing markup contents.
    /// </summary>
    public static class MarkupNormalizationUtils
    {
        /// <summary>
        /// Normalizes a content type.
        /// </summary>
        /// <param name="contentType">The content type, or null if none.</param>
        /// <returns>The normalized content type, or null if none.  May be the same instance if <paramref name="contentType"/>
        /// was already normalized.</returns>
        public static string NormalizeContentType(string contentType)
        {
            return NormalizationUtils.NormalizePrintableASCII(contentType);
        }

        /// <summary>
        /// Normalizes an attachment name.
        /// </summary>
        /// <param name="attachmentName">The attachment name, or null if none.</param>
        /// <returns>The normalized attachment name, or null if none.  May be the same instance if <paramref name="attachmentName"/>
        /// was already normalized.</returns>
        public static string NormalizeAttachmentName(string attachmentName)
        {
            return NormalizationUtils.NormalizeName(attachmentName);
        }

        /// <summary>
        /// Normalizes a stream name.
        /// </summary>
        /// <param name="streamName">The stream name, or null if none.</param>
        /// <returns>The normalized stream name, or null if none.  May be the same instance if <paramref name="streamName"/>
        /// was already normalized.</returns>
        public static string NormalizeStreamName(string streamName)
        {
            return NormalizationUtils.NormalizeName(streamName);
        }

        /// <summary>
        /// Normalizes a section name.
        /// </summary>
        /// <param name="sectionName">The section name, or null if none.</param>
        /// <returns>The normalized section name, or null if none.  May be the same instance if <paramref name="sectionName"/>
        /// was already normalized.</returns>
        public static string NormalizeSectionName(string sectionName)
        {
            return NormalizationUtils.NormalizeName(sectionName);
        }

        /// <summary>
        /// Normalizes markup text.
        /// </summary>
        /// <param name="text">The text, or null if none.</param>
        /// <returns>The normalized text, or null if none.  May be the same instance if <paramref name="text"/>
        /// was already normalized.</returns>
        public static string NormalizeText(string text)
        {
            return NormalizationUtils.NormalizeXmlText(text);
        }
    }
}