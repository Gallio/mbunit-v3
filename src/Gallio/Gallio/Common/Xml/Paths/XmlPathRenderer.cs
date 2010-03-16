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
    internal abstract class XmlPathRenderer
    {
        private readonly IXmlPathStrict rootPath;
        private readonly NodeFragment fragment;
        private readonly List<Line> lines = new List<Line>();
        private string pendingAttribute;
        private string pendingContent;

        // Rendering constants
        private const string Ellipsis = "…";
        private const int ContextualAttributes = 2;

        protected struct Line
        {
            public int Level;
            public string Text;
        }

        /// <summary>
        /// Formats a strict path into a human readable form, based on the contents of an existing XML fragment.
        /// </summary>
        /// <param name="path">The strict path to format.</param>
        /// <param name="fragment">The fragment used to format the path..</param>
        /// <param name="options">Rendering options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/>, or <paramref name="fragment"/> is null.</exception>
        public static string Run(IXmlPathStrict path, NodeFragment fragment, XmlPathRenderingOptions options)
        {
            var renderer = ((options & XmlPathRenderingOptions.UseIndentation) != 0)
                ? (XmlPathRenderer)new XmlPathRendererWithIndentation(path, fragment)
                : (XmlPathRenderer)new XmlPathRendererFlat(path, fragment);

            return renderer.RunImpl();
        }

        protected XmlPathRenderer(IXmlPathStrict path, NodeFragment fragment)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (fragment == null)
                throw new ArgumentNullException("fragment");

            this.rootPath = path;
            this.fragment = fragment;
        }

        private string RunImpl()
        {
            ProcessPathNode(rootPath, 0);
            return Consolidate(lines);
        }

        protected abstract string Consolidate(List<Line> lines);

        private void ProcessPathNode(IXmlPathStrict path, int level)
        {
            var node = fragment.Find(path);

            if (node != null)
                Aggregate(node, level);

            if (path.Parent != null && !path.Parent.IsEmpty)
            {
                ProcessPathNode(path.Parent, level + 1);
            }
        }

        private void Aggregate(INode node, int level)
        {
            switch (node.Type)
            {
                case NodeType.Element:
                    AggregateElement((NodeElement)node, level);
                    break;

                case NodeType.Comment:
                    AggregateComment((NodeComment)node, level);
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
                    AggregateDeclaration((NodeDeclaration)node, level);
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private void AggregateComment(NodeComment node, int level)
        {
            AddLine(String.Format("<!--{0}-->", node.Text), level);
        }

        private void AggregateDeclaration(NodeDeclaration node, int level)
        {
            var output = new StringBuilder("<?xml");
            output.Append(pendingAttribute ?? (node.Attributes.Count > 0 ? " " + Ellipsis : String.Empty));
            output.Append("?>");
            AddLine(output.ToString(), level);
        }

        private void AggregateAttribute(NodeAttribute node)
        {
            var output = new StringBuilder(" ");

            if (!node.IsFirst)
                output.Append(Ellipsis + " ");

            output.Append(FormatAttribute(node));

            if (!node.IsLast)
                output.Append(" " + Ellipsis);

            pendingAttribute = output.ToString();
        }

        private static string FormatAttribute(NodeAttribute node)
        {
            return String.Format("{0}='{1}'", node.Name, node.Value);
        }

        private void AggregateContent(NodeContent node)
        {
            var output = new StringBuilder();

            if (!node.IsFirst)
                output.Append(Ellipsis + " ");

            output.Append(node.Text);
            pendingContent = output.ToString();
        }

        private void AggregateElement(NodeElement node, int level)
        {
            var output = new StringBuilder("<" + node.Name);
            output.Append(pendingAttribute ?? AggregateContextualAttributes(node));

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

            AddLine(output.ToString(), level);
            
            if (!node.IsFirst)
               AddLine(Ellipsis, level);
        }

        private static string AggregateContextualAttributes(NodeElement node)
        {
            var output = new StringBuilder();

            if (node.Attributes.Count > 0)
            {
                bool isAlone = node.IsFirst && node.IsLast;

                if (!isAlone)
                {
                    int count = Math.Min(ContextualAttributes, node.Attributes.Count);

                    for (int i = 0; i < count; i++)
                    {
                        output.Append(" " + FormatAttribute(node.Attributes[i]));
                    }
                }

                if (isAlone || node.Attributes.Count > ContextualAttributes)
                    output.Append(" " + Ellipsis);
            }

            return output.ToString();
        }

        private void AddLine(string text, int level)
        {
            lines.Add(new Line
            {
                Level = level,
                Text = text
            });

            ResetPendingEntries();
        }

        private void ResetPendingEntries()
        {
            pendingAttribute = null;
            pendingContent = null;
        }
    }
}
