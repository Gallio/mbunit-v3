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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// <para>
    /// A report container is used to load or save the contents of a report.  A report container
    /// consists of zero or more logical report files distinguished by extension and a content
    /// folder.  The content folder holds attachment contents and format-specific resources.
    /// </para>
    /// <para>
    /// The definition of a report container is necessarily somewhat abstract.  It is intended
    /// to allow the files associated with report rendered in multiple formats to coexist
    /// side by side unambiguously and without duplication of common attachment contents.
    /// </para>
    /// <para>
    /// Paths within reports are always specified relative to the root of the container
    /// with each path segment delimited by backslashes.  The first path segment must be
    /// <see cref="ReportName" />, optionally with an extra period-delimited extension.
    /// For example, if <see cref="ReportName" /> is <c>"Report"</c> then <c>"Report.xml"</c> and
    /// <c>"Report/Contents.txt"</c> are valid paths but <c>"Bar"</c> and
    /// <c>"Report-NotAnExtension"</c> are not.
    /// </para>
    /// <para>
    /// By convention report content such as attachments and images are 
    /// stored in a folder with the same name as <see cref="ReportName" />.
    /// </para>
    /// </summary>
    /// <example>
    /// <para>
    /// This example demonstrates the structure of report files with the report name of
    /// <c>"MyReport"</c>.
    /// <list type="bullet">
    /// <item><c>"MyReport.html"</c>: A report file.</item>
    /// <item><c>"MyReport.txt"</c>: Another report file in a different format.</item>
    /// <item><c>"MyReport\"</c>: The standard report content directory by convention.</item>
    /// <item><c>"MyReport\Step123\AttachedLog.txt"</c>: An execution log attachment.</item>
    /// <item><c>"MyReport\img\TestIcon.png"</c>: An image used in the report.</item>
    /// <item><c>"MyReport\img\FixtureIcon.png"</c>: Another image used in the report.</item>
    /// <item><c>"MyReport\img\Screenshot.jpg"</c>: An execution log attachment.</item>
    /// </list>
    /// </para>
    /// </example>
    public interface IReportContainer
    {
        /// <summary>
        /// <para>
        /// Gets the base name of the report.
        /// </para>
        /// <para>
        /// Logical files and folders associated with the report all begin with this
        /// name and are disambiguated by extension.
        /// </para>
        /// </summary>
        string ReportName { get; }

        /// <summary>
        /// Deletes the entire contents of the report in all of its formats.
        /// </summary>
        /// <exception cref="IOException">Thrown if an I/O error occurs</exception>
        void DeleteReport();

        /// <summary>
        /// Opens a report file for reading.
        /// </summary>
        /// <param name="path">The relative path of the report file within the container</param>
        /// <returns>The stream</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        /// <exception cref="IOException">Thrown if an I/O error occurs</exception>
        Stream OpenRead(string path);

        /// <summary>
        /// <para>
        /// Opens a report file for writing, overwriting any previous file with the same name.
        /// </para>
        /// <para>
        /// It is not necessary to create "directories" within the container.  They are
        /// automatically created when new files are opened for writing within them.
        /// </para>
        /// </summary>
        /// <param name="path">The path of the report file</param>
        /// <param name="contentType">The content type of the file, or null if not specified</param>
        /// <param name="encoding">The text encoding of the file, or null if not specified or if the file is binary</param>
        /// <returns>The stream</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        /// <exception cref="IOException">Thrown if an I/O error occurs</exception>
        Stream OpenWrite(string path, string contentType, Encoding encoding);

        /// <summary>
        /// Replaces invalid characters in a file or directory name with underscores
        /// and trims it if it is too long.
        /// </summary>
        /// <param name="fileName">The file or directory name</param>
        /// <returns>The encoded file or directory name</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> is null</exception>
        string EncodeFileName(string fileName);
    }
}
