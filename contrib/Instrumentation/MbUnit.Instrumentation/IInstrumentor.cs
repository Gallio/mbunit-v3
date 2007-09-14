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
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;

namespace MbUnit.Instrumentation
{
    /// <summary>
    /// An instrumentor loads assemblies in such a way that arbitrary instrumentation
    /// can be added in the form of method interception.
    /// </summary>
    public interface IInstrumentor
    {
        /// <summary>
        /// Loads an assembly from a file in such a way that it can be instrumented.
        /// </summary>
        /// <param name="assemblyPath">The path of the assembly file to load</param>
        /// <param name="instrumentedAssemblySavePath">If not null, saves a copy
        /// of the instrumented assembly to the specified path</param>
        /// <returns>The loaded assembly</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyPath"/> is null</exception>
        Assembly InstrumentAndLoad(string assemblyPath, string instrumentedAssemblySavePath);

        /// <summary>
        /// Adds an interceptor to a method.
        /// </summary>
        /// <param name="method">The method to intercept</param>
        /// <param name="interceptor">The interceptor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> or
        /// <paramref name="interceptor"/> is null</exception>
        void AddInterceptor(MethodInfo method, IInterceptor interceptor);

        /// <summary>
        /// Removes an interceptor from a method.
        /// </summary>
        /// <param name="method">The method to intercept</param>
        /// <param name="interceptor">The interceptor</param>
        /// <returns>True if an interceptor was removed</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> or
        /// <paramref name="interceptor"/> is null</exception>
        bool RemoveInterceptor(MethodInfo method, IInterceptor interceptor);
    }
}
