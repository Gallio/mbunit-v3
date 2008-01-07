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
using System.Windows.Forms;
using Gallio.Icarus.Adapter;
using Gallio.Icarus.AdapterModel;
using Gallio.Icarus.Core.Model;
using Gallio.Icarus.Core.Presenter;
using Gallio.Core.ConsoleSupport;
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
        [STAThread, LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // TODO: Should use some kind of GUI-based logger.
            Runtime.Initialize(new RuntimeSetup(), new RichConsoleLogger(NativeConsole.Instance));
            try
            {
                // wire up model
                Main main = new Main();
                ProjectAdapter projectAdapter = new ProjectAdapter(main, new ProjectAdapterModel());
                projectAdapter.Project = ParseArguments(args);
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
                    if (file.EndsWith(".gallio"))
                    {
                        try
                        {
                            project = SerializationUtils.LoadFromXml<Project>(file);
                        }
                        catch (Exception)
                        { }
                        return project;
                    }
                    else
                    {
                        project.TestPackageConfig.AssemblyFiles.Add(file);
                    }
                }
            }
            return project;
        }
    }
}