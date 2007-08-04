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
using MbUnit.Core.Harness;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Core.Runner.Monitors;
using MbUnit.Core.Services.Runtime;
using MbUnit.Echo;

namespace MbUnit.Echo
{
    /// <summary>
    /// The main process.
    /// </summary>
    /// <todo author="jeff">
    /// Cancelation should be handled more intelligently.  For example, if the user
    /// cancels the test run, we should still display summary results.
    /// </todo>
    public sealed class MainClass : IDisposable
    {
        private string applicationTitle;
        private LevelFilteredLogger logger;
        private MainArguments arguments = new MainArguments();
        private TestPackage package;
        private RuntimeSetup runtimeSetup;
        private AutoRunner runner;

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
            // Disable ordinary cancelation handling.
            // We handle cancelation directly in ways that should result in the user
            // losing less data than if the OS just kills the process.
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
            };

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

                switch (arguments.Verbosity)
                {
                    case Verbosity.Quiet:
                        logger.Level = LoggerLevel.Warn;
                        break;
                    case Verbosity.Normal:
                    case Verbosity.Verbose:
                        logger.Level = LoggerLevel.Info;
                        break;
                    case Verbosity.Debug:
                        logger.Level = LoggerLevel.Debug;
                        break;
                }

                if (arguments.Help)
                {
                    ShowHelp();
                    return ResultCode.Success;
                }

                logger.Debug(arguments.ToString());

                BuildRuntimeSetupFromArguments();
                BuildPackageFromArguments();
                int resultCode = RunPackage();

                if (resultCode == ResultCode.Canceled)
                    logger.Warn("Canceled!");
                return resultCode;
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

        private void BuildPackageFromArguments()
        {
            package = new TestPackage();

            foreach (string path in arguments.AssemblyPath)
                package.AddHintDirectory(Path.GetFullPath(path));

            foreach (string file in arguments.Files)
                package.AddAssemblyFile(Path.GetFullPath(file));
        }

        private int RunPackage()
        {
            if (package.AssemblyFiles.Length == 0)
            {
                logger.Warn("No test assemblies to execute!");
                return ResultCode.Success;
            }

            using (runner = AutoRunner.CreateRunner(runtimeSetup))
            {
                StringWriter debugWriter = null;
                if (arguments.Verbosity == Verbosity.Debug)
                {
                    debugWriter = new StringWriter();
                    new DebugMonitor(debugWriter).Attach(runner);
                }

                runner.TestExecutionOptions.Filter = arguments.GetFilter();

                Stopwatch stopWatch = Stopwatch.StartNew();
                logger.InfoFormat("\nStart time: {0}", DateTime.Now.ToShortTimeString());

                using (ConsoleProgressMonitor progressMonitor = new ConsoleProgressMonitor())
                {
                    runner.LoadProject(progressMonitor, package);

                    if (progressMonitor.IsCanceled)
                        return ResultCode.Canceled;
                }

                using (ConsoleProgressMonitor progressMonitor = new ConsoleProgressMonitor())
                {
                    runner.BuildTemplates(progressMonitor);

                    if (progressMonitor.IsCanceled)
                        return ResultCode.Canceled;
                }

                using (ConsoleProgressMonitor progressMonitor = new ConsoleProgressMonitor())
                {
                    runner.BuildTests(progressMonitor);

                    if (progressMonitor.IsCanceled)
                        return ResultCode.Canceled;
                }

                if (arguments.SaveTemplateTree != null)
                {
                    using (ConsoleProgressMonitor progressMonitor = new ConsoleProgressMonitor())
                    {
                        progressMonitor.BeginTask("Saving template tree to: " + arguments.SaveTemplateTree + ".", 1);

                        SaveToXml(runner.TemplateModel.RootTemplate, arguments.SaveTemplateTree);

                        if (progressMonitor.IsCanceled)
                            return ResultCode.Canceled;
                    }
                }

                if (arguments.SaveTestTree != null)
                {
                    using (ConsoleProgressMonitor progressMonitor = new ConsoleProgressMonitor())
                    {
                        progressMonitor.BeginTask("Saving test tree to: " + arguments.SaveTestTree + ".", 1);

                        SaveToXml(runner.TestModel.RootTest, arguments.SaveTestTree);

                        if (progressMonitor.IsCanceled)
                            return ResultCode.Canceled;
                    }
                }

                using (ConsoleProgressMonitor progressMonitor = new ConsoleProgressMonitor())
                {
                    runner.Run(progressMonitor);

                    if (progressMonitor.IsCanceled)
                        return ResultCode.Canceled;
                }

                if (debugWriter != null)
                    logger.Debug(debugWriter.ToString());

                logger.InfoFormat("\nStop time: {0}", DateTime.Now.ToShortTimeString());
                logger.InfoFormat("Total execution time: {0:#0.000}s", stopWatch.Elapsed.TotalSeconds);
            }

            // TODO: Return a different code if there were failures.
            return ResultCode.Success;
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
