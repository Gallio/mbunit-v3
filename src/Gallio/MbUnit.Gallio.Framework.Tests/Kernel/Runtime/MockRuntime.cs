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
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Framework.Tests.Kernel.Runtime
{
    public class MockRuntime : IRuntime
    {
        private MultiMap<Type, object> components;

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
            return ListUtils.ConvertAllToArray<object, T>(components[typeof(T)], delegate(object component)
            {
                return (T) component;
            });
        }

        public void Dispose()
        {
        }
    }
}
