using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Services.Runtime
{
    /// <summary>
    /// Holds a static singleton reference to the runtime.
    /// </summary>
    public static class RuntimeHolder
    {
        private static IRuntime instance;

        /// <summary>
        /// Gets or sets the runtime instance.
        /// May be null if the runtime has not been initialized yet.
        /// </summary>
        public static IRuntime Instance
        {
            get { return instance; }
            set { instance = value; }
        }
    }
}
