// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Runtime;
using Gallio.Icarus.Adapter;
using Gallio.Icarus.AdapterModel;
using Gallio.Icarus.Core.Model;
using Gallio.Icarus.Core.Presenter;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Reflection;
using Gallio.Runner.Projects;
using Gallio.Runtime.Windsor;

namespace Gallio.Icarus
{
    static class Program
    {
        private static Main main;
        private static ProjectAdapter projectAdapter;
        private static ProjectPresenter projectPresenter;

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
            using (RuntimeBootstrap.Initialize(WindsorRuntimeFactory.Instance, runtimeSetup, new IcarusLogger(main)))
            {
                // wire up model
                projectAdapter = new ProjectAdapter(main, new ProjectAdapterModel());
                if (args.Length > 0)
                {
                    Project project = ParseArguments(args);
                    if (project != null)
                        projectAdapter.Project = project;
                }
                else
                {
                    if (main.Settings.RestorePreviousSettings)
                    {
                        string defaultProject = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Gallio\\Icarus\\Icarus.gallio");
                        if (File.Exists(defaultProject))
                            main.ProjectFileName = defaultProject;
                    }
                }

                projectPresenter = new ProjectPresenter(projectAdapter, new TestRunnerModel());

                Application.Run(main);
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
