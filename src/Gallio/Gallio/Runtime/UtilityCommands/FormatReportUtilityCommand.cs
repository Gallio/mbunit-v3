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
using Gallio.Common.IO;
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;
using Gallio.Common.Collections;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to load an XML test report and save
    /// </summary>
    public class FormatReportUtilityCommand : BaseReportTransformationUtilityCommand<FormatReportUtilityCommand.Arguments>
    {
        private string inputPath;
        private string outputPath;
        private string inputName;
        private Report report;
        private ReportArchive reportArchive;

        /// <inheritdoc />
        protected override bool ExecuteImpl()
        {
            Context.Logger.Log(LogSeverity.Important, "Formatting a test report.");
            return Prepare()
                && LoadReport(inputPath, inputName, out report)
                && SaveReport(report, reportArchive, Args.ReportType, outputPath, () => 
                    (Args.ReportNameFormat != null) ? report.FormatReportName(Args.ReportNameFormat) : inputName, ParseOptions(Args.ReportFormatterProperties));
        }

        private bool Prepare()
        {
            outputPath = Args.ReportOutput ?? Environment.CurrentDirectory;

            try
            {
                inputPath = Path.GetDirectoryName(Args.ReportPath);
                inputName = Path.GetFileNameWithoutExtension(Args.ReportPath);
            }
            catch (ArgumentException exception)
            {
                Context.Logger.Log(LogSeverity.Error, "The specified report is not a valid file path.", exception);
                return false;
            }

            return ReportArchive.TryParse(Args.ReportArchive, out reportArchive);
        }

        /// <summary>
        /// The arguments for the command.
        /// </summary>
        public class Arguments
        {
            /// <summary>
            /// The format of the output report name.
            /// </summary>
            [DefaultCommandLineArgument(CommandLineArgumentFlags.Required,
                Description = "The path of the existing XML test report.",
                ValueLabel = "ReportPath")]
            public string ReportPath;

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
            [CommandLineArgument(CommandLineArgumentFlags.Required,
                Description = "The type of the output report.",
                LongName = "ReportType",
                ShortName = "rt")]
            public string ReportType;

            /// <summary>
            /// Key/Value property for the report formatter.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.Multiple,
                 Description = "Specifies a property key/value for the report formatters.  eg. \"AttachmentContentDisposition=Absent\"",
                 LongName = "ReportFormatterProperty",
                 ShortName = "rfp",
                 ValueLabel = "key=value")]
            public string[] ReportFormatterProperties = EmptyArray<string>.Instance;
        }
    }
}
