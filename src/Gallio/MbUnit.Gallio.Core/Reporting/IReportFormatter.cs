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
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// A report formatter provides a strategy for formatting reports for human consumption.
    /// </summary>
    public interface IReportFormatter
    {
        /// <summary>
        /// Gets the unique name of the formatter.  The name is used by the user to select
        /// a formatter case-insensitively.  eg. "XML", "HTML", "MHTML".
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the preferred extension used by the formatter to save its reports.
        /// </summary>
        string PreferredExtension { get; }

        /// <summary>
        /// Formats a report and saves it to a file.
        /// Overwrites the file and replaces associated resources if they exist.
        /// </summary>
        /// <remarks>
        /// <para>
        /// By convention the <paramref name="outputFilename"/> always refers to the
        /// primary file written by the report formatter including its extension.
        /// Auxiliary resources may be written to the same directory in files with the same
        /// base name as <paramref name="outputFilename"/> but with different extensions.
        /// If there are many auxiliary resources (such as linked images and execution log attachments)
        /// then they should be created in a directory with the same base name as
        /// <paramref name="outputFilename"/>.
        /// </para>
        /// <para>
        /// Example layout:
        /// <list type="bullet">
        /// <item>Reports\IntegrationTests.html (the value of <paramref name="outputFilename"/></item>
        /// <item>Reports\IntegrationTests.css (an associated file, could also be put in the IntegrationTests folder)</item>
        /// <item>Reports\IntegrationTests\TestIcon.png (an image used in the report)</item>
        /// <item>Reports\IntegrationTests\FixtureIcon.png (another image used in the report)</item>
        /// <item>Reports\IntegrationTests\Screenshot.jpg (an execution log attachment)</item>
        /// <item>Reports\IntegrationTests\AttachedLog.txt (another execution log attachment)</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="report">The report to format</param>
        /// <param name="outputFilename">The name of the file to write including its extension</param>
        /// <param name="options">Custom options for the report formatter</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <returns>The complete list of files that were written</returns>
        IList<string> Format(Report report, string outputFilename, Dictionary<string, string> options,
            IProgressMonitor progressMonitor);
    }
}
