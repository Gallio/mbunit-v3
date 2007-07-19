using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The root test in the test tree.
    /// </summary>
    public class RootTest : BaseTest
    {
        /// <summary>
        /// Creates the root test.
        /// </summary>
        public RootTest()
            : base("Root", CodeReference.Unknown, null)
        {
            // Note: The kind will be set by the RootTemplateBinding.
            Kind = null;
        }
    }
}
