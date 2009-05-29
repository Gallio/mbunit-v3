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
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Gallio.Runner.Caching;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.ReSharperRunner.Provider
{
    internal static class SessionCache
    {
        private static readonly TemporaryDiskCache diskCache = new TemporaryDiskCache();
        private const string SerializedReportFileName = "Report.data";
        private const string HtmlReportFileName = "Report.html";
        private const string ReportBaseName = "Report";

        public static IDiskCacheGroup GetReportCacheGroup(string sessionId)
        {
            return diskCache.Groups["Gallio.ReSharperRunner.Report:Session-" + sessionId];
        }

        public static FileInfo GetHtmlFormattedReport(string sessionId, bool condensed)
        {
            IDiskCacheGroup group = GetReportCacheGroup(sessionId);
            string directory = condensed ? "Condensed" : "Full";

            FileInfo htmlReportFile = group.GetFileInfo(Path.Combine(directory, HtmlReportFileName));
            if (!htmlReportFile.Exists)
            {
                Report report = LoadSerializedReport(sessionId);
                if (report == null)
                    return null;

                group.CreateSubdirectory(directory);
                IReportManager reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
                FileSystemReportContainer reportContainer = new FileSystemReportContainer(htmlReportFile.DirectoryName, ReportBaseName);
                IReportWriter reportWriter = reportManager.CreateReportWriter(report, reportContainer);
                var reportFormatterOptions = new ReportFormatterOptions();
                reportManager.Format(reportWriter, condensed ? "Html-Condensed" : "Html", reportFormatterOptions, NullProgressMonitor.CreateInstance());
            }

            return htmlReportFile;
        }

        public static bool HasSerializedReport(string sessionId)
        {
            IDiskCacheGroup group = GetReportCacheGroup(sessionId);
            return group.Exists && group.GetFileInfo(SerializedReportFileName).Exists;
        }

        public static Report LoadSerializedReport(string sessionId)
        {
            try
            {
                if (HasSerializedReport(sessionId))
                {
                    IDiskCacheGroup group = GetReportCacheGroup(sessionId);
                    BinaryFormatter formatter = CreateBinaryFormatter();
                    using (Stream stream = group.OpenFile(SerializedReportFileName, FileMode.Open, FileAccess.Read, FileShare.Delete))
                        return (Report)formatter.Deserialize(stream);
                }
            }
            catch (DiskCacheException)
            {
            }

            return null;
        }

        public static void SaveSerializedReport(string sessionId, Report report)
        {
            IDiskCacheGroup group = GetReportCacheGroup(sessionId);
            group.Delete();

            BinaryFormatter formatter = CreateBinaryFormatter();
            using (Stream stream = group.OpenFile(SerializedReportFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Delete))
                formatter.Serialize(stream, report);
        }

        public static void ClearSerializedReport(string sessionId)
        {
            IDiskCacheGroup group = GetReportCacheGroup(sessionId);
            group.Delete();
        }

        private static BinaryFormatter CreateBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
            formatter.FilterLevel = TypeFilterLevel.Full;
            return formatter;
        }
    }
}
