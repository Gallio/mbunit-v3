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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Logging;
using Gallio.Icarus.Properties;
using Gallio.Icarus.Runtime;
using Gallio.Runner;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Common.Policies;
using Gallio.UI.ErrorReporting;

namespace Gallio.Icarus
{
    /// <summary>
    /// The Icarus program.
    /// </summary>
    public class IcarusProgram : ConsoleProgram<IcarusArguments>
    {
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

            if (Arguments.Help)
            {
                ShowHelp();
                return 0;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            UnhandledErrorPolicy();

            var runtimeSetup = new RuntimeSetup
            {
                RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(
                    typeof(IcarusProgram).Assembly))
            };

            var runtimeLogger = new RuntimeLogger();

            GenericCollectionUtils.ForEach(Arguments.PluginDirectories, runtimeSetup.AddPluginDirectory);

            using (RuntimeBootstrap.Initialize(runtimeSetup, runtimeLogger))
            {
                // wire up services & components
                var scanner = new DefaultConventionScanner(RuntimeAccessor.Registry, "Gallio.Icarus");
                scanner.Scan(Assembly.GetExecutingAssembly());

                var optionsController = RuntimeAccessor.ServiceLocator.Resolve<IOptionsController>();
                
                // create & initialize a test runner whenever the test runner factory is changed
                optionsController.TestRunnerFactory.PropertyChanged += (s, e) => 
                    ConfigureTestRunnerFactory(optionsController.TestRunnerFactory);
                
                ConfigureTestRunnerFactory(optionsController.TestRunnerFactory);
                
                var runtimeLogController = RuntimeAccessor.ServiceLocator.Resolve<IRuntimeLogController>();
                runtimeLogController.SetLogger(runtimeLogger);

                var applicationController = RuntimeAccessor.ServiceLocator.Resolve<IApplicationController>();
                applicationController.Arguments = Arguments;

                ErrorDialogUnhandledExceptionHandler.RunApplicationWithHandler(new Main(applicationController));
            }

            return ResultCode.Success;
        }

        private static void UnhandledErrorPolicy()
        {
            BlackBoxLogger.Initialize();
            UnhandledExceptionPolicy.ReportUnhandledException += (sender, e) => BlackBoxLogger.Log(e);

            Application.ThreadException += (sender, e) => UnhandledExceptionPolicy.Report("Error from Application.ThreadException", e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => UnhandledExceptionPolicy.Report("Error from Application.ThreadException",
                (Exception)e.ExceptionObject);
        }

        private static void ConfigureTestRunnerFactory(string factoryName)
        {
            var testController = RuntimeAccessor.ServiceLocator.Resolve<ITestController>();
            var testRunnerManager = RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>();
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
