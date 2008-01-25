using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Hosting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Hosting
{
    [TestFixture]
    [TestsOn(typeof(RuntimeRegisteredComponentResolver<>))]
    public class RuntimeRegisteredComponentResolverTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenRuntimeArgumentIsNull()
        {
            new RuntimeRegisteredComponentResolver<DummyRegisteredComponent>(null);
        }

        [Test]
        public void GetNamesDelegatesToTheRuntime()
        {
            IRuntime runtime = Mocks.CreateMock<IRuntime>();

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

                CollectionAssert.AreElementsEqual(new string[] { "abc", "def" }, names);
            }
        }

        [Test]
        public void ResolveDelegatesToTheRuntime()
        {
            IRuntime runtime = Mocks.CreateMock<IRuntime>();
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
