// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Text;
using Gallio.Collections;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// <para>
    /// Provides helpers for working with properties.
    /// </para>
    /// <para>
    /// This class is intended to assist with the implementation of new
    /// reflection policies.  It should not be used directly by clients of the
    /// reflection API.
    /// </para>
    /// </summary>
    public static class ReflectorPropertyUtils
    {
        /// <summary>
        /// Gets a list of index parameters for a property based on the parameter
        /// lists of the associated get or set method.
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>The list of index parameters</returns>
        public static IList<IParameterInfo> GetIndexParameters(IPropertyInfo property)
        {
            IMethodInfo getMethod = property.GetMethod;
            if (getMethod != null)
                return getMethod.Parameters;

            IList<IParameterInfo> setterParameters = property.SetMethod.Parameters;
            if (setterParameters.Count == 1)
                return EmptyArray<IParameterInfo>.Instance;

            IParameterInfo[] parameters = new IParameterInfo[setterParameters.Count - 1];
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = setterParameters[i];
            return parameters;
        }
    }
}
