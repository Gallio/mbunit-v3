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
    /// A handler factory for pre-manufactured instances.
    /// </summary>
    public class InstanceHandlerFactory : IHandlerFactory
    {
        private readonly object instance;

        /// <summary>
        /// Creates an instance handler factory given a particular pre-manufactured instance.
        /// </summary>
        /// <param name="instance">The instance to be returned by the handler</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="instance"/> is null</exception>
        public InstanceHandlerFactory(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            this.instance = instance;
        }

        /// <inheritdoc />
        public IHandler CreateHandler(IServiceLocator serviceLocator, IResourceLocator resourceLocator, Type contractType, Type objectType, PropertySet properties)
        {
            if (! contractType.IsInstanceOfType(instance))
                throw new RuntimeException(string.Format("Could not satisfy contract of type '{0}' using pre-manufactured instance of type '{1}'.",
                    contractType, instance.GetType()));

            return new Handler(instance);
        }

        private sealed class Handler : IHandler
        {
            private readonly object instance;

            public Handler(object instance)
            {
                this.instance = instance;
            }

            public object Activate()
            {
                return instance;
            }
        }
    }
}
