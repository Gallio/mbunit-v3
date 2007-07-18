using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Collections;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Framework.Services.Runtime;

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
