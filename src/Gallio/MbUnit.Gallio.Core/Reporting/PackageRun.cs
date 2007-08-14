using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Summarizes the execution of a test package for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public class PackageRun
    {
        private List<TestRun> testRuns;
        private PackageRunStatistics statistics;
        private DateTime startTime;
        private DateTime endTime;

        /// <summary>
        /// Gets or sets the time when the package run started.
        /// </summary>
        [XmlAttribute("startTime")]
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Gets or sets the time when the package run ended.
        /// </summary>
        [XmlAttribute("endTime")]
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        /// <summary>
        /// Gets or sets the array of test runs performed as part of the package run.
        /// Never null.
        /// </summary>
        [XmlArray("testRuns", Namespace=SerializationUtils.XmlNamespace, IsNullable=false)]
        [XmlArrayItem("testRun", Namespace=SerializationUtils.XmlNamespace, IsNullable=false)]
        public TestRun[] TestRuns
        {
            get { return testRuns.ToArray(); }
            set { testRuns = new List<TestRun>(value); }
        }

        /// <summary>
        /// Gets or sets the statistics for the package run.
        /// </summary>
        [XmlElement("statistics", Namespace=SerializationUtils.XmlNamespace, IsNullable=true)]
        public PackageRunStatistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }

        /// <summary>
        /// Adds a test run to the list.
        /// </summary>
        /// <param name="run">The test run to add</param>
        public void AddTestRun(TestRun run)
        {
            testRuns.Add(run);
        }

        /// <summary>
        /// Creates an initialized package run with a blank list of test runs and statistics.
        /// </summary>
        /// <param name="startTime">The start time</param>
        /// <returns>The package run</returns>
        public static PackageRun Create(DateTime startTime)
        {
            PackageRun run = new PackageRun();
            run.startTime = startTime;
            run.testRuns = new List<TestRun>();
            run.statistics = new PackageRunStatistics();
            return run;
        }
    }
}
