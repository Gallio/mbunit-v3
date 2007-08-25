using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base class for MbUnit-derived tests.
    /// </summary>
    public class MbUnitTest : BaseTest
    {
        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="templateBinding">The template binding that produced this test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/> or <paramref name="templateBinding"/> is null</exception>
        public MbUnitTest(string name, CodeReference codeReference, ITemplateBinding templateBinding)
            : base(name, codeReference, templateBinding)
        {
        }
    }
}