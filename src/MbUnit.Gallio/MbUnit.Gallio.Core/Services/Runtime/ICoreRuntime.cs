using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Services.Runtime
{
    /// <summary>
    /// An additional interface implemented by runtimes that is only
    /// visible to the core.  This interface provides services that the
    /// MbUnit framework shouldn't know about.
    /// </summary>
    public interface ICoreRuntime : IRuntime
    {
        /// <summary>
        /// Gets a deep copy of the runtime setup used to configure this runtime.
        /// </summary>
        /// <returns>The runtime setup</returns>
        RuntimeSetup GetRuntimeSetup();
    }
}
