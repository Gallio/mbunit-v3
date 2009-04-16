// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using System.IO;
using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using Gallio.Reflection;
using Gallio.Runtime.Logging;
using Gallio.VisualStudio.Interop;
using MbUnit.Framework;

namespace Gallio.VisualStudio.Shell.Tests
{
    /// <summary>
    /// This class launches an instance of Visual Studio for running integration tests.
    /// </summary>
    [AssemblyFixture]
    public static class VisualStudioTestHarness
    {
        private static IVisualStudio visualStudio;

        /// <summary>
        /// Gets an instance of Visual Studio.
        /// </summary>
        public static IVisualStudio GetVisualStudio()
        {
            if (visualStudio == null)
                visualStudio = VisualStudioManager.Instance.LaunchVisualStudio(VisualStudioVersion.VS2008, NullLogger.Instance);

            return visualStudio;
        }

        public static Solution OpenSolutionIfNeeded()
        {
            string solutionPath = Path.Combine(Path.GetDirectoryName(
                AssemblyUtils.GetAssemblyLocalPath(typeof(VisualStudioTestHarness).Assembly)), @"..\TestSolution.sln");

            Solution solution = null;
            visualStudio.Call(dte =>
            {
                if (!dte.Solution.IsOpen)
                    dte.Solution.Open(solutionPath);

                solution = dte.Solution;
            });

            return solution;
        }

        [FixtureSetUp]
        public static void SetUp()
        {
            ComRetryMessageFilter.Install(TimeSpan.FromSeconds(30));
        }

        [FixtureTearDown]
        public static void TearDown()
        {
            try
            {
                if (visualStudio != null)
                {
                    visualStudio.Call(dte => dte.Quit());
                    visualStudio = null;
                }
            }
            finally
            {
                ComRetryMessageFilter.Uninstall();
            }
        }
    }
}
