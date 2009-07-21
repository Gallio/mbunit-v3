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
using Gallio.Common.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handler factory creates handlers for activating plugin, component and traits instances.
    /// </summary>
    public interface IHandlerFactory
    {
        /// <summary>
        /// Creates a handler.
        /// </summary>
        /// <param name="dependencyResolver">The object dependency resolver, not null.</param>
        /// <param name="contractType">The contract type: the handler will ensure that the objects it produces are subclasses or implementations of the contract type.</param>
        /// <param name="objectType">The object type: the handler will produce objects that are (possibly decorated) instances of the object type.</param>
        /// <param name="properties">The configuration properties for the objects produced by the handler.</param>
        /// <returns>The handler.</returns>
        IHandler CreateHandler(IObjectDependencyResolver dependencyResolver, Type contractType, Type objectType, PropertySet properties);
    }
}
