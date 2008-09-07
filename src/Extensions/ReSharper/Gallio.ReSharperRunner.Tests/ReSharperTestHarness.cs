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
using JetBrains.UI;
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
using Resources;

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
        private static readonly Assembly TestAssembly = typeof(ReSharperTestHarness).Assembly;
        private static bool isTestSolutionLoaded;

        private static readonly string VersionSuffix = typeof(ReSharperTestHarness).Assembly.GetName().Name.Substring(22, 2);

        [FixtureSetUp]
        public static void SetUp()
        {
            if (Shell.Instance == null)
            {
                new GallioTestShell();
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

            GallioTestShell shell = Shell.Instance as GallioTestShell;
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
                Path.Combine(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(TestAssembly)), @"..\..\TestSolution" + VersionSuffix + ".sln"));

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

        private class GallioTestShell : TestShell
        {
            // We need to use our own configuration because the standard JetBrains.resources.config.AllAssemblies.xml
            // resource in JetBrains.ReSharper.Resources refers to actual testing assemblies that JetBrains does not
            // publish.  This is problematic because it causes the TestShell to fail to initialize.
            //
            // Our configuration is basically a stripped down version of the standard one. -- Jeff.
            private static readonly string ConfigName = typeof(ReSharperTestHarness).Namespace + ".TestAssemblies" + VersionSuffix + ".xml";

            public GallioTestShell()
#if RESHARPER_31
                : base(TestAssembly, ConfigName)
#else
                : base(TestAssembly, TestAssembly, ConfigName)
#endif
            {
            }

#if ! RESHARPER_31 && ! RESHARPER_40
            public override void  InitializeComponents()
            {
                // As of ReSharper v4.1, the way the images are loaded has changed somewhat.
                // Instead of always loading from JetBrains.ReSharper.Resources, the images are resolved
                // from the "ConfigurationAssembly", which is the one that contains "AllAssemblies.xml".
                // Of course, that's our test assembly which obviously does not have the resources we need.
                //
                // So we explicitly make JetBrains.ReSharper.Resources eligible for image loading.
                //
                // Another approach might be to hack the ApplicationConfiguration object after the
                // AllAssembliesXml has been loaded. -- Jeff.
                ImageLoader.ImageId.AssembliesEligibleDefault[0] = typeof(ResourceLoader).Assembly;

                base.InitializeComponents();
            }
#endif
        }
    }
}
