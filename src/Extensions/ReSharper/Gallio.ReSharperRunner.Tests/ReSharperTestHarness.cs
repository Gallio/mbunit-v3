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

extern alias MbUnit2;
using System.IO;
using System.Reflection;
using Gallio.Hosting;
using Gallio.ReSharperRunner.Tests;
using JetBrains.ProjectModel;
using JetBrains.Shell;
using JetBrains.Shell.Progress;
using JetBrains.Shell.Test;
using JetBrains.Util;
using MbUnit2::MbUnit.Framework;

[assembly: AssemblyCleanup(typeof(ReSharperTestHarness))]

namespace Gallio.ReSharperRunner.Tests
{
    /// <summary>
    /// This class helps to configure a ReSharper test shell with a limited
    /// subset of ReSharper assemblies that we require for intergration
    /// testing purposes.
    /// </summary>
    public static class ReSharperTestHarness
    {
        private static readonly Assembly TestAssembly = typeof(ReSharperTestHarness).Assembly;
        private static bool isTestSolutionLoaded;

        [SetUp]
        public static void SetUp()
        {
            if (Shell.Instance == null)
            {
                new TestShell(TestAssembly, TestAssembly.GetName().Name + ".TestAssemblies.xml");
            }
        }

        [TearDown]
        public static void TearDown()
        {
            TestShell shell = Shell.Instance as TestShell;
            if (shell != null)
            {
                shell.TearDown();
                isTestSolutionLoaded = false;
            }
        }

        public static void LoadTestSolutionIfNeeded()
        {
            if (isTestSolutionLoaded)
                return;

            FileSystemPath testSolutionPath = new FileSystemPath(
                Path.Combine(Path.GetDirectoryName(Loader.GetAssemblyLocalPath(TestAssembly)), @"..\TestSolution.sln"));
            SolutionManager.Instance.OpenSolution(testSolutionPath, new SimpleTaskExecutor());

            isTestSolutionLoaded = true;
        }
    }
}
