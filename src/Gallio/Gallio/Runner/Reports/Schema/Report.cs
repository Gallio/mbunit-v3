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
using System.Globalization;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Xml;
using Gallio.Model.Schema;

namespace Gallio.Runner.Reports.Schema
{
    /// <summary>
    /// A report includes a description of the test package, the model objects,
    /// the combined results of all test runs and summary statistics.
    /// </summary>
    [Serializable]
    [XmlRoot("report", Namespace=SchemaConstants.XmlNamespace)]
    [XmlType(Namespace=SchemaConstants.XmlNamespace)]
    public sealed class Report
    {
        private readonly List<LogEntry> logEntries;

        /// <summary>
        /// Creates an empty report.
        /// </summary>
        public Report()
        {
            logEntries = new List<LogEntry>();
        }

        /// <summary>
        /// Gets or sets the test package configuration, or null if none.
        /// </summary>
        [XmlElement("testPackage", IsNullable = false, Namespace = SchemaConstants.XmlNamespace)]
        public TestPackageData TestPackage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the test model, or null if none.
        /// </summary>
        [XmlElement("testModel", IsNullable = false, Namespace = SchemaConstants.XmlNamespace)]
        public TestModelData TestModel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the test package run results, or null if none.
        /// </summary>
        [XmlElement("testPackageRun", IsNullable = false, Namespace = SchemaConstants.XmlNamespace)]
        public TestPackageRun TestPackageRun
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a mutable list of log entries.
        /// </summary>
        [XmlArray("logEntries", IsNullable = false, Namespace = SchemaConstants.XmlNamespace)]
        [XmlArrayItem("logEntry", typeof(LogEntry), IsNullable = false, Namespace = SchemaConstants.XmlNamespace)]
        public List<LogEntry> LogEntries
        {
            get { return logEntries; }
        }

        /// <summary>
        /// Adds a log entry to the report.
        /// </summary>
        /// <param name="logEntry">The log entry to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logEntry"/> is null.</exception>
        public void AddLogEntry(LogEntry logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException("logEntry");

            logEntries.Add(logEntry);
        }

        /// <summary>
        /// Formats the specified name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The following tags are processed:
        /// <list type="bullet">
        /// <item><c>{0}</c> is replaced by the start date of the test run, or by the current date if not applicable ("yyyyMMdd").</item>
        /// <item><c>{1}</c> is replaced by the start time of the test run, or by the current time if not applicable ("HHmmss").</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="reportNameFormat">The report name format.</param>
        /// <returns>The resulting formatted report name.</returns>
        public string FormatReportName(string reportNameFormat)
        {
            DateTime reportTime = TestPackageRun != null ? TestPackageRun.StartTime : DateTime.Now;
            return String.Format(CultureInfo.InvariantCulture, reportNameFormat,
                reportTime.ToString(@"yyyyMMdd"),
                reportTime.ToString(@"HHmmss"));
        }
    }
}