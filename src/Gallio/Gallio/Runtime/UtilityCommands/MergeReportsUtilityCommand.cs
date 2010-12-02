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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.IO;
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to merge several XML test reports.
    /// </summary>
    public class MergeReportsUtilityCommand : BaseReportTransformationUtilityCommand<MergeReportsUtilityCommand.Arguments>
    {
        private Pair<string, string>[] inputFileNames;
        private string outputPath;
        private Report[] inputReports;
        private Report outputReport;
        private ReportArchive reportArchive;

        /// <inheritdoc />
        protected override bool ExecuteImpl()
        {
            Context.Logger.Log(LogSeverity.Important, "Merging test reports.");
            return Prepare()
                && Load()
                && Merge()
                && SaveReport(outputReport, reportArchive, Args.ReportType, outputPath, () =>
                    outputReport.FormatReportName(Args.ReportNameFormat ?? "MergedReport-{0}-{1}"));
        }

        private bool Prepare()
        {
            outputPath = Args.ReportOutput ?? Environment.CurrentDirectory;

            try
            {
                inputFileNames = GenericCollectionUtils.ConvertAllToArray(Args.ReportPaths, 
                    path => new Pair<string, string>(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)));
            }
            catch (ArgumentException exception)
            {
                Context.Logger.Log(LogSeverity.Error, "One or several of the specified reports are not valid file paths.", exception);
                return false;
            }

            return ReportArchive.TryParse(Args.ReportArchive, out reportArchive);
        }

        private bool Load()
        {
            bool succeeded = true;
            inputReports = GenericCollectionUtils.ConvertAllToArray(inputFileNames, x =>
            {
                Report report;
                succeeded &= LoadReport(x.First, x.Second, out report);
                return report;
            });
            return succeeded;
        }

        private bool Merge()
        {
            IReportMerger reportMerger = ReportManager.CreateReportMerger(inputReports);
            outputReport = Context.ProgressMonitorProvider.Run(monitor => reportMerger.Merge(monitor));
            return true;
        }

        /// <summary>
        /// The arguments for the command.
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// The path of the input test reports.
            /// </summary>
            [DefaultCommandLineArgument(CommandLineArgumentFlags.Required | CommandLineArgumentFlags.MultipleUnique,
                Description = "The path of the existing XML test reports.",
                ValueLabel = "ReportPaths")]
            public string[] ReportPaths;

            /// <summary>
            /// The format of the output report name.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "The format of the output report name (optional).",
                LongName = "ReportNameFormat",
                ShortName = "rnf")]
            public string ReportNameFormat;

            /// <summary>
            /// The path of the output report (optiona).
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "The path of the output report (optional).",
                LongName = "ReportOutput",
                ShortName = "ro")]
            public string ReportOutput;

            /// <summary>
            /// Pack the resulting report in a compressed archive (optional).
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "Pack the resulting report in a compressed archive (optional).",
                LongName = "ReportArchive",
                ShortName = "ra")]
            public string ReportArchive;

            /// <summary>
            /// The type of the output report.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce,
                Description = "The type of the output report.",
                LongName = "ReportType",
                ShortName = "rt")]
            public string ReportType;
        }
    }
}
