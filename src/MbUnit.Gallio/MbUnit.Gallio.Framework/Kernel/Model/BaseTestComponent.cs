using System;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestComponent" />.
    /// </summary>
    public class BaseTestComponent : BaseModelComponent, ITestComponent
    {
        /// <summary>
        /// Initializes a component initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public BaseTestComponent(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
        }
    }
}