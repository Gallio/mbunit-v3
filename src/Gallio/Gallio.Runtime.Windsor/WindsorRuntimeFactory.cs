// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Loader;
using Gallio.Runtime.Windsor;

namespace Gallio.Runtime.Windsor
{
    /// <summary>
    /// Creates instances of the <see cref="WindsorRuntime" />.
    /// </summary>
    [Serializable]
    public sealed class WindsorRuntimeFactory : RuntimeFactory
    {
        /// <summary>
        /// Gets the singleton instance of the Windsor runtime factory.
        /// </summary>
        public static readonly WindsorRuntimeFactory Instance = new WindsorRuntimeFactory();

        private WindsorRuntimeFactory()
        {
        }

        /// <inheritdoc />
        protected override IRuntime CreateRuntimeInternal(RuntimeSetup runtimeSetup)
        {
            return new WindsorRuntime(new DefaultAssemblyResolverManager(), runtimeSetup);
        }
    }
}