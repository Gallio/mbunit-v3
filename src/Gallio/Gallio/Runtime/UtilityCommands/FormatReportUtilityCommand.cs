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
using Gallio.Runner.Reports;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Logging;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to load an XML test report and save
    /// </summary>
    public class FormatReportUtilityCommand : BaseUtilityCommand<FormatReportUtilityCommand.Arguments>
    {
        private UtilityCommandContext context;
        private Arguments arguments;
        private string inputPath;
        private string outputPath;
        private string inputName;
        private IReportManager reportManager;
        private Report report;

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, Arguments arguments)
        {
            this.context = context;
            this.arguments = arguments;
            context.Logger.Log(LogSeverity.Important, "Formatting a test report.");
            return Prepare() && LoadReport() && SaveReport() ? 0 : -1;
        }

        private bool Prepare()
        {
            outputPath = arguments.ReportOutput ?? Environment.CurrentDirectory;
            reportManager = RuntimeAccessor.ServiceLocator.Resolve<IReportManager>();

            try
            {
                inputPath = Path.GetDirectoryName(arguments.ReportPath);
                inputName = Path.GetFileNameWithoutExtension(arguments.ReportPath);
            }
            catch (ArgumentException exception)
            {
                context.Logger.Log(LogSeverity.Error, "The specified report is not a valid file path.", exception);
                return false;
            }

            return true;
        }

        private bool LoadReport()
        {
            return CaptureFileException("The specified report is not a valid file path.", () =>
            {
                using (IReportContainer inputContainer = new FileSystemReportContainer(inputPath, inputName))
                {
                    IReportReader reportReader = reportManager.CreateReportReader(inputContainer);
                    report = context.ProgressMonitorProvider.Run(pm => reportReader.LoadReport(true, pm));
                }
            });
        }

        private bool SaveReport()
        {
            return CaptureFileException("The specified output directory is not a valid file path.", () =>
            {
                var outputName = (arguments.ReportNameFormat != null) ? report.FormatReportName(arguments.ReportNameFormat) : inputName;

                using (IReportContainer outputContainer = new FileSystemReportContainer(outputPath, outputName))
                {
                    IReportWriter reportWriter = reportManager.CreateReportWriter(report, outputContainer);
                    var options = new ReportFormatterOptions();
                    context.ProgressMonitorProvider.Run(pm => reportManager.Format(reportWriter, arguments.ReportType, options, pm));
                }
            });
        }

        private bool CaptureFileException(string errorMessage, Action func)
        {
            try
            {
                func();
                return true;
            }
            catch (FileNotFoundException exception)
            {
                context.Logger.Log(LogSeverity.Error, errorMessage, exception);
                return false;
            }
            catch (DirectoryNotFoundException exception)
            {
                context.Logger.Log(LogSeverity.Error, errorMessage, exception);
                return false;
            }
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
                Description = "The format of the output report name.",
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
            /// The type of the output report.
            /// </summary>
            [CommandLineArgument(CommandLineArgumentFlags.Required,
                Description = "The type of the output report.",
                LongName = "ReportType",
                ShortName = "rt")]
            public string ReportType;
        }
    }
}
