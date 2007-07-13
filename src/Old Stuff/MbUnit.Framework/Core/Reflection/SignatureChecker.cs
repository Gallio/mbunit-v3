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