// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
