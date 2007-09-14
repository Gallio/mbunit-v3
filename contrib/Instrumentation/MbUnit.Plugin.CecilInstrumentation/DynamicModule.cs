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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Describes a module in a dynamic assembly.
    /// </summary>
    internal sealed class DynamicModule
    {
        private readonly ModuleBuilder builder;
        private readonly DynamicAssembly assembly;

        public DynamicModule(ModuleBuilder builder, DynamicAssembly assembly)
        {
            this.builder = builder;
            this.assembly = assembly;
        }

        public ModuleBuilder Builder
        {
            get { return builder; }
        }

        public DynamicAssembly Assembly
        {
            get { return assembly; }
        }

        public DynamicType DefineType(string name, TypeAttributes attr, Type parent, PackingSize packingSize, int typesize)
        {
            TypeBuilder typeBuilder = builder.DefineType(name, attr, parent, packingSize, typesize);
            DynamicType type = new DynamicType(typeBuilder, this);
            return type;
        }
    }
}
