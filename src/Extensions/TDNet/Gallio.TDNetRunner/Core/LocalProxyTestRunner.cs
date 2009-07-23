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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Loader;
using Gallio.TDNetRunner.Facade;
using Gallio.TDNetRunner.Properties;

namespace Gallio.TDNetRunner.Core
{
    public class LocalProxyTestRunner : BaseProxyTestRunner
    {
        private IProxyTestRunner testRunner;

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (testRunner != null)
            {
                if (disposing)
                    testRunner.Dispose();

                testRunner = null;
            }
        }

        /// <inheritdoc />
        protected override FacadeTestRunState RunImpl(IFacadeTestListener testListener, string assemblyPath, string cref, FacadeOptions facadeOptions)
        {
            string versionLabel = VersionPolicy.GetVersionLabel(Assembly.GetExecutingAssembly());

            testListener.WriteLine(String.Format(Resources.RunnerNameAndVersion + "\n", versionLabel), FacadeCategory.Info);

            switch (facadeOptions.FilterCategoryMode)
            {
                case FacadeFilterCategoryMode.Include:
                    testListener.WriteLine(String.Format("Included categories: {0}\n", CombineCategoryNameList(facadeOptions.FilterCategoryNames)), FacadeCategory.Info);
                    break;

                case FacadeFilterCategoryMode.Exclude:
                    testListener.WriteLine(String.Format("Excluded categories: {0}\n", CombineCategoryNameList(facadeOptions.FilterCategoryNames)), FacadeCategory.Info);
                    break;
            }

            EnsureTestRunnerIsCreated();
            return testRunner.Run(testListener, assemblyPath, cref, facadeOptions);
        }

        private static string CombineCategoryNameList(IList<string> categoryNames)
        {
            if (categoryNames.Count == 0)
                return "<none>";

            StringBuilder result = new StringBuilder();
            foreach (string categoryName in categoryNames)
            {
                if (result.Length != 0)
                    result.Append(", ");

                result.Append(categoryName);
            }

            return result.ToString();
        }

        /// <inheritdoc />
        protected override void AbortImpl()
        {
            if (testRunner != null)
                testRunner.Abort();
        }

        internal virtual IProxyTestRunner CreateRemoteProxyTestRunner()
        {
            try
            {
                IGallioRemoteEnvironment environment = EnvironmentManager.GetSharedEnvironment();

                AppDomain.CurrentDomain.AssemblyResolve += ResolveRunnerAssembly;

                Type runnerType = typeof(RemoteProxyTestRunner);
                object runner = environment.AppDomain.CreateInstanceAndUnwrap(runnerType.Assembly.FullName, runnerType.FullName);

                return (IProxyTestRunner)runner;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= ResolveRunnerAssembly;
            }
        }

        private void EnsureTestRunnerIsCreated()
        {
            if (testRunner == null)
                testRunner = CreateRemoteProxyTestRunner();
        }

        /// <summary>
        /// This resolver is used to ensure that we can cast the test runner's transparent proxy to IGallioTestRunner.
        /// </summary>
        /// <remarks>
        /// <para>
        /// TestDriven.Net initially loaded this assembly using Assembly.LoadFrom.  When the cast occurs, the runtime implicitly
        /// tries to load the interface using Assembly.Load by fullname which does not normally consider anything loaded with LoadFrom.
        /// So we introduce a resolver that recognizes when we are attempting to load this assembly by fullname and
        /// just returns it.  Without it, an InvalidCastException will occur.
        /// </para>
        /// </remarks>
        private static Assembly ResolveRunnerAssembly(object sender, ResolveEventArgs e)
        {
            Assembly runnerAssembly = typeof(LocalProxyTestRunner).Assembly;
            return e.Name == runnerAssembly.FullName ? runnerAssembly : null;
        }
    }
}
