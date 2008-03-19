using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace NBehave.Core
{
    /// <summary>
    /// A <see cref="IPatternTestFrameworkExtension"/> that registers MbUnit as a tool
    /// when the test assembly contains a reference to the MbUnit assembly.
    /// </summary>
    public class NBehaveTestFrameworkExtension : BasePatternTestFrameworkExtension
    {
        /// <inheritdoc />
        public override IEnumerable<ToolInfo> GetReferencedTools(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, "NBehave");
            if (frameworkAssemblyName == null)
                yield break;

            yield return new ToolInfo("NBehave", String.Format("NBehave v{0}", frameworkAssemblyName.Version));
        }
    }
}
