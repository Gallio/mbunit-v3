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

// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux


using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Castle.Core.Logging;
using MbUnit.Core.Runner;
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Core.Serialization;
using MbUnit.Echo;

namespace MbUnit.Echo
{
    public sealed class MainClass : IDisposable
    {
        private string applicationTitle;
        private LevelFilteredLogger logger;
        private MainArguments arguments = new MainArguments();
        private TestProjectInfo project;

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

        private void BuildProjectFromArguments()
        {
            project = TestProjectInfo.Create();

            foreach (string path in arguments.AssemblyPath)
                project.AddHintDirectory(path);

            foreach (string file in arguments.Files)
                project.AddAssemblyFile(file);
        }

        private void RunProject()
        {
            if (project.AssemblyFiles.Length == 0)
            {
                logger.Warn("No test assemblies to execute!");
                return;
            }

            AutoRunner runner = new AutoRunner();
            runner.Logger = logger;

            Stopwatch stopWatch = Stopwatch.StartNew();
            logger.InfoFormat("\nStart time: {0}", DateTime.Now.ToShortTimeString());
            logger.Info("Loading test assemblies...\n");

            runner.LoadProject(project);

            logger.Info("\nStarting execution...\n");
            runner.Run();

            logger.InfoFormat("\nStop time: {0}", DateTime.Now.ToShortTimeString());
            logger.InfoFormat("Total execution time: {0:#0.000}s", stopWatch.Elapsed.TotalSeconds);
        }
    }
}
