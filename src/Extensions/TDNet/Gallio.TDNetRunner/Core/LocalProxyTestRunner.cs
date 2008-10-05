using System;
using System.Reflection;
using Gallio.Loader;
using Gallio.TDNetRunner.Facade;
using Gallio.TDNetRunner.Properties;

namespace Gallio.TDNetRunner.Core
{
    public class LocalProxyTestRunner : IProxyTestRunner
    {
        private IProxyTestRunner testRunner;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (testRunner != null)
            {
                if (disposing)
                    testRunner.Dispose();

                testRunner = null;
            }
        }

        /// <inheritdoc />
        public FacadeTestRunState Run(IFacadeTestListener testListener, string assemblyPath, string cref)
        {
            Version appVersion = Assembly.GetCallingAssembly().GetName().Version;
            testListener.WriteLine(String.Format(Resources.RunnerNameAndVersion + "\n",
                appVersion.Major, appVersion.Minor, appVersion.Build, appVersion.Revision), FacadeCategory.Info);

            EnsureTestRunnerIsCreated();
            return testRunner.Run(testListener, assemblyPath, cref);
        }

        /// <inheritdoc />
        public void Abort()
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
        /// <para>
        /// This resolver is used to ensure that we can cast the test runner's transparent proxy to IGallioTestRunner.
        /// </para>
        /// <para>
        /// TestDriven.Net initially loaded this assembly using Assembly.LoadFrom.  When the cast occurs, the runtime implicitly
        /// tries to load the interface using Assembly.Load by fullname which does not normally consider anything loaded with LoadFrom.
        /// So we introduce a resolver that recognizes when we are attempting to load this assembly by fullname and
        /// just returns it.  Without it, an InvalidCastException will occur.
        /// </para>
        /// </summary>
        private static Assembly ResolveRunnerAssembly(object sender, ResolveEventArgs e)
        {
            Assembly runnerAssembly = typeof(LocalProxyTestRunner).Assembly;
            return e.Name == runnerAssembly.FullName ? runnerAssembly : null;
        }
    }
}
