// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Core
{
    /// <summary>
    /// A <see cref="IPatternTestFrameworkExtension"/> that registers MbUnit as a tool
    /// when the test assembly contains a reference to the MbUnit assembly.
    /// </summary>
    public class MbUnitTestFrameworkExtension : BasePatternTestFrameworkExtension
    {
        /// <inheritdoc />
        public override IEnumerable<ToolInfo> GetReferencedTools(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, "MbUnit");
            if (frameworkAssemblyName == null)
                yield break;

            yield return new ToolInfo("MbUnit", String.Format("MbUnit v{0}", frameworkAssemblyName.Version));
        }
    }
}
