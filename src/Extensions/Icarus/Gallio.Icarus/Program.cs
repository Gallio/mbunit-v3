// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Services.Interfaces;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Icarus.Services;

namespace Gallio.Icarus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)]
        // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Arguments arguments = ParseArguments(args);

            RuntimeLogController runtimeLogController = new RuntimeLogController();

            IOptionsController optionsController = OptionsController.Instance;

            RuntimeSetup runtimeSetup = new RuntimeSetup();
            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            runtimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(Program).Assembly));
            runtimeSetup.PluginDirectories.AddRange(optionsController.PluginDirectories);
            
            using (RuntimeBootstrap.Initialize(runtimeSetup, runtimeLogController))
            {
                IProjectTreeModel projectTreeModel = new ProjectTreeModel(Paths.DefaultProject, new Project());
                IProjectController projectController = new ProjectController(projectTreeModel);
                
                ITestRunner testRunner = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>().CreateTestRunner(
                    optionsController.TestRunnerFactory);
                ITestRunnerService testRunnerService = new TestRunnerService(testRunner);
                ITestController testController = new TestController(testRunnerService, new TestTreeModel());
                
                IExecutionLogController executionLogController = new ExecutionLogController(testController);

                IReportManager reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
                IReportController reportController = new ReportController(new ReportService(reportManager));

                Main main = new Main(projectController, testController, runtimeLogController, executionLogController, 
                    reportController, arguments);

                testRunnerService.Initialize();
                main.CleanUp += delegate { testRunnerService.Dispose(); };

                Application.Run(main);
            }
        }

        private static Arguments ParseArguments(string[] args)
        {
            // parse command line arguments
            CommandLineArgumentParser argumentParser = new CommandLineArgumentParser(typeof(Arguments));
            Arguments arguments = new Arguments();
            argumentParser.Parse(args, arguments, delegate { });
            return arguments;
        }
    }
}
