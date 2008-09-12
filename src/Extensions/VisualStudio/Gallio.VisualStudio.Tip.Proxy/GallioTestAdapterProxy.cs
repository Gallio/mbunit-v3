using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using Gallio.Loader;
using Microsoft.VisualStudio.TestTools.Execution;
using Microsoft.VisualStudio.TestTools.TestAdapter;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Proxies the <see cref="ITestAdapter" /> interface over to the actual implementation
    /// after initializing the loader.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation actually loads up Gallio within a fresh AppDomain because the
    /// test adapter is called from a context that includes the test assemblies within
    /// its ApplicationBase.  That becomes a problem if the test assemblies are linked to
    /// a different version of Gallio.dll than that of the installed runtime.  The resulting
    /// conflict produces hard to debug problems related to types being loaded
    /// multiple times from different locations.
    /// </para>
    /// <para>
    /// So we sidestep this whole mess at least while trying to get Gallio off the ground.
    /// Then Gallio takes its own ordinary measures to cope with any version conflicts.
    /// </para>
    /// </remarks>
    public class GallioTestAdapterProxy : MarshalByRefTestAdapterProxy
    {
        private static readonly object runnerAppDomainSyncRoot = new object();
        private static AppDomain runnerAppDomain;

        public GallioTestAdapterProxy()
            : base(CreateRemoteShim())
        {
        }

        private static IShim CreateRemoteShim()
        {
            PrepareRunnerAppDomain();

            IShim shim = (IShim)runnerAppDomain.CreateInstanceFromAndUnwrap(typeof(Shim).Assembly.Location, typeof(Shim).FullName);
            shim.AddHintDirectory(Path.GetDirectoryName(typeof(IRunContext).Assembly.Location));
            return shim;
        }

        private static void PrepareRunnerAppDomain()
        {
            lock (runnerAppDomainSyncRoot)
            {
                if (runnerAppDomain != null)
                    return;

                string runtimePath = GallioLoader.GetDefaultRuntimePath();
                AppDomainSetup appDomainSetup = new AppDomainSetup();
                appDomainSetup.ApplicationName = "Gallio";
                appDomainSetup.ApplicationBase = runtimePath;
                Evidence evidence = AppDomain.CurrentDomain.Evidence;
                PermissionSet defaultPermissionSet = new PermissionSet(PermissionState.Unrestricted);
                StrongName[] fullTrustAssemblies = new StrongName[0];
                runnerAppDomain = AppDomain.CreateDomain(appDomainSetup.ApplicationName, evidence, appDomainSetup, defaultPermissionSet, fullTrustAssemblies);
            }
        }

        private interface IShim : ITestAdapter
        {
            void AddHintDirectory(string path);
        }

        private sealed class Shim : MarshalByRefTestAdapterProxy, IShim
        {
            public Shim()
                : base(CreateTarget())
            {
            }

            public void AddHintDirectory(string path)
            {
                GallioLoader.Instance.AddHintDirectory(path);
            }

            private static ITestAdapter CreateTarget()
            {
                return ProxyHelper.GetTargetFactory().CreateTestAdapter();
            }
        }
    }
}
