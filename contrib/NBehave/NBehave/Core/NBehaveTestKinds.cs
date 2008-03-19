using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;

namespace NBehave.Core
{
    /// <summary>
    /// Declares additional <see cref="TestKinds" /> for NBehave.
    /// </summary>
    public static class NBehaveTestKinds
    {
        /// <summary>
        /// The test declares a concern.
        /// </summary>
        public const string Concern = "Concern";

        /// <summary>
        /// The test declares a specification context.
        /// </summary>
        public const string Context = "Context";

        /// <summary>
        /// The test declares a specification.
        /// </summary>
        public const string Specification = "Specification";
    }
}
