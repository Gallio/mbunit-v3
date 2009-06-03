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

namespace Gallio.Common.Markup
{
    /// <summary>
    /// Defines constants for commonly used mime types for attachment contents.
    /// </summary>
    public static class MimeTypes
    {
        /// <summary>
        /// Binary data.
        /// </summary>
        public const string Binary = "application/octet-stream";

        /// <summary>
        /// Plain text data.
        /// </summary>
        public const string PlainText = "text/plain";

        /// <summary>
        /// Xml data.
        /// </summary>
        public const string Xml = "text/xml";
        
        /// <summary>
        /// HTML.
        /// </summary>
        public const string Html = "text/html";

        /// <summary>
        /// Well-formed XHTML.
        /// </summary>
        public const string XHtml = "text/xhtml+xml";

        /// <summary>
        /// Cascading Style Sheet.
        /// </summary>
        public const string Css = "text/css";

        /// <summary>
        /// JavaScript
        /// </summary>
        public const string JavaScript = "text/javascript";

        /// <summary>
        /// PNG image.
        /// </summary>
        public const string Png = "image/png";

        /// <summary>
        /// GIF image.
        /// </summary>
        public const string Gif = "image/gif";

        /// <summary>
        /// MHTML web archive.
        /// </summary>
        public const string MHtml = "multipart/related";

        /// <summary>
        /// Guesses the mime type given a well-known extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The mime type, or null if not known</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null.</exception>
        public static string GetMimeTypeByExtension(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            switch (extension)
            {
                case ".txt":
                    return PlainText;

                case ".xml":
                    return Xml;

                case ".html":
                case ".htm":
                    return Html;

                case ".xhtml":
                    return XHtml;

                case ".mhtml":
                case ".mht":
                    return MHtml;

                case ".css":
                    return Css;

                case ".js":
                    return JavaScript;

                case ".png":
                    return Png;

                case ".gif":
                    return Gif;

                default:
                    return null;
            }
        }

    }
}