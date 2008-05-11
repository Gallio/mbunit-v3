// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Castle.Core;
using Castle.MicroKernel;

namespace Gallio.Runtime.Windsor
{
    /// <summary>
    /// <para>
    /// Resolves arrays of dependencies.
    /// </para>
    /// <para>
    /// This code is courtesy of Hammett: http://hammett.castleproject.org/?p=257
    /// </para>
    /// </summary>
    internal class ArraySubDependencyResolver : ISubDependencyResolver
    {
        private readonly IKernel kernel;

        public ArraySubDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public object Resolve(CreationContext context, ISubDependencyResolver parentResolver,
            ComponentModel model, DependencyModel dependency)
        {
            Type serviceType = dependency.TargetType.GetElementType();
            IHandler[] handlers = kernel.GetHandlers(serviceType);
            Array result = Array.CreateInstance(serviceType, handlers.Length);

            for (int i = 0; i < handlers.Length; i++)
                result.SetValue(handlers[i].Resolve(context), i);

            return result;

            // Missing from RC3.
            // return kernel.ResolveAll(dependency.TargetType.GetElementType(), null);
        }

        public bool CanResolve(CreationContext context, ISubDependencyResolver parentResolver,
            ComponentModel model, DependencyModel dependency)
        {
            return dependency.TargetType != null
                && dependency.TargetType.IsArray
                    && dependency.TargetType.GetElementType().IsInterface;
        }
    }
}