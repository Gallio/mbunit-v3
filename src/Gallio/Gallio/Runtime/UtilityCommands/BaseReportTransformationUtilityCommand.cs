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

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// A utility command to load an XML test report and save
    /// </summary>
    public abstract class BaseReportTransformationUtilityCommand<TArguments> : BaseUtilityCommand<TArguments>
    {
        private UtilityCommandContext context;
        private TArguments arguments;
        private IReportManager reportManager;

        /// <summary>
        /// Gets the context of the current utility command.
        /// </summary>
        protected UtilityCommandContext Context
        { 
            get { return context; } 
        }

        /// <summary>
        /// Gets the arguments of the command.
        /// </summary>
        protected TArguments Args
        {
            get { return arguments; }
        }

        /// <summary>
        /// Gets the report manager.
        /// </summary>
        protected IReportManager ReportManager
        {
            get { return reportManager; }
        }

        /// <inheritdoc />
        public override int Execute(UtilityCommandContext context, TArguments arguments)
        {
            this.context = context;
            this.arguments = arguments;
            reportManager = RuntimeAccessor.ServiceLocator.Resolve<IReportManager>();
            return ExecuteImpl() ? 0 : -1;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns>True if the execution was successful; otherwise, false.</returns>
        protected abstract bool ExecuteImpl();

        /// <summary>
        /// Loads the specified report.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="inputName"></param>
        /// <param name="report"></param>
        /// <returns></returns>
        protected bool LoadReport(string inputPath, string inputName, out Report report)
        {
            report = CaptureFileException(() => "The specified report is not a valid file path.", () =>
            {
                var factory = new ReportContainerFactory(new FileSystem(), inputPath, inputName);

                using (IReportContainer inputContainer = factory.MakeForReading())
                {
                    IReportReader reportReader = reportManager.CreateReportReader(inputContainer);
                    return Context.ProgressMonitorProvider.Run(pm => reportReader.LoadReport(true, pm));
                }
            });

            return (report != null);
        }

        /// <summary>
        /// Saves the specified report.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="reportArchive"></param>
        /// <param name="formatterName"></param>
        /// <param name="outputPath"></param>
        /// <param name="getOutputName"></param>
        /// <returns></returns>
        protected bool SaveReport(Report report, ReportArchive reportArchive, string formatterName, string outputPath, Func<string> getOutputName)
        {
            return CaptureFileException(() => "The specified output directory is not a valid file path.", () =>
            {
                var factory = new ReportContainerFactory(new FileSystem(), outputPath, getOutputName());

                using (IReportContainer outputContainer = factory.MakeForSaving(reportArchive))
                {
                    IReportWriter reportWriter = reportManager.CreateReportWriter(report, outputContainer);
                    var options = new ReportFormatterOptions();
                    Context.ProgressMonitorProvider.Run(pm => reportManager.Format(reportWriter, formatterName, options, pm));
                    return true;
                }
            });
        }

        /// <summary>
        /// Try-function which executes the specified action and catch any file-related exception.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the function.</typeparam>
        /// <param name="getErrorMessage">The error message to log.</param>
        /// <param name="func"></param>
        /// <returns></returns>
        protected T CaptureFileException<T>(Func<string> getErrorMessage, Func<T> func)
        {
            try
            {
                return func();
            }
            catch (FileNotFoundException exception)
            {
                context.Logger.Log(LogSeverity.Error, getErrorMessage(), exception);
                return default(T);
            }
            catch (DirectoryNotFoundException exception)
            {
                context.Logger.Log(LogSeverity.Error, getErrorMessage(), exception);
                return default(T);
            }
        }
    }
}
