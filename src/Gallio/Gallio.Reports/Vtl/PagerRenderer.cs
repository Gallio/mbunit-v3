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
using System.IO;
using System.Xml;
using Gallio.Common;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Render an HTML pager control.
    /// </summary>
    public class PagerRenderer
    {
        private readonly int pageIndex;
        private readonly int pageCount;
        private readonly int visiblePages;
        private readonly Func<int, string> linkMaker;
        private XmlTextWriter writer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pageIndex">The index of the current page.</param>
        /// <param name="pageCount">The number of pages.</param>
        /// <param name="visiblePages">The number of visible pages shown near the current page.</param>
        /// <param name="linkMaker">A function that creates a link reference to a particular page.</param>
        public PagerRenderer(int pageIndex, int pageCount, int visiblePages, Func<int, string> linkMaker)
        {
            this.pageIndex = pageIndex;
            this.pageCount = pageCount;
            this.visiblePages = visiblePages;
            this.linkMaker = linkMaker;
        }

        /// <summary>
        /// Renders the control.
        /// </summary>
        /// <returns>HTML pager control.</returns>
        public string Run()
        { 
            if (pageCount <= 1)
                return String.Empty;

            using (var text = new StringWriter())
            {
                using (writer = new XmlTextWriter(text))
                {
                    writer.WriteStartElement("ul");
                    writer.WriteAttributeString("class", "pager");
                    GeneratePreviousPageLink();
                    GenerateSummaryPageLink();
                    int start = 1;
                    int end = pageCount;

                    if (pageCount > visiblePages)
                    {
                        int middle = (int)Math.Ceiling(visiblePages / 2d) - 1;
                        int below = (pageIndex - middle);
                        int above = (pageIndex + middle);

                        if (below < 4)
                        {
                            above = visiblePages;
                            below = 1;
                        }
                        else if (above > (pageCount - 4))
                        {
                            above = pageCount;
                            below = (pageCount - visiblePages);
                        }

                        start = below;
                        end = above;
                    }

                    if (start > 3)
                    {
                        GeneratePageLink("1", 1);
                        GeneratePageLink("2", 2);
                        GenerateRawText("...");
                    }

                    for (int i = start; i <= end; i++)
                    {
                        if (i == pageIndex)
                        {
                            GenerateCurrentPage(i.ToString());
                        }
                        else
                        {
                            GeneratePageLink(i.ToString(), i);
                        }
                    }

                    if (end < (pageCount - 3))
                    {
                        GenerateRawText("...");
                        GeneratePageLink((pageCount - 1).ToString(), pageCount - 1);
                        GeneratePageLink(pageCount.ToString(), pageCount);
                    }

                    GenerateNextPageLink();
                    writer.WriteEndElement();
                }

                return text.ToString();
            }
        }

        private void GeneratePreviousPageLink()
        {
            if (pageIndex > 0)
                GeneratePageLink("<", pageIndex - 1);
            else
                GenerateDisabledLink("<");
        }

        private void GenerateSummaryPageLink()
        {
            if (pageIndex != 0)
                GeneratePageLink("Summary", 0);
            else
                GenerateCurrentPage("Summary");
        }

        private void GenerateNextPageLink()
        {
            if (pageIndex < pageCount)
                GeneratePageLink(">", pageIndex + 1);
            else
                GenerateDisabledLink(">");
        }

        private void GeneratePageLink(string text, int index)
        {
            writer.WriteStartElement("li");
            writer.WriteStartElement("a");
            writer.WriteAttributeString("href", linkMaker(index));
            writer.WriteString(text);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void GenerateDisabledLink(string text)
        {
            writer.WriteStartElement("li");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "disabled");
            writer.WriteString(text);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void GenerateRawText(string text)
        {
            writer.WriteStartElement("li");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "raw");
            writer.WriteString(text);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void GenerateCurrentPage(string text)
        {
            writer.WriteStartElement("li");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "current");
            writer.WriteString(text);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
