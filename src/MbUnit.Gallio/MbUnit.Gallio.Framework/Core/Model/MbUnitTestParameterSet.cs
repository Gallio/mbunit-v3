using System;
using MbUnit.Framework.Model;

namespace MbUnit.Framework.Core.Model
{
    /// <summary>
    /// Represents a parameter set derived from an MbUnit fixture or test method.
    /// </summary>
    public class MbUnitTestParameterSet : BaseTestParameterSet
    {
        private MbUnitTestTemplate template;

        /// <summary>
        /// Initializes a test parameter set.
        /// </summary>
        /// <param name="template">The containing template</param>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition of the parameter set</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="codeReference"/> is null</exception>
        public MbUnitTestParameterSet(MbUnitTestTemplate template, string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            this.template = template;
        }

        /// <summary>
        /// Gets the containing test template.
        /// </summary>
        public MbUnitTestTemplate Template
        {
            get { return template; }
        }
    }
}
