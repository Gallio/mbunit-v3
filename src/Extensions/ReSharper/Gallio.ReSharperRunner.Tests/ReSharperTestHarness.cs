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

using System.IO;
using System.Reflection;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Tests;
using JetBrains.ProjectModel;
using JetBrains.Util;
using MbUnit.Framework;

#if RESHARPER_31
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Shell.Test;
#else
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.Application.Test;
#endif

namespace Gallio.ReSharperRunner.Tests
{
    /// <summary>
    /// This class helps to configure a ReSharper test shell with a limited
    /// subset of ReSharper assemblies that we require for intergration
    /// testing purposes.
    /// </summary>
    [AssemblyFixture]
    public static class ReSharperTestHarness
    {
        private static readonly Assembly testAssembly = typeof(ReSharperTestHarness).Assembly;
        private static bool isTestSolutionLoaded;

        private static readonly string VersionSuffix = typeof(ReSharperTestHarness).Assembly.GetName().Name.Substring(22, 2);

        [FixtureSetUp]
        public static void SetUp()
        {
            if (Shell.Instance == null)
            {
                string configName = typeof(ReSharperTestHarness).Namespace + ".TestAssemblies" + VersionSuffix + ".xml";
#if RESHARPER_31
                new TestShell(testAssembly, configName);
#else
                new TestShell(testAssembly, testAssembly, configName);
#endif
            }
        }

        [FixtureTearDown]
        public static void TearDown()
        {
            if (isTestSolutionLoaded)
            {
                RunWithWriteLock(delegate
                {
                    SolutionManager.Instance.CloseSolution(SolutionManager.Instance.CurrentSolution);
                });
                isTestSolutionLoaded = false;
            }

            TestShell shell = Shell.Instance as TestShell;
            if (shell != null)
            {
                shell.TearDown();
            }
        }

        public static void LoadTestSolutionIfNeeded()
        {
            if (isTestSolutionLoaded)
                return;

            FileSystemPath testSolutionPath = new FileSystemPath(
                Path.Combine(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(testAssembly)), @"..\..\TestSolution" + VersionSuffix + ".sln"));

            RunWithWriteLock(delegate
            {
                SolutionManager.Instance.OpenSolution(testSolutionPath, new SimpleTaskExecutor());
            });

            isTestSolutionLoaded = true;
        }

        private static void RunWithWriteLock(Action action)
        {
#if !RESHARPER_31
            TestShell.RunGuarded(delegate
            {
                using (WriteLockCookie.Create())
                {
#endif
                    action();
#if !RESHARPER_31
                }
            });
#endif
        }
    }
}
