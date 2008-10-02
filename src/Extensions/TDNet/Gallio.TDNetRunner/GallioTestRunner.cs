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

using System;
using System.Reflection;
using Gallio.Loader;
using Gallio.TDNetRunner.Core;
using TestDriven.TestRunner.Framework;

namespace Gallio.TDNetRunner
{
    /// <summary>
    /// Gallio test runner for TestDriven.NET.
    /// </summary>
    /// <remarks>
    /// This class deliberately does not depend on any Gallio types outside of this
    /// test runner assembly.  That's because the AppDomain the test runner is loaded
    /// in also includes test assemblies which could contain a different version of
    /// Gallio.  So we create our own AppDomain so that Gallio itself can deal with
    /// the version conflicts.  Then we take some care not to directly refer to
    /// TestDriven.Net types within our own AppDomain so we don't need to load TestDriven.Net
    /// types.
    /// </remarks>
    [Serializable]
    public class GallioTestRunner : ITestRunner, IDisposable
    {
        internal readonly static string testRunnerName = typeof(GallioTestRunner).FullName;

        private IProxyTestRunner testRunner;

        /// <summary>
        /// Initializes the gallio test runner.
        /// </summary>
        public GallioTestRunner()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (testRunner != null)
            {
                testRunner.Dispose();
                testRunner = null;
            }
        }

        /// <inheritdoc />
        public TestRunResult Run(ITestListener testListener, ITraceListener traceListener, string assemblyPath, string testPath)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (traceListener == null)
                throw new ArgumentNullException("traceListener");
            if (assemblyPath == null)
                throw new ArgumentNullException("assemblyPath");

            GetTestRunner();

            AdapterProxyTestListener proxyTestListner = new AdapterProxyTestListener(testListener, traceListener);
            ProxyTestResult result = testRunner.Run(proxyTestListner, assemblyPath, testPath);

            return ToTestRunResult(result);
        }

        /// <inheritdoc />
        public void Abort()
        {
            if (testRunner != null)
            {
                testRunner.Abort();
            }
        }

        internal virtual IProxyTestRunner CreateTestRunner()
        {
            try
            {
                IGallioRemoteEnvironment environment = EnvironmentManager.GetSharedEnvironment();

                Type runnerType = typeof(RemoteProxyTestRunner);
                return (IProxyTestRunner) environment.AppDomain.CreateInstanceAndUnwrap(
                    runnerType.Assembly.FullName, runnerType.FullName);
            }
            catch (InvalidCastException ex)
            {
                throw new ApplicationException("The Gallio test runner was unable to obtain a proxy for the test runner.  This usually happens because we are trying to run tests that are themselves linked to a Gallio.TDNetRunner assembly in a different folder than is registered with TestDriven.Net.", ex);
            }
        }

        private void GetTestRunner()
        {
            if (testRunner == null)
                testRunner = CreateTestRunner();
        }

        private static TestRunResult ToTestRunResult(ProxyTestResult result)
        {
            if (result == null)
                return null;

            return new TestRunResult()
            {
                IsExecuted = result.IsExecuted,
                IsFailure = result.IsFailure,
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Name = result.Name,
                StackTrace = result.StackTrace,
                TestRunner = testRunnerName,
                TotalTests = result.TotalTests
            };
        }
    }
}