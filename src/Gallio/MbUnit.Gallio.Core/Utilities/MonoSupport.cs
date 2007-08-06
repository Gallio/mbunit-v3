using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Utilities
{
    /// <summary>
    /// Provides helpers for supporting mono.
    /// </summary>
    public static class MonoSupport
    {
        /// <summary>
        /// Returns true if the application is running within the Mono runtime.
        /// </summary>
        public static bool IsUsingMonoRuntime
        {
            get { return Type.GetType("Mono.Runtime") != null; }
        }
    }
}
