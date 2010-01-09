// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Policies;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Core
{
    /// <summary>
    /// The MbUnit test framework.
    /// </summary>
    public class MbUnitTestFramework : PatternTestFramework
    {
        internal const string AssemblyKind = "MbUnit v3 Assembly";

        /// <summary>
        /// Gets the MbUnit version label.
        /// </summary>
        /// <value>A string similar to "MbUnit v3.1 build 200".</value>
        public static string VersionLabel
        {
            get { return String.Format("MbUnit v{0}", VersionPolicy.GetVersionLabel(Assembly.GetExecutingAssembly())); }
        }

        /// <inheritdoc />
        public override IEnumerable<PatternTestFrameworkExtensionInfo> GetExtensions(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, "MbUnit");
            if (frameworkAssemblyName == null)
                yield break;

            yield return new PatternTestFrameworkExtensionInfo("MbUnit v3", VersionLabel)
                {
                    AssemblyKind = "MbUnit v3 Assembly"
                };
        }
    }
}
