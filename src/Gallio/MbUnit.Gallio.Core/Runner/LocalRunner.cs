using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.Harness;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// A local runner runs tests locally within the current AppDomain using
    /// a <see cref="LocalTestDomain" />.
    /// </summary>
    public class LocalRunner : BaseRunner
    {
        /// <summary>
        /// Creates a local runner using the current runtime stored in <see cref="RuntimeHolder.Instance" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="RuntimeHolder.Instance" /> is null</exception>
        public LocalRunner()
            : this(GetRuntime())
        {
        }

        /// <summary>
        /// Creates a local runner using the specified runtime.
        /// </summary>
        /// <param name="runtime">The runtime to use</param>
        public LocalRunner(ICoreRuntime runtime)
            : base(runtime, new LocalTestDomainFactory(runtime, runtime.Resolve<ITestHarnessFactory>()))
        {
        }

        private static ICoreRuntime GetRuntime()
        {
            ICoreRuntime runtime = RuntimeHolder.Instance as ICoreRuntime;
            if (runtime == null)
                throw new InvalidOperationException("The framework's runtime holder has not been initialized.");

            return runtime;
        }
    }
}
