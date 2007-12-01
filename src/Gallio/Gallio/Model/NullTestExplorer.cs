using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A null test explorer.
    /// </summary>
    public class NullTestExplorer : ITestExplorer
    {
        /// <inheritdoc />
        public void Reset()
        {
        }

        /// <inheritdoc />
        public bool IsTest(ICodeElementInfo element)
        {
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<ITest> ExploreType(ITypeInfo type)
        {
            return EmptyArray<ITest>.Instance;
        }

        /// <inheritdoc />
        public IEnumerable<ITest> ExploreAssembly(IAssemblyInfo assembly)
        {
            return EmptyArray<ITest>.Instance;
        }
    }
}
