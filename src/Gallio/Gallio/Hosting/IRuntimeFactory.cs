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
using System.Text;
using Castle.Core;

namespace Gallio.Hosting
{
    /// <summary>
    /// Creates an instance of <see cref="IRuntime" /> given
    /// the runtime setup options.
    /// </summary>
    public interface IRuntimeFactory
    {
        /// <summary>
        /// Creates a <see cref="IRuntime" /> given setup options.
        /// </summary>
        /// <remarks>
        /// The runtime's <see cref="IInitializable.Initialize" /> method
        /// will be called on the instance returned to perform any additional
        /// deferred initialization once the global runtime <see cref="Runtime.Instance" />
        /// property has been set.
        /// </remarks>
        /// <param name="setup">The runtime setup options, never null</param>
        /// <returns>The runtime</returns>
        IRuntime CreateRuntime(RuntimeSetup setup);
    }
}
