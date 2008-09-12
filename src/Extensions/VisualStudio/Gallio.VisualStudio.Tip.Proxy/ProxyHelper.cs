using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Loader;
using Gallio.Runtime;
using Gallio.Runtime.Logging;

namespace Gallio.VisualStudio.Tip
{
    internal static class ProxyHelper
    {
        /// <summary>
        /// Gets the factory for constructing the target of the proxy.
        /// </summary>
        /// <returns>The target factory</returns>
        public static IProxyTargetFactory GetTargetFactory()
        {
            GallioLoader.Initialize().SetupRuntime();
            return ResolveTargetFactory();
        }

        private static IProxyTargetFactory ResolveTargetFactory()
        {
            return RuntimeAccessor.Instance.Resolve<IProxyTargetFactory>();
        }
    }
}
