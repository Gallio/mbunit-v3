// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using Gallio.Icarus.Adapter;
using Gallio.Icarus.AdapterModel;
using Gallio.Icarus.Core.Model;
using Gallio.Icarus.Core.Presenter;
using Gallio.Hosting.ConsoleSupport;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Projects;

namespace Gallio.Icarus
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IcarusLogger icarusLogger = new IcarusLogger();
            Runtime.Initialize(new RuntimeSetup(), icarusLogger);
            try
            {
                // wire up model
                Main main = new Main();
                icarusLogger.ProjectAdapterView = main;
                ProjectAdapter projectAdapter = new ProjectAdapter(main, new ProjectAdapterModel());
                if (args.Length > 0)
                    projectAdapter.Project = ParseArguments(args);
                else
                {
                    if (main.Settings.RestorePreviousSettings)
                    {
                        string defaultProject = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Gallio/Icarus/Icarus.gallio");
                        if (File.Exists(defaultProject))
                        {
                            try
                            {
                                projectAdapter.Project = LoadProject(defaultProject);
                            }
                            catch
                            {
                                MessageBox.Show("Cannot load default project \"" + defaultProject + "\"");
                                //TODO: Maybe delete the buggy project?
                            }
                        }
                    }
                }
                main.HintDirectories = projectAdapter.Project.TestPackageConfig.HintDirectories;
                main.ApplicationBaseDirectory = projectAdapter.Project.TestPackageConfig.HostSetup.ApplicationBaseDirectory;
                main.ShadowCopy = projectAdapter.Project.TestPackageConfig.HostSetup.ShadowCopy;
                main.WorkingDirectory = projectAdapter.Project.TestPackageConfig.HostSetup.WorkingDirectory;
                ProjectPresenter projectPresenter = new ProjectPresenter(projectAdapter, new TestRunnerModel());
                Application.Run(main);
            }
            finally
            {
                Runtime.Shutdown();
            }
        }

        private static Project ParseArguments(string[] args)
        {
            // parse command line arguments
            CommandLineArgumentParser argumentParser = new CommandLineArgumentParser(typeof(Arguments));
            Arguments arguments = new Arguments();
            Project project = new Project();
            if (argumentParser.Parse(args, arguments, delegate { }))
            {
                foreach (string file in arguments.Assemblies)
                {
                    if (File.Exists(file))
                    {
                        if (file.EndsWith(".gallio"))
                        {
                            project = LoadProject(file);
                            break;
                        }
                        else
                            project.TestPackageConfig.AssemblyFiles.Add(file);
                    }
                }
            }
            return project;
        }

        private static Project LoadProject(string fileName)
        {
            Project project = new Project();
            try
            {
                project = SerializationUtils.LoadFromXml<Project>(fileName);
            }
            catch
            { }
            return project;
        }
    }
}