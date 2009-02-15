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
using Gallio.Runtime;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime
{
    [TestFixture]
    [TestsOn(typeof(RuntimeRegisteredComponentResolver<>))]
    public class RuntimeRegisteredComponentResolverTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenRuntimeArgumentIsNull()
        {
            new RuntimeRegisteredComponentResolver<DummyRegisteredComponent>(null);
        }

        [Test]
        public void GetNamesDelegatesToTheRuntime()
        {
            IRuntime runtime = Mocks.StrictMock<IRuntime>();

            using (Mocks.Record())
            {
                Expect.Call(runtime.ResolveAll<DummyRegisteredComponent>()).Return(new DummyRegisteredComponent[]
                {
                    new DummyRegisteredComponent("abc"),
                    new DummyRegisteredComponent("def")
                });
            }

            using (Mocks.Playback())
            {
                RuntimeRegisteredComponentResolver<DummyRegisteredComponent> resolver =
                    new RuntimeRegisteredComponentResolver<DummyRegisteredComponent>(runtime);

                IList<string> names = resolver.GetNames();

                Assert.AreElementsEqual(new string[] { "abc", "def" }, names);
            }
        }

        [Test]
        public void ResolveDelegatesToTheRuntime()
        {
            IRuntime runtime = Mocks.StrictMock<IRuntime>();
            DummyRegisteredComponent[] components = new DummyRegisteredComponent[]
            {
                new DummyRegisteredComponent("abc"),
                new DummyRegisteredComponent("def")
            };

            using (Mocks.Record())
            {
                SetupResult.For(runtime.ResolveAll<DummyRegisteredComponent>()).Return(components);
            }

            using (Mocks.Playback())
            {
                RuntimeRegisteredComponentResolver<DummyRegisteredComponent> resolver =
                    new RuntimeRegisteredComponentResolver<DummyRegisteredComponent>(runtime);

                Assert.AreSame(components[1], resolver.Resolve("def"), "Resolver should match component names.");
                Assert.AreSame(components[0], resolver.Resolve("AbC"), "Resolver should match component names case-insensitively.");
                Assert.IsNull(resolver.Resolve("unknown"), "Resolver should return null when no component with the specified name was found.");
            }
        }

        [Test, ExpectedArgumentNullException]
        public void ResolveShouldThrowIfTheNameArgumentIsNull()
        {
            IRuntime runtime = Mocks.Stub<IRuntime>();

            RuntimeRegisteredComponentResolver<DummyRegisteredComponent> resolver =
                new RuntimeRegisteredComponentResolver<DummyRegisteredComponent>(runtime);

            resolver.Resolve(null);
        }

        private class DummyRegisteredComponent : IRegisteredComponent
        {
            private readonly string name;

            public DummyRegisteredComponent(string name)
            {
                this.name = name;
            }

            public string Name
            {
                get { return name; }
            }

            public string Description
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
