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
