using System;
using System.Reflection;

namespace MbUnit.Core.Reflection
{
    using MbUnit.Framework;

    public class SignatureChecker
    {
        private Type returnType;
        private Type[] parameters;

        public SignatureChecker(Type returnType, params Type[] parameters)
        {
            this.returnType = returnType;
            this.parameters = parameters;
        }

        public void Check(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            try
            {
                ReflectionAssert.IsAssignableFrom(this.returnType, method.ReturnType);
                ParameterInfo[] pis = method.GetParameters();
                Assert.AreEqual(this.parameters.Length, pis.Length,
                    "Parameters count are not equal");
                for (int i = 0; i < pis.Length; ++i)
                {
                    Assert.AreEqual(this.parameters[i], pis[i].ParameterType,
                        "Parameter {0} is not of the same type");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid method signature", ex);
            }
        }
    }
}