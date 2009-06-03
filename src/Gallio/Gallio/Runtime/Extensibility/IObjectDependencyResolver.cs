// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Resolves object dependencies and parameters.
    /// </summary>
    public interface IObjectDependencyResolver
    {
        /// <summary>
        /// Resolves a dependency.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="parameterType">The parameter type.</param>
        /// <param name="configurationArgument">An optional configuration argument that supplies a value
        /// for the parameter or describes the means by which the value should be obtained, or null if none.</param>
        /// <returns>An object that describes whether the dependency was satisfied and the value it was assigned.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameterName"/> or 
        /// <paramref name="parameterType"/> is null.</exception>
        DependencyResolution ResolveDependency(string parameterName, Type parameterType, string configurationArgument);
    }
}
