using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace MbUnit.Plugin.CecilInstrumentation
{
    /// <summary>
    /// Describes a module in a dynamic assembly.
    /// </summary>
    internal class DynamicConstructor
    {
        private readonly ConstructorBuilder builder;
        private readonly Type[] parameterTypes;

        public DynamicConstructor(ConstructorBuilder builder, Type[] parameterTypes)
        {
            this.builder = builder;
            this.parameterTypes = parameterTypes;
        }

        public ConstructorBuilder Builder
        {
            get { return builder; }
        }

        public Type[] ParameterTypes
        {
            get { return parameterTypes; }
        }
    }
}
