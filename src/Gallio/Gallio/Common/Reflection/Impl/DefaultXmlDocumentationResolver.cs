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
using System.IO;
using System.Text;
using System.Xml;

namespace Gallio.Common.Reflection.Impl
{
    /// <summary>
    /// The default XML documentation resolver reads XML documentation files on
    /// demand when available and caches them in memory for subsequent accesses.
    /// It takes care of mapping member names to XML documentation conventions
    /// when asked to resolve the documentation for a member.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All operations are thread-safe.
    /// </para>
    /// </remarks>
    public class DefaultXmlDocumentationResolver : IXmlDocumentationResolver
    {
        private readonly Dictionary<string, CachedDocument> cachedDocuments;

        /// <summary>
        /// Creates an XML documentation loader.
        /// </summary>
        public DefaultXmlDocumentationResolver()
        {
            cachedDocuments = new Dictionary<string, CachedDocument>();
        }

        /// <inheritdoc />
        public string GetXmlDocumentation(string assemblyPath, string memberId)
        {
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");
            if (memberId == null)
                throw new ArgumentNullException("memberId");

            CachedDocument document = GetDocument(assemblyPath);
            return document != null ? document.GetXmlDocumentation(memberId) : null;
        }

        private CachedDocument GetDocument(string assemblyPath)
        {
            assemblyPath = Path.GetFullPath(assemblyPath);

            lock (cachedDocuments)
            {
                CachedDocument document;
                if (cachedDocuments.TryGetValue(assemblyPath, out document))
                    return document;

                if (assemblyPath != null)
                {
                    string documentPath = Path.ChangeExtension(assemblyPath, @".xml");

                    if (File.Exists(documentPath))
                    {
                        document = CachedDocument.Load(documentPath);
                        cachedDocuments.Add(assemblyPath, document);
                        return document;
                    }
                }

                cachedDocuments.Add(assemblyPath, null);
                return null;
            }
        }

        private sealed class CachedDocument
        {
            private readonly Dictionary<string, string> members;

            private CachedDocument()
            {
                members = new Dictionary<string, string>();
            }

            public static CachedDocument Load(string documentPath)
            {
                // This is optimized somewhat to avoid reading in the XML document
                // as a big lump of objects.  We just need the contents; the structure
                // itself is irrelevant.

                CachedDocument document = new CachedDocument();

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false; // needed for whitespace normalization to work within comments
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreComments = true;
                settings.ValidationType = ValidationType.None;
                settings.ProhibitDtd = true;
                settings.CheckCharacters = false;
                settings.CloseInput = true;

                using (XmlReader reader = XmlReader.Create(documentPath, settings))
                {
                    for (; ; )
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == @"member")
                            {
                                string name = reader.GetAttribute(@"name");
                                string content = NormalizeWhitespace(reader.ReadInnerXml());

                                // Note: The member name might not be unique if the XML file
                                //       is malformed.  So we keep the last occurrence only.
                                document.members[name] = content;
                                continue;
                            }
                            else if (reader.Name != @"doc" && reader.Name != @"members")
                            {
                                reader.Skip();
                                continue;
                            }
                        }

                        if (!reader.Read())
                            break;
                    }
                }

                return document;
            }

            public string GetXmlDocumentation(string id)
            {
                string content;
                members.TryGetValue(id, out content);
                return content;
            }

            /// <summary>
            /// The XML documentation gets pretty-printed with indentation by the compiler.
            /// We try to recover the original formatting by stripping out the minimal leading
            /// whitespace from each line.
            /// </summary>
            private static string NormalizeWhitespace(string content)
            {
                int count = content.Length;
                if (count == 0)
                    return string.Empty;

                // Compute the amount of leading whitespace that appears on all non-empty lines.
                int leading = int.MaxValue;
                for (int i = 0, currentLeading = 0; i < count; )
                {
                    char c = content[i++];

                    if (c == ' ')
                    {
                        currentLeading += 1;
                    }
                    else if (c == '\n')
                    {
                        currentLeading = 0;
                    }
                    else
                    {
                        if (currentLeading < leading)
                            leading = currentLeading;

                        currentLeading = 0;

                        while (i < count && content[i++] != '\n') ;
                    }
                }

                // Normalize the content.
                // Strip leading and trailing empty lines and newlines.
                // Remove leading whitespace up to the previously computed limit.
                StringBuilder output = new StringBuilder(count);
                int lastContentPos = 0;
                for (int i = 0, skippedLeading = 0; i < count; )
                {
                    char c = content[i++];

                    if (c == ' ')
                    {
                        skippedLeading += 1;

                        if (skippedLeading > leading)
                            output.Append(' ');
                    }
                    else if (c == '\n')
                    {
                        skippedLeading = 0;

                        if (lastContentPos == 0)
                        {
                            output.Length = 0; // strip leading empty lines
                        }
                        else
                        {
                            output.Append('\n');
                        }
                    }
                    else
                    {
                        skippedLeading = 0;

                        output.Append(c);
                        lastContentPos = output.Length;

                        while (i < count && (c = content[i++]) != '\n')
                        {
                            output.Append(c);
                            if (c != ' ')
                                lastContentPos = output.Length;
                        }

                        output.Length = lastContentPos; // strip trailing whitespace
                        output.Append('\n');
                    }
                }

                output.Length = lastContentPos; // strip trailing empty lines and newlines

                return output.ToString();
            }
        }
    }
}