using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mono.Cecil;

namespace MbUnit.Plugin.CecilInstrumentation
{
    internal static class CecilUtils
    {
        public static bool IsVoid(TypeReference typeReference)
        {
            return typeReference.FullName == "System.Void";
        }

        public static CallingConventions GetCallingConventions(MethodReference methodReference)
        {
            MethodCallingConvention c = (MethodCallingConvention) ((int) methodReference.CallingConvention & 0x0f);
            switch (c)
            {
                case MethodCallingConvention.ThisCall:
                    return CallingConventions.HasThis;
                case MethodCallingConvention.VarArg:
                    return CallingConventions.VarArgs;
                default:
                    return CallingConventions.Standard;
            }
        }
    }
}
