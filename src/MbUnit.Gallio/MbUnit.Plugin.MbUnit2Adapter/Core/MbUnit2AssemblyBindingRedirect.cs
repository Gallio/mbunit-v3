// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Core.Runner;

namespace MbUnit.Plugin.MbUnit2Adapter.Core
{
    /// <summary>
    /// Contributes a binding redirect for the MbUnit v2 assemblies.
    /// This ensures that we can run MbUnit v2 tests even if the version of MbUnit
    /// they were built against differs from the plugin so long as no breaking API
    /// changes are encountered.
    /// </summary>
    public class MbUnit2AssemblyBindingRedirect : IIsolatedTestDomainContributor
    {
        /// <inheritdoc />
        public void Apply(IsolatedTestDomain domain)
        {
            Assembly frameworkAssembly = typeof(MbUnit2::MbUnit.Framework.Assert).Assembly;
            domain.BootstrapAssemblies.Add(frameworkAssembly, true);
        }
    }
}
