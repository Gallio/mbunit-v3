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
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Castle.Core.Logging;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Framework.Kernel.Events;

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
        private LevelFilteredLogger logger = new PrettyConsoleLogger();
        private readonly MainArguments arguments = new MainArguments();
        private bool haltExecution = false;
        private int resultCode;

        public MainClass(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            SetUp(args);
        }

        public void SetUp(string[] args)
        {
            SetApplicationTitle();
            InstallCancelHandler();

            if (!ParseArguments(args))
            {
                haltExecution = true;
                resultCode = ResultCode.InvalidArguments;
            }

            if (arguments.Help)
            {
                ShowHelp();
                haltExecution = true;
                resultCode = ResultCode.Success;
            }

            if (resultCode == ResultCode.Success)
                SetUpLogger();
        }

        public int Run()
        {
            try
            {
                if (haltExecution)
                {
                    return resultCode;
                }
                resultCode = RunTests();
                CheckResultCode();
                return resultCode;
            }
            catch (Exception ex)
            {
                logger.FatalFormat(ex, "A fatal exception occurred.");
                return ResultCode.FatalException;
            }
        }

        private int RunTests()
        {
            using (TestRunnerHelper testRunnerHelper = new TestRunnerHelper
                (
                delegate { return new ConsoleProgressMonitor(); },
                logger,
                arguments.GetFilter()
                ))
            {
                testRunnerHelper.TemplateTreePersister = SaveTemplateTree;
                testRunnerHelper.TemplateTreePersister = SaveTestTree;
                testRunnerHelper.AddPluginDirectories(arguments.PluginDirectories);
                testRunnerHelper.AddHintDirectories(arguments.HintDirectories);
                testRunnerHelper.AddAssemblyFiles(arguments.Assemblies);
                return testRunnerHelper.Run();
            }
        }

        private void SetUpLogger()
        {
            SetVerbosityLevel();
            logger.Debug(arguments.ToString());
        }

        private static void InstallCancelHandler()
        {
            // Disable ordinary cancelation handling.
            // We handle cancelation directly in ways that should result in the user
            // losing less data than if the OS just kills the process.
            ConsoleCancelHandler.Install();
        }

        private void CheckResultCode()
        {
            if (resultCode == ResultCode.Canceled)
                logger.Warn("Canceled!");
        }

        private bool ParseArguments(string[] args)
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
                return false;
            }
            return true;
        }

        private void SetApplicationTitle()
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            applicationTitle = string.Format("MbUnit Echo - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build);
            Console.Title = applicationTitle;
        }

        private void SetVerbosityLevel()
        {
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

        private void SaveTemplateTree(object root, IProgressMonitor progressMonitor)
        {
            if (arguments.SaveTemplateTree != null)
            {
                progressMonitor.BeginTask("Saving template tree to: " + arguments.SaveTemplateTree + ".", 1);
                SaveToXml(root, arguments.SaveTemplateTree);
            }
        }

        private void SaveTestTree(object root, IProgressMonitor progressMonitor)
        {
            if (arguments.SaveTestTree != null)
            {
                progressMonitor.BeginTask("Saving test tree to: " + arguments.SaveTestTree + ".", 1);
                SaveToXml(root, arguments.SaveTestTree);
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

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion
    }
}
