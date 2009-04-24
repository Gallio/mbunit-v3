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
using Gallio.Collections;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A handler factory for singletons.
    /// </summary>
    public class SingletonHandlerFactory : IHandlerFactory
    {
        /// <inheritdoc />
        public IHandler CreateHandler(IServiceLocator serviceLocator, IResourceLocator resourceLocator, Type contractType, Type objectType, PropertySet properties)
        {
            if (! contractType.IsAssignableFrom(objectType))
                throw new RuntimeException(string.Format("Could not satisfy contract of type '{0}' by creating an instance of type '{1}'.",
                    contractType, objectType));

            var dependencyResolver = new DefaultObjectDependencyResolver(serviceLocator, resourceLocator);
            var objectFactory = new ObjectFactory(dependencyResolver, objectType, properties);
            return new SingletonHandler(objectFactory);
        }

        private sealed class SingletonHandler : IHandler
        {
            private readonly ObjectFactory objectFactory;
            private object instance;

            public SingletonHandler(ObjectFactory objectFactory)
            {
                this.objectFactory = objectFactory;
            }

            public object Activate()
            {
                if (instance == null)
                {
                    lock (this)
                    {
                        if (instance == null)
                            instance = objectFactory.CreateInstance();
                    }
                }

                return instance;
            }
        }
    }
}
