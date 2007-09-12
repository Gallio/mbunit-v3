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
