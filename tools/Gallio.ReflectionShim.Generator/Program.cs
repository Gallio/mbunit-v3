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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Gallio.ReflectionShim.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: [key file path] [output directory]");
            }

            StrongNameKeyPair keyPair;
            using (FileStream keyFile = File.OpenRead(args[0]))
                keyPair = new StrongNameKeyPair(keyFile);

            AssemblyName assemblyName = new AssemblyName("Gallio.ReflectionShim");
            assemblyName.KeyPair = keyPair;

            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, args[1]);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Gallio.ReflectionShim.dll");

            CreateShimType(moduleBuilder, "Gallio.Common.Reflection.Impl.AssemblyShim", typeof(Assembly)).CreateType();
            CreateShimType(moduleBuilder, "Gallio.Common.Reflection.Impl.ModuleShim", typeof(Module)).CreateType();

            assemblyBuilder.Save("Gallio.ReflectionShim.dll");
        }

        private static TypeBuilder CreateShimType(ModuleBuilder moduleBuilder, string typeName, Type superClass)
        {
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Abstract | TypeAttributes.Public, superClass);
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[0]);
            constructor.GetILGenerator().Emit(OpCodes.Ret);

            ConstructorInfo clsCompliantAttribConstructor = typeof(CLSCompliantAttribute).GetConstructor(new[] { typeof(bool) });
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(clsCompliantAttribConstructor, new object[] { true }));
            return typeBuilder;
        }
    }
}
