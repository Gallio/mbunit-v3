// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using MbUnit.Framework.Services.Reports;

namespace MbUnit.Framework.Services.Reports
{
    /// <summary>
    /// Primitive interface for writing reports.  Client code will typically
    /// write to a report using the higher-level <see cref="IReport" /> interface
    /// through the <see cref="IReportService" />.
    /// </summary>
    public interface IReportWriter
    {
        /// <summary>
        /// Writes a text string to the report.
        /// </summary>
        /// <param name="streamName">The name of the report stream</param>
        /// <param name="text">The text to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="text"/> is null</exception>
        void WriteText(string streamName, string text);

        /// <summary>
        /// Writes an attachment to the report.
        /// </summary>
        /// <remarks>
        /// The implementation should allow the same attachment instance to be attached
        /// multiple times and optimized this case by representing embedded attachments
        /// as links.
        /// </remarks>
        /// <param name="streamName">The name of the report stream in which to embed
        /// the attachment, or null to attach it to the top level of the report</param>
        /// <param name="attachment">The attachment to write</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="attachment"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a different attachment instance
        /// with the same name was already written</exception>
        void WriteAttachment(string streamName, Attachment attachment);

        /// <summary>
        /// Begins a report section.
        /// </summary>
        /// <param name="streamName">The name of the report stream</param>
        /// <param name="sectionName">The name of the section to begin</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> or
        /// <paramref name="sectionName"/> is null</exception>
        void BeginSection(string streamName, string sectionName);

        /// <summary>
        /// Ends the current report section.
        /// </summary>
        /// <param name="streamName">The name of the report stream</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="streamName"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if there is no current report section</exception>
        void EndSection(string streamName);

        /// <summary>
        /// Closes the report.
        /// </summary>
        void Close();
    }
}
