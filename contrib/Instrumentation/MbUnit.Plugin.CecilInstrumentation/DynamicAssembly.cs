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
using System.Reflection.Emit;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Describes a dynamic assembly.
    /// </summary>
    internal sealed class DynamicAssembly
    {
        private readonly AssemblyBuilder builder;
        private readonly Dictionary<Type, DynamicType> dynamicTypes;
        private readonly string filename;

        public DynamicAssembly(AssemblyBuilder builder, string filename)
        {
            this.builder = builder;
            this.filename = filename;

            dynamicTypes = new Dictionary<Type, DynamicType>();
        }

        public AssemblyBuilder Builder
        {
            get { return builder; }
        }

        public DynamicType GetDynamicType(Type type)
        {
            // FIXME: Not quite right...
            while (type.HasElementType)
                type = type.GetElementType();
            if (type.IsGenericType)
                type = type.GetGenericTypeDefinition();

            DynamicType dynamicType;
            dynamicTypes.TryGetValue(type, out dynamicType);
            return dynamicType;
        }

        public void RegisterDynamicType(TypeBuilder typeBuilder, DynamicType dynamicType)
        {
            dynamicTypes.Add(typeBuilder, dynamicType);
        }

        public void CreateAllTypes()
        {
            foreach (TypeBuilder typeBuilder in dynamicTypes.Keys)
                typeBuilder.CreateType();
        }

        public DynamicModule DefineDynamicModule(string name)
        {
            ModuleBuilder moduleBuilder = builder.DefineDynamicModule(name, filename);
            DynamicModule module = new DynamicModule(moduleBuilder, this);
            return module;
        }

        public void Save()
        {
            builder.Save(filename);
        }
    }
}
