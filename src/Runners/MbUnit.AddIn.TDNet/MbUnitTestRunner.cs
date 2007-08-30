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
using MbUnit.Core.Runner;
using TestDriven.Framework;
using TDF = TestDriven.Framework;

namespace MbUnit.AddIn.TDNet
{
    /// <summary>
    /// MbUnit test runner for TestDriven.Net.
    /// </summary>
    [Serializable]
    public class MbUnitTestRunner : TDF.ITestRunner
    {
        #region TDF.ITestRunner Members

        TestRunState TDF.ITestRunner.RunAssembly(ITestListener testListener, Assembly assembly)
        {
            return Run(testListener, assembly, "");
        }

        TestRunState TDF.ITestRunner.RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            if (member == null)
                throw new ArgumentNullException("member");
            string filter = "Member=" + member.Name;
            testListener.WriteLine(filter, Category.Info);
            return Run(testListener, assembly, filter);
        }

        TestRunState TDF.ITestRunner.RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            string filter = "Type=" + ns;
            testListener.WriteLine(filter, Category.Info);
            return Run(testListener, assembly, filter);
        }

        #endregion

        private static TestRunState Run(ITestListener listener, Assembly assembly, string filter)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");

            TDDLogger logger = new TDDLogger(listener);
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            logger.Info(String.Format("MbUnit.AddIn - Version {0}.{1} build {2}", appVersion.Major, appVersion.Minor, appVersion.Build));
            using (TestRunnerHelper testRunnerHelper = new TestRunnerHelper
                (
                delegate { return new RunnerProgressMonitor(logger); },
                logger,
                filter
                ))
            {
                string location = new Uri(assembly.CodeBase).LocalPath;
                testRunnerHelper.Package.AssemblyFiles.Add(location);

                int result = testRunnerHelper.Run();
                return GetTddResult(result);
            }
        }

        private static TestRunState GetTddResult(int result)
        {
            switch(result)
            {
                case ResultCode.FatalException:
                case ResultCode.InvalidArguments:
                    return TestRunState.Error;
                case ResultCode.Failure:
                    return TestRunState.Failure;
                case ResultCode.NoTests:
                    return TestRunState.NoTests;
                default:
                    return TestRunState.Success;
            }
        }
    }
}