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
using MbUnit.Collections;
using MbUnit.Core.RuntimeSupport;

namespace MbUnit.Tests.Core.RuntimeSupport
{
    /// <summary>
    /// A simple mock implementation of a <see cref="IRuntime" /> component
    /// registry.
    /// </summary>
    public class MockRuntime : IRuntime
    {
        private readonly MultiMap<Type, object> components;

        public MockRuntime()
        {
            components = new MultiMap<Type, object>();
        }

        public MultiMap<Type, object> Components
        {
            get { return components; }
        }

        public object Resolve(Type service)
        {
            return components[service][0];
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public T[] ResolveAll<T>()
        {
            return GenericUtils.ConvertAllToArray<object, T>(components[typeof(T)], delegate(object component)
            {
                return (T) component;
            });
        }

        public RuntimeSetup GetRuntimeSetup()
        {
            return new RuntimeSetup();
        }

        public string MapUriToLocalPath(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}