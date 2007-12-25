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

using System.Reflection;
using Gallio.Reflection;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// The built-in assembly resolver only resolves assemblies that are part
    /// of the Gallio distribution or its plugins.  We deliberately do not
    /// try to resolve the test assemblies themselves because it would lock
    /// them in memory and prevent them from being recompiled.  We use
    /// <see cref="Assembly.Load(string)" /> to ensure that only assemblies that can
    /// be resolved using the currently configured assembly resolvers are
    /// loaded.
    /// </summary>
    public class BuiltInAssemblyResolver : IAssemblyResolver
    {
        public static readonly BuiltInAssemblyResolver Instance = new BuiltInAssemblyResolver();

        public Assembly ResolveAssembly(IAssemblyInfo assemblyInfo)
        {
            return Assembly.Load(assemblyInfo.FullName);
        }
    }
}
