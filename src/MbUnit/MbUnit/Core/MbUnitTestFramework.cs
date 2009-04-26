using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Core
{
    /// <summary>
    /// The MbUnit test framework.
    /// </summary>
    public class MbUnitTestFramework : PatternTestFramework
    {
        /// <inheritdoc />
        protected override IEnumerable<PatternTestFrameworkExtensionInfo> GetExtensions(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, "MbUnit");
            if (frameworkAssemblyName == null)
                yield break;

            yield return new PatternTestFrameworkExtensionInfo("MbUnit v3",
                String.Format("MbUnit v{0}", AssemblyUtils.GetApplicationVersion(Assembly.GetExecutingAssembly())));
        }
    }
}
