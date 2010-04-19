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
using System.Collections.Generic;
using Gallio.Common.Collections;
using Gallio.Common.Platform;
using Microsoft.Win32;

namespace Gallio.Common.Markup
{
    /// <summary>
    /// Defines constants for commonly used mime types for attachment contents.
    /// </summary>
    public static class MimeTypes
    {
        private static readonly Dictionary<string, string> extensionsByMimeType;
        private static readonly Dictionary<string, string> mimeTypesByExtension;

        static MimeTypes()
        {
            extensionsByMimeType = new Dictionary<string, string>();
            mimeTypesByExtension = new Dictionary<string, string>();

            RegisterMimeTypeExtensions(PlainText, ".txt");
            RegisterMimeTypeExtensions(Xml, ".xml");
            RegisterMimeTypeExtensions(Html, ".html", ".htm");
            RegisterMimeTypeExtensions(XHtml, ".xhtml");
            RegisterMimeTypeExtensions(MHtml, ".mht", ".mhtml");
            RegisterMimeTypeExtensions(Css, ".css");
            RegisterMimeTypeExtensions(JavaScript, ".js");
            RegisterMimeTypeExtensions(Png, ".png");
            RegisterMimeTypeExtensions(Jpeg, ".jpg", ".jpeg");
            RegisterMimeTypeExtensions(Gif, ".gif");
            RegisterMimeTypeExtensions(FlashVideo, ".flv");
        }

        private static void RegisterMimeTypeExtensions(string mimeType, params string[] extensions)
        {
            if (extensions.Length != 0)
                extensionsByMimeType.Add(mimeType, extensions[0]);

            foreach (string extension in extensions)
                mimeTypesByExtension.Add(extension, mimeType);
        }

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
        /// JPEG image.
        /// </summary>
        public const string Jpeg = "image/jpeg";

        /// <summary>
        /// GIF image.
        /// </summary>
        public const string Gif = "image/gif";

        /// <summary>
        /// MHTML web archive.
        /// </summary>
        public const string MHtml = "multipart/related";

        /// <summary>
        /// Flash video.
        /// </summary>
        public const string FlashVideo = "video/x-flv";

        /// <summary>
        /// Gets all the registered mime types.
        /// </summary>
        public static IEnumerable<string> All
        {
            get
            {
                return extensionsByMimeType.Keys;
            }
        }

        /// <summary>
        /// Guesses the mime type given a well-known extension.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>The mime type, or null if not known.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null.</exception>
        public static string GetMimeTypeByExtension(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            string mimeType;
            mimeTypesByExtension.TryGetValue(extension, out mimeType);

            if (mimeType == null && ! DotNetRuntimeSupport.IsUsingMono)
                mimeType = GetMimeTypeForExtensionFromWindowsRegistry(extension);

            return mimeType;
        }

        /// <summary>
        /// Guesses the extension given a well-known mime-type.
        /// </summary>
        /// <param name="mimeType">The mime type.</param>
        /// <returns>The extension, or null if not known.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mimeType"/> is null.</exception>
        public static string GetExtensionByMimeType(string mimeType)
        {
            if (mimeType == null)
                throw new ArgumentNullException("mimeType");

            string extension;
            extensionsByMimeType.TryGetValue(mimeType, out extension);
            return extension;
        }

        private static string GetMimeTypeForExtensionFromWindowsRegistry(string extension)
        {
            return Registry.GetValue(@"HKEY_CLASSES_ROOT\" + extension, "Content Type", null) as string;
        }
    }
}