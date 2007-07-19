extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Core.Runner;

namespace MbUnit.Plugin.MbUnit2Adapter.Core
{
    /// <summary>
    /// Contributes a binding redirect for the MbUnit v2 assemblies.
    /// This ensures that we can run MbUnit v2 tests even if the version of MbUnit
    /// they were built against differs from the plugin so long as no breaking API
    /// changes are encountered.
    /// </summary>
    public class MbUnit2AssemblyBindingRedirect : IIsolatedTestDomainContributor
    {
        /// <inheritdoc />
        public void Apply(IsolatedTestDomain domain)
        {
            Assembly frameworkAssembly = typeof(MbUnit2::MbUnit.Framework.Assert).Assembly;
            domain.BootstrapAssemblies.Add(frameworkAssembly, true);
        }
    }
}
