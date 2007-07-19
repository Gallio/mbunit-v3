// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Castle.Core.Logging;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Core.Services.Runtime;
using MbUnit.Echo;

namespace MbUnit.Echo
{
    public sealed class MainClass : IDisposable
    {
        private string applicationTitle;
        private LevelFilteredLogger logger;
        private MainArguments arguments = new MainArguments();
        private TestProject project;
        private RuntimeSetup runtimeSetup;

        public MainClass()
        {
            logger = new PrettyConsoleLogger();
        }

        public void Dispose()
        {
        }

        public LevelFilteredLogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        public int Run(string[] args)
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            applicationTitle = string.Format("MbUnit Echo - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build);

            Console.Title = applicationTitle;

            try
            {
                try
                {
                    CommandLineUtility.ParseCommandLineArguments(args, arguments, delegate(string message)
                    {
                        logger.FatalFormat("Error: {0}", message);
                    });
                }
                catch (Exception)
                {
                    ShowHelp();
                    return ResultCode.InvalidArguments;
                }

                logger.Level = arguments.Verbose ? LoggerLevel.Debug : LoggerLevel.Info;

                if (arguments.Help)
                {
                    ShowHelp();
                    return ResultCode.Success;
                }

                logger.Debug(arguments.ToString());

                BuildRuntimeSetupFromArguments();
                BuildProjectFromArguments();
                RunProject();

                return ResultCode.Success;
            }
            catch (Exception ex)
            {
                logger.FatalFormat(ex, "A fatal exception occurred.");
                return ResultCode.FatalException;
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(applicationTitle);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Get the latest version at ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("http://www.mbunit.com/");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 78));

            Console.ResetColor();
            Console.WriteLine("Project Lead: Andrew Stopford");
            Console.WriteLine("Contributors: Ben Hall, Graham Hay, Johan Appelgren, Joey Calisay,");
            Console.WriteLine("              David Parkinson, Jeff Brown, Marc Stober, Mark Haley");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 78));

            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine(CommandLineUtility.CommandLineArgumentsUsage(typeof(MainArguments)));
        }

        private void BuildRuntimeSetupFromArguments()
        {
            runtimeSetup = new RuntimeSetup();

            foreach (string pluginDirectory in arguments.PluginDirectories)
                runtimeSetup.AddPluginDirectory(Path.GetFullPath(pluginDirectory));
        }

        private void BuildProjectFromArguments()
        {
            project = new TestProject();

            foreach (string path in arguments.AssemblyPath)
                project.AddHintDirectory(Path.GetFullPath(path));

            foreach (string file in arguments.Files)
                project.AddAssemblyFile(Path.GetFullPath(file));
        }

        private void RunProject()
        {
            if (project.AssemblyFiles.Length == 0)
            {
                logger.Warn("No test assemblies to execute!");
                return;
            }

            using (AutoRunner runner = AutoRunner.CreateRunner(runtimeSetup))
            {
                runner.Logger = logger;

                Stopwatch stopWatch = Stopwatch.StartNew();
                logger.InfoFormat("\nStart time: {0}", DateTime.Now.ToShortTimeString());
                logger.Info("Loading test assemblies...\n");

                runner.LoadProject(project);

                if (arguments.SaveTemplateTree != null)
                {
                    SaveToXml(runner.GetTemplateTreeRoot(), arguments.SaveTemplateTree);
                }

                if (arguments.SaveTestTree != null)
                {
                    SaveToXml(runner.GetTestTreeRoot(), arguments.SaveTestTree);
                }

                logger.Info("\nStarting execution...\n");
                runner.Run();

                logger.InfoFormat("\nStop time: {0}", DateTime.Now.ToShortTimeString());
                logger.InfoFormat("Total execution time: {0:#0.000}s", stopWatch.Elapsed.TotalSeconds);
            }
        }

        private static void SaveToXml(object root, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(root.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
                serializer.Serialize(writer, root);
        }
    }
}
