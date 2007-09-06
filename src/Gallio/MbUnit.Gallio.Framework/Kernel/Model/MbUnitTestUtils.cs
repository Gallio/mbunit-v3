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
using MbUnit.Framework.Properties;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Utility functions to help build MbUnit tests.
    /// </summary>
    public static class MbUnitTestUtils
    {
        /// <summary>
        /// Creates an action that invokes a method on the fixture without parameters.
        /// </summary>
        /// <param name="method">The method to invoke</param>
        /// <returns>The action</returns>
        public static Action<MbUnitTestState> CreateFixtureMethodInvoker(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException(@"method");

            return delegate(MbUnitTestState state)
            {
                if (method.IsStatic)
                {
                    method.Invoke(null, null);
                }
                else
                {
                    object instance = state.FixtureInstance;
                    if (instance == null)
                        throw new ModelException(Resources.MbUnitTestUtils_ExpectedToInvokeAnInstanceMethod);

                    method.Invoke(instance, null);
                }
            };
        }
    }
}
