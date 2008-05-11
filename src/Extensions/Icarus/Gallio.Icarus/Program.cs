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
using Gallio.Icarus.Adapter;
using Gallio.Icarus.AdapterModel;
using Gallio.Icarus.Core.Model;
using Gallio.Icarus.Core.Presenter;
using Gallio.Reflection;
using Gallio.Runner;
using Gallio.Runner.Projects;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Icarus.Interfaces;
using Gallio.Icarus.Core.Interfaces;

namespace Gallio.Icarus
{
    static class Program
    {
        private static Main main;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            main = new Main();

            RuntimeSetup runtimeSetup = new RuntimeSetup();
            // Set the installation path explicitly to ensure that we do not encounter problems
            // when the test assembly contains a local copy of the primary runtime assemblies
            // which will confuse the runtime into searching in the wrong place for plugins.
            runtimeSetup.InstallationPath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(Program).Assembly));
            runtimeSetup.PluginDirectories.AddRange(main.Settings.PluginDirectories);
            using (RuntimeBootstrap.Initialize(runtimeSetup, new IcarusLogger(main)))
            {
                // wire up model
                IProjectAdapter projectAdapter = new ProjectAdapter(main, new ProjectAdapterModel());
                if (args.Length > 0)
                {
                    Project project = ParseArguments(args);
                    if (project != null)
                        projectAdapter.Project = project;
                }
                else
                {
                    if (main.Settings.RestorePreviousSettings && File.Exists(Paths.DefaultProject))
                        main.ProjectFileName = Paths.DefaultProject;
                }

                ITestRunner testRunner = RuntimeAccessor.Instance.Resolve<ITestRunnerManager>().CreateTestRunner(
                    main.Settings.TestRunnerFactory);

                IReportManager reportManager = RuntimeAccessor.Instance.Resolve<IReportManager>();
                ITestRunnerModel testRunnerModel = new TestRunnerModel(testRunner, reportManager);

                IProjectPresenter projectPresenter = new ProjectPresenter(projectAdapter, testRunnerModel);

                testRunnerModel.Initialize();
                main.CleanUp += delegate { testRunnerModel.Dispose(); };

                Application.Run(main);

                GC.KeepAlive(projectPresenter);
            }
        }

        private static Project ParseArguments(string[] args)
        {
            // parse command line arguments
            CommandLineArgumentParser argumentParser = new CommandLineArgumentParser(typeof(Arguments));
            Arguments arguments = new Arguments();
            Project project = new Project();
            project.TestPackageConfig.HostSetup.ShadowCopy = true;

            if (argumentParser.Parse(args, arguments, delegate { }))
            {
                foreach (string file in arguments.Assemblies)
                {
                    if (File.Exists(file))
                    {
                        if (file.EndsWith(".gallio"))
                        {
                            main.ProjectFileName = file;
                            return null;
                        }
                        else
                            project.TestPackageConfig.AssemblyFiles.Add(file);
                    }
                }
            }
            return project;
        }
    }
}
