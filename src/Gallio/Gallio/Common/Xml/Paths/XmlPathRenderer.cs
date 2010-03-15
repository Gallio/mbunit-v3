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
using System.Text;

namespace Gallio.Common.Xml.Paths
{
    /// <summary>
    /// Formats a strict path into a human readable form, based on the contents of an existing XML fragment.
    /// </summary>
    internal sealed class XmlPathRenderer
    {
        private readonly IXmlPathStrict rootPath;
        private readonly NodeFragment fragment;
        private readonly XmlPathRenderingOptions options;
        private readonly List<string> lines = new List<string>();
        private string pendingAttribute;
        private string pendingContent;

        // Rendering constants
        private const string Ellipsis = "…";
        private const int tab = 2;

        /// <summary>
        /// Formats a strict path into a human readable form, based on the contents of an existing XML fragment.
        /// </summary>
        /// <param name="path">The strict path to format.</param>
        /// <param name="fragment">The fragment used to format the path..</param>
        /// <param name="options">Rendering options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/>, or <paramref name="fragment"/> is null.</exception>
        public static string Run(IXmlPathStrict path, NodeFragment fragment, XmlPathRenderingOptions options)
        {
            return new XmlPathRenderer(path, fragment, options).RunImpl();
        }

        private XmlPathRenderer(IXmlPathStrict path, NodeFragment fragment, XmlPathRenderingOptions options)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (fragment == null)
                throw new ArgumentNullException("fragment");

            this.rootPath = path;
            this.fragment = fragment;
            this.options = options;
        }

        private string RunImpl()
        {
            ProcessPathNode(rootPath);

            if ((options & XmlPathRenderingOptions.UseIndentation) != 0)
                return ConsolidateWithIndentation();

            return ConsolidateFlat();
        }

        private string ConsolidateFlat()
        {
            lines.Reverse();
            return String.Concat(lines.ToArray());
        }

        private string ConsolidateWithIndentation()
        {
            var output = new StringBuilder();
            output.AppendLine();

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[lines.Count - i - 1];
                output.AppendLine(new string(' ', i*tab) + line);
            }

            return output.ToString();
        }

        private void ProcessPathNode(IXmlPathStrict path)
        {
            Aggregate(fragment.Find(path));

            if (path.Parent != null && !path.Parent.IsEmpty)
            {
                ProcessPathNode(path.Parent);
            }
        }

        private void Aggregate(INode node)
        {
            switch (node.Type)
            {
                case NodeType.Element:
                    AggregateElement((NodeElement)node);
                    break;

                case NodeType.Comment:
                    AggregateComment((NodeComment)node);
                    break;

                case NodeType.Content:
                    AggregateContent((NodeContent)node);
                    break;

                case NodeType.Attribute:
                    AggregateAttribute((NodeAttribute)node);
                    break;

                case NodeType.Fragment:
                    break;

                case NodeType.Declaration:
                default:
                    throw new InvalidOperationException();
            }
        }

        private void AggregateComment(NodeComment node)
        {
            AddLine(String.Format("<!--{0}-->", node.Text));
        }

        private void AggregateAttribute(NodeAttribute node)
        {
            var output = new StringBuilder(" ");

            if (!node.IsFirst)
                output.Append(Ellipsis + " ");

            output.AppendFormat("{0}='{1}'", node.Name, node.Value);

            if (!node.IsLast)
                output.Append(" " + Ellipsis);

            pendingAttribute = output.ToString();
        }

        private void AggregateContent(NodeContent node)
        {
            pendingContent = node.Text;
        }

        private void AggregateElement(NodeElement node)
        {
            var output = new StringBuilder("<" + node.Name);
            output.Append(pendingAttribute ?? (node.Attributes.Count > 0 ? " " + Ellipsis : String.Empty));

            if (node.Children.Count == 0)
            {
                output.Append("/>");
            }
            else
            {
                output.Append(">");

                if (pendingContent != null)
                {
                    output.Append(pendingContent);
                    output.AppendFormat("</{0}>", node.Name);
                }
            }

            AddLine(output.ToString());
        }

        private void AddLine(string line)
        {
            lines.Add(line);
            ResetPendingEntries();
        }

        private void ResetPendingEntries()
        {
            pendingAttribute = null;
            pendingContent = null;
        }
    }
}
