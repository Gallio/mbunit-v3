using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Hosting
{
    /// <summary>
    /// Creates instances of the <see cref="WindsorRuntime" />.
    /// </summary>
    public class WindsorRuntimeFactory : IRuntimeFactory
    {
        /// <inheritdoc />
        public IRuntime CreateRuntime(RuntimeSetup setup)
        {
            return new WindsorRuntime(new DefaultAssemblyResolverManager(), setup);
        }
    }
}
