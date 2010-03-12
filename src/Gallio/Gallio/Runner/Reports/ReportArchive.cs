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
using System.IO;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.IO;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// The type of file archive used to enclose a test report.
    /// </summary>
    public sealed class ReportArchive
    {
        internal enum ReportArchiveValue
        {
            Normal,
            Zip,
        }

        private readonly ReportArchiveValue value;
        private readonly Type reportContainerForSavingType;

        internal ReportArchiveValue Value
        {
            get
            {
                return value;
            }
        }

        /// <summary>
        /// Gets the report container type.
        /// </summary>
        public Type ReportContainerForSavingType
        {
            get
            {
                return reportContainerForSavingType;
            }
        }

        private ReportArchive(ReportArchiveValue value, Type reportContainerForSavingType)
        {
            this.value = value;
            this.reportContainerForSavingType = reportContainerForSavingType;
        }

        /// <summary>
        /// Parses the specified value and returns the corresponding <see cref="ReportArchive"/> value,
        /// or the default value if null or an empty string was specified.
        /// </summary>
        /// <param name="value">The value of the searched archive.</param>
        /// <param name="reportArchive">The resulting report archive item.</param>
        /// <returns>True if the parsing was successful; otherwise false.</returns>
        public static bool TryParse(string value, out ReportArchive reportArchive)
        {
            return ParseImpl(value, out reportArchive, false);
        }

        /// <summary>
        /// Parses the specified value and returns the corresponding <see cref="ReportArchive"/> value,
        /// or the default value if null or an empty string was specified.
        /// </summary>
        /// <param name="value">The value of the searched archive.</param>
        /// <returns>The resulting report archive item.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified value does not correspond
        /// to an known report archive value,</exception>
        public static ReportArchive Parse(string value)
        {
            ReportArchive reportArchive;
            ParseImpl(value, out reportArchive, true);
            return reportArchive;
        }

        private static bool ParseImpl(string value, out ReportArchive reportArchive, bool throwOnFailure)
        {
            if (String.IsNullOrEmpty(value))
            {
                reportArchive = Normal;
                return true;
            }

            foreach (ReportArchive item in All)
            {
                if (item.Value.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    reportArchive = item;
                    return true;
                }
            }

            if (throwOnFailure)
            {
                string[] names = GenericCollectionUtils.ToArray(GenericCollectionUtils.Select(All, x => x.Value.ToString()));
                throw new ArgumentException(String.Format("Invalid report archive mode '{0}'. It must be one of the following values: {1}.", 
                    value, String.Join(", ", names)));
            }

            reportArchive = Normal;
            return false;
        }

        private static IEnumerable<ReportArchive> All
        {
            get
            {
                yield return Normal;
                yield return Zip;
            }
        }

        /// <summary>
        /// Non-compressed file structure.
        /// </summary>
        public static readonly ReportArchive Normal = new ReportArchive(ReportArchiveValue.Normal, typeof(FileSystemReportContainer));

        /// <summary>
        /// Compressed zip archive.
        /// </summary>
        public static readonly ReportArchive Zip = new ReportArchive(ReportArchiveValue.Zip, typeof(ArchiveReportContainer));
    }
}

