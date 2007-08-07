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
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Castle.Core.Logging;
using MbUnit.Core.Harness;
using MbUnit.Core.Runner.Monitors;
using MbUnit.Core.Services.Runtime;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Running tests with Gallio involves creating specific objects in a predefined
    /// way. This class tries to simplify this process by implementing a common
    /// pattern.
    /// </summary>
    public class TestRunnerHelper : IDisposable
    {
        #region Private Members

        private readonly TestPackage package;
        private readonly ProgressMonitorCreator progressMonitorCreator;
        private readonly RuntimeSetup runtimeSetup;
        private readonly LevelFilteredLogger logger;
        private readonly StringWriter debugWriter;
        private readonly Filter<ITest> filter;
        private TreePersister templateTreePersister;
        private TreePersister testTreePersister;
        private Stopwatch stopWatch;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressMonitorCreator">A delegate to a rutine that will be
        /// called to create progress monitors</param>
        /// <param name="logger"></param>
        /// <param name="verbosity"></param>
        /// <param name="filter"></param>
        public TestRunnerHelper(
            ProgressMonitorCreator progressMonitorCreator,
            LevelFilteredLogger logger,
            Verbosity verbosity,
            Filter<ITest> filter)
        {
            CheckRequiredArgument(progressMonitorCreator, "progressMonitorCreator");
            CheckRequiredArgument(filter, "filter");
            CheckRequiredArgument(logger, "logger");

            runtimeSetup = new RuntimeSetup();
            package = new TestPackage();

            this.progressMonitorCreator = progressMonitorCreator;
            this.filter = filter;
            this.logger = logger;
            if (verbosity == Verbosity.Debug)
            {
                debugWriter = new StringWriter();
            }
        }

        #endregion

        #region Public Delegates

        /// <summary>
        /// Defines a method capable of persisting a tree (template or test)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="progressMonitor"></param>
        public delegate void TreePersister(object root, IProgressMonitor progressMonitor);

        /// <summary>
        /// Defines a method that is able to create IProgressMonitor objects
        /// </summary>
        /// <returns></returns>
        public delegate IProgressMonitor ProgressMonitorCreator();

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public TreePersister TemplateTreePersister
        {
            get { return templateTreePersister; }
            set { templateTreePersister = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TreePersister TestTreePersister
        {
            get { return testTreePersister; }
            set { testTreePersister = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Run()
        {
            using (AutoRunner runner = AutoRunner.CreateRunner(runtimeSetup))
            {
                if (!HasTestAssemblies())
                    return ResultCode.NoTests;

                if (debugWriter != null)
                    new DebugMonitor(debugWriter).Attach(runner);
                
                ApplyFilter(runner);

                CreateStopWatch();

                if (!LoadProject(runner))
                    return ResultCode.Canceled;

                if (!BuildTemplates(runner))
                    return ResultCode.Canceled;

                if (!BuildTests(runner))
                    return ResultCode.Canceled;

                if (!PersistTemplateTree(runner))
                    return ResultCode.Canceled;

                if (!PersistTestTree(runner))
                    return ResultCode.Canceled;

                if (!RunTests(runner))
                    return ResultCode.Canceled;

                if (debugWriter != null)
                    logger.Debug(debugWriter.ToString());

                LogExecutionTime();
                stopWatch = null;

                return ResultCode.Success;
            }
        }

        private void LogExecutionTime()
        {
            logger.InfoFormat("\nStop time: {0}", DateTime.Now.ToShortTimeString());
            logger.InfoFormat("Total execution time: {0:#0.000}s", stopWatch.Elapsed.TotalSeconds);
        }

        private void CreateStopWatch()
        {
            stopWatch = Stopwatch.StartNew();
            logger.InfoFormat("\nStart time: {0}\n", DateTime.Now.ToShortTimeString());
        }

        #region Utility Methods

        /// <summary>
        /// Adds a plugin directory
        /// </summary>
        /// <param name="pluginDirectory"></param>
        public void AddPluginDirectory(string pluginDirectory)
        {
            AddPluginDirectory(pluginDirectory, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginDirectory"></param>
        /// <param name="resolvePath"></param>
        public void AddPluginDirectory(string pluginDirectory, bool resolvePath)
        {
            string path = pluginDirectory;
            if (resolvePath)
            {
                path = Path.GetFullPath(path);
            }
            runtimeSetup.AddPluginDirectory(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginDirectories"></param>
        public void AddPluginDirectories(IEnumerable<string> pluginDirectories)
        {
            AddPluginDirectories(pluginDirectories, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginDirectories"></param>
        /// <param name="resolvePaths"></param>
        public void AddPluginDirectories(IEnumerable<string> pluginDirectories, bool resolvePaths)
        {
            foreach (string pluginDirectory in pluginDirectories)
            {
                AddPluginDirectory(pluginDirectory, resolvePaths);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hintDirectory"></param>
        public void AddHintDirectory(string hintDirectory)
        {
            AddHintDirectory(hintDirectory, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hintDirectory"></param>
        /// <param name="resolvePath"></param>
        public void AddHintDirectory(string hintDirectory, bool resolvePath)
        {
            string path = hintDirectory;
            if (resolvePath)
            {
                path = Path.GetFullPath(path);
            }
            package.AddHintDirectory(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hintDirectories"></param>
        public void AddHintDirectories(IEnumerable<string> hintDirectories)
        {
            AddHintDirectories(hintDirectories, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hintDirectories"></param>
        /// <param name="resolvePaths"></param>
        public void AddHintDirectories(IEnumerable<string> hintDirectories, bool resolvePaths)
        {
            foreach (string hintDirectory in hintDirectories)
            {
                AddHintDirectory(hintDirectory, resolvePaths);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile"></param>
        public void AddAssemblyFile(string assemblyFile)
        {
            AddAssemblyFile(assemblyFile, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="resolvePath"></param>
        public void AddAssemblyFile(string assemblyFile, bool resolvePath)
        {
            string path = assemblyFile;
            if (resolvePath)
            {
                path = Path.GetFullPath(path);
            }
            package.AddAssemblyFile(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFiles"></param>
        public void AddAssemblyFiles(IEnumerable<string> assemblyFiles)
        {
            AddAssemblyFiles(assemblyFiles, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyFiles"></param>
        /// <param name="resolvePaths"></param>
        public void AddAssemblyFiles(IEnumerable<string> assemblyFiles, bool resolvePaths)
        {
            foreach (string assemblyFile in assemblyFiles)
            {
                package.AddAssemblyFile(assemblyFile);
            }
        } 

        #endregion

        #endregion

        #region Private Methods

        private bool HasTestAssemblies()
        {
            if (package.AssemblyFiles.Length == 0)
            {
                logger.Warn("No test assemblies to execute!");
                return false;
            }

            return true;
        }

        private void ApplyFilter(ITestRunner runner)
        {
            runner.TestExecutionOptions.Filter = filter;
        }
        
        private bool LoadProject(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                runner.LoadProject(progressMonitor, package);
                if (progressMonitor.IsCanceled)
                    return false;
            }
            return true;
        }

        private bool BuildTemplates(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                runner.BuildTemplates(progressMonitor);
                if (progressMonitor.IsCanceled)
                    return false;
            }
            return true;
        }

        private bool BuildTests(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                runner.BuildTests(progressMonitor);
                if (progressMonitor.IsCanceled)
                    return false;
            }
            return true;
        }

        private bool RunTests(ITestRunner runner)
        {
            using (IProgressMonitor progressMonitor = progressMonitorCreator())
            {
                runner.Run(progressMonitor);
                if (progressMonitor.IsCanceled)
                    return false;
            }
            return true;
        }

        private bool PersistTemplateTree(ITestRunner runner)
        {
            if (templateTreePersister != null)
            {
                using (IProgressMonitor progressMonitor = progressMonitorCreator())
                {
                    templateTreePersister(runner.TemplateModel.RootTemplate, progressMonitor);
                    if (progressMonitor.IsCanceled)
                        return false;
                }
            }
            return true;
        }

        private bool PersistTestTree(ITestRunner runner)
        {
            if (testTreePersister != null)
            {
                using (IProgressMonitor progressMonitor = progressMonitorCreator())
                {
                    testTreePersister(runner.TestModel.RootTest, progressMonitor);
                    if (progressMonitor.IsCanceled)
                        return false;
                }
            }
            return true;
        }

        private static void CheckRequiredArgument(object argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// The IDisposable interface is implemented just to be able to use this
        /// class in a using statement.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }
}
