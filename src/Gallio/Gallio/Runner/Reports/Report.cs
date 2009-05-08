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
using System.Xml.Serialization;
using Gallio.Common.Xml;
using Gallio.Model.Serialization;
using Gallio.Model;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// A report includes a description of the test package, the model objects,
    /// the combined results of all test runs and summary statistics.
    /// </summary>
    [Serializable]
    [XmlRoot("report", Namespace=XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class Report
    {
        private TestPackageConfig testPackageConfig;
        private TestModelData testModel;
        private TestPackageRun testPackageRun;
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
        [XmlElement("testPackageConfig", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
            set { testPackageConfig = value; }
        }

        /// <summary>
        /// Gets or sets the test model, or null if none.
        /// </summary>
        [XmlElement("testModel", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public TestModelData TestModel
        {
            get { return testModel; }
            set { testModel = value; }
        }

        /// <summary>
        /// Gets or sets the test package run results, or null if none.
        /// </summary>
        [XmlElement("testPackageRun", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public TestPackageRun TestPackageRun
        {
            get { return testPackageRun; }
            set { testPackageRun = value; }
        }

        /// <summary>
        /// Gets a mutable list of log entries.
        /// </summary>
        [XmlArray("logEntries", IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        [XmlArrayItem("logEntry", typeof(LogEntry), IsNullable = false, Namespace = XmlSerializationUtils.GallioNamespace)]
        public List<LogEntry> LogEntries
        {
            get { return logEntries; }
        }

        /// <summary>
        /// Adds a log entry to the report.
        /// </summary>
        /// <param name="logEntry">The log entry to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logEntry"/> is null</exception>
        public void AddLogEntry(LogEntry logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException("logEntry");

            logEntries.Add(logEntry);
        }
    }
}
