using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MbUnit.Model
{
    /// <summary>
    /// Abstract base class for test framework implementations.
    /// </summary>
    public abstract class BaseTestFramework : ITestFramework
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public virtual void PrepareAssemblies(IList<Assembly> assemblies)
        {
        }

        /// <inheritdoc />
        public virtual void BuildTemplates(TemplateTreeBuilder builder, IList<Assembly> assemblies)
        {
        }
    }
}
