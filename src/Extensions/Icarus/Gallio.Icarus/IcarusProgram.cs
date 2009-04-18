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
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Mediator.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Properties;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Icarus.Services;
using Gallio.Utilities;

namespace Gallio.Icarus
{
    /// <summary>
    /// The Icarus program.
    /// </summary>
    public class IcarusProgram : ConsoleProgram<IcarusArguments>
    {
        private ITestController testController;

        /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        private IcarusProgram()
        {
            ApplicationName = Resources.ApplicationName;
        }

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return 1;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var runtimeLogController = new RuntimeLogController();

            var runtimeSetup = new RuntimeSetup
            {
                RuntimePath =
                    Path.GetDirectoryName(
                    AssemblyUtils.GetFriendlyAssemblyLocation(typeof (IcarusProgram).Assembly))
            };

            var optionsController = new OptionsController(new FileSystem(), new DefaultXmlSerializer(),
                new Utilities.UnhandledExceptionPolicy());

            // create & initialize a test runner whenever the test runner factory is changed
            optionsController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "TestRunnerFactory")
                    return;

                ConfigureTestRunnerFactory(optionsController.TestRunnerFactory);
            };
            optionsController.Load();

            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            runtimeSetup.PluginDirectories.AddRange(optionsController.PluginDirectories);

            using (RuntimeBootstrap.Initialize(runtimeSetup, runtimeLogController))
            {
                testController = new TestController(new TestTreeModel());

                ConfigureTestRunnerFactory(optionsController.TestRunnerFactory);

                var reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();

                IMediator mediator = new Mediator.Mediator
                {
                    ProjectController = new ProjectController(new ProjectTreeModel(Paths.DefaultProject, new Project()),
                        optionsController, new FileSystem(), new DefaultXmlSerializer()),
                    TestController = testController,
                    ReportController = new ReportController(new ReportService(reportManager), new FileSystem()),
                    RuntimeLogController = runtimeLogController,
                    OptionsController = optionsController
                };
                mediator.ExecutionLogController = new ExecutionLogController(mediator.TestController,
                    optionsController);
                mediator.AnnotationsController = new AnnotationsController(mediator.TestController);
                mediator.TestResultsController = new TestResultsController(mediator.TestController, 
                    mediator.OptionsController);

                var applicationController = new ApplicationController(Arguments, mediator);
                var main = new Main(applicationController);

                Application.Run(main);
            }

            return ResultCode.Success;
        }

        private void ConfigureTestRunnerFactory(string factoryName)
        {
            var testRunnerManager = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>();
            var testRunnerFactory = testRunnerManager.GetFactory(factoryName);
            testController.SetTestRunnerFactory(testRunnerFactory);
        }

        protected override void ShowHelp()
        {
            ArgumentParser.ShowUsageInMessageBox(ApplicationTitle);
        }

        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        internal static int Main(string[] args)
        {
            return new IcarusProgram().Run(NativeConsole.Instance, args);
        }
    }
}
