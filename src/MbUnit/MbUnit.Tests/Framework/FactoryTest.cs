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
using Gallio.Framework;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using System.Collections;

#pragma warning disable 0649

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(FactoryAttribute))]
    [RunSample(typeof(FactoryOnGenericTestClass<>))]
    [RunSample(typeof(SimpleUseCases))]
    [RunSample(typeof(GenericUseCases<>))]
    [RunSample(typeof(IndirectInstanceUseCases))]
    public class FactoryTest : BaseTestWithSampleRunner
    {
        [Test]
        [Row(typeof(FactoryOnGenericTestClass<>), "Test", new string[]
        {
            "System.Int32, 123",
            "System.String, abc" 
        })]
        [Row(typeof(SimpleUseCases), "InstanceFactoryTest", new string[]
        {
            "InstanceField: System.Int32, 123",
            "InstanceField: System.String, abc",
            "InstanceProperty: System.Int32, 123",
            "InstanceProperty: System.String, abc",
            "InstanceMethod: System.Int32, 123",
            "InstanceMethod: System.String, abc",
        })]
        [Row(typeof(GenericUseCases<>), "InstanceFactoryTest", new string[]
        {
            "InstanceField: System.Int32, 123",
            "InstanceField: System.String, abc",
            "InstanceProperty: System.Int32, 123",
            "InstanceProperty: System.String, abc",
            "InstanceMethod: System.Int32, 123",
            "InstanceMethod: System.String, abc",
        })]
        [Row(typeof(SimpleUseCases), "StaticFactoryTest", new string[]
        {
            "StaticField: System.Int32, 123",
            "StaticField: System.String, abc",
            "StaticProperty: System.Int32, 123",
            "StaticProperty: System.String, abc",
            "StaticMethod: System.Int32, 123",
            "StaticMethod: System.String, abc",
            "StaticField: System.Int32, 123",
            "StaticField: System.String, abc",
            "StaticProperty: System.Int32, 123",
            "StaticProperty: System.String, abc",
            "StaticMethod: System.Int32, 123",
            "StaticMethod: System.String, abc",
            "ExternalField: System.Int32, 123",
            "ExternalField: System.String, abc",
            "ExternalProperty: System.Int32, 123",
            "ExternalProperty: System.String, abc",
            "ExternalMethod: System.Int32, 123",
            "ExternalMethod: System.String, abc",
        })]
        [Row(typeof(GenericUseCases<>), "StaticFactoryTest", new string[]
        {
            "StaticField: System.Int32, 123",
            "StaticField: System.String, abc",
            "StaticProperty: System.Int32, 123",
            "StaticProperty: System.String, abc",
            "StaticMethod: System.Int32, 123",
            "StaticMethod: System.String, abc",
            "StaticField: System.Int32, 123",
            "StaticField: System.String, abc",
            "StaticProperty: System.Int32, 123",
            "StaticProperty: System.String, abc",
            "StaticMethod: System.Int32, 123",
            "StaticMethod: System.String, abc",
            "ExternalField: System.Int32, 123",
            "ExternalField: System.String, abc",
            "ExternalProperty: System.Int32, 123",
            "ExternalProperty: System.String, abc",
            "ExternalMethod: System.Int32, 123",
            "ExternalMethod: System.String, abc",
        })]
        [Row(typeof(IndirectInstanceUseCases), "IndirectFactoryTest", new string[]
        {
            "IndirectFactoryField: 123",
            "IndirectFactoryField: 456",
            "IndirectFactoryField: 789",
        })]
        public void VerifySampleOutput(Type fixtureType, string sampleName, string[] output)
        {
            IList<TestStepRun> runs = Runner.GetTestCaseRunsWithin(
                CodeReference.CreateFromMember(fixtureType.GetMethod(sampleName)));

            Assert.AreEqual(output.Length, runs.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
                AssertLogContains(runs[i], output[i]);
        }

        [Explicit("Sample")]
        [Factory(typeof(FactoryOnGenericTestClass<object>), "Factory")]
        internal class FactoryOnGenericTestClass<T>
        {
            private readonly T value;

            public FactoryOnGenericTestClass(T value)
            {
                this.value = value;
            }

            public static IEnumerable<object[]> Factory
            {
                get
                {
                    yield return new object[] { typeof(int), 123 };
                    yield return new object[] { typeof(string), "abc" };
                }
            }

            [Test]
            public void Test()
            {
                TestLog.WriteLine("{0}, {1}", typeof(T), value);
            }
        }

        [Explicit("Sample")]
        internal class SimpleUseCases
        {
            public IEnumerable<object[]> FactoryInstanceField;
            public static IEnumerable<object[]> FactoryStaticField = new object[][]
            {
                new object[] { typeof(int), 123, "StaticField" },
                new object[] { typeof(string), "abc", "StaticField" }
            };

            public SimpleUseCases()
            {
                FactoryInstanceField = new object[][]
                {
                    new object[] { typeof(int), 123, this, "InstanceField" },
                    new object[] { typeof(string), "abc", this, "InstanceField" }
                };
            }

            public IEnumerable<object[]> FactoryInstanceProperty
            {
                get
                {
                    yield return new object[] { typeof(int), 123, this, "InstanceProperty" };
                    yield return new object[] { typeof(string), "abc", this, "InstanceProperty" };
                }
            }

            public static IEnumerable<object[]> FactoryStaticProperty
            {
                get
                {
                    yield return new object[] { typeof(int), 123, "StaticProperty" };
                    yield return new object[] { typeof(string), "abc", "StaticProperty" };
                }
            }

            public IEnumerable<object[]> FactoryInstanceMethod()
            {
                yield return new object[] { typeof(int), 123, this, "InstanceMethod" };
                yield return new object[] { typeof(string), "abc", this, "InstanceMethod" };
            }

            public static IEnumerable<object[]> FactoryStaticMethod()
            {
                yield return new object[] { typeof(int), 123, "StaticMethod" };
                yield return new object[] { typeof(string), "abc", "StaticMethod" };
            }

            [Test]
            [Factory("FactoryInstanceField", Order = 1)]
            [Factory("FactoryInstanceProperty", Order = 2)]
            [Factory("FactoryInstanceMethod", Order = 3)]
            public void InstanceFactoryTest<T>(T value, SimpleUseCases instance, string source)
            {
                Assert.AreSame(this, instance);
                TestLog.WriteLine("{0}: {1}, {2}", source, typeof(T), value);
            }

            [Test]
            [Factory("FactoryStaticField", Order = 1)]
            [Factory("FactoryStaticProperty", Order = 2)]
            [Factory("FactoryStaticMethod", Order = 3)]
            [Factory(typeof(SimpleUseCases), "FactoryStaticField", Order = 4)]
            [Factory(typeof(SimpleUseCases), "FactoryStaticProperty", Order = 5)]
            [Factory(typeof(SimpleUseCases), "FactoryStaticMethod", Order = 6)]
            [Factory(typeof(ExternalFactories), "Field", Order = 7)]
            [Factory(typeof(ExternalFactories), "Property", Order = 8)]
            [Factory(typeof(ExternalFactories), "Method", Order = 9)]
            public void StaticFactoryTest<T>(T value, string source)
            {
                TestLog.WriteLine("{0}: {1}, {2}", source, typeof(T), value);
            }
        }

        [Explicit("Sample")]
        [Row(typeof(int))]
        internal class GenericUseCases<TOuter>
        {
            public IEnumerable<object[]> FactoryInstanceField;
            public static IEnumerable<object[]> FactoryStaticField = new object[][]
            {
                new object[] { typeof(int), 123, "StaticField" },
                new object[] { typeof(string), "abc", "StaticField" }
            };

            public GenericUseCases()
            {
                FactoryInstanceField = new object[][]
                {
                    new object[] { typeof(int), 123, this, "InstanceField" },
                    new object[] { typeof(string), "abc", this, "InstanceField" }
                };
            }

            public IEnumerable<object[]> FactoryInstanceProperty
            {
                get
                {
                    yield return new object[] { typeof(int), 123, this, "InstanceProperty" };
                    yield return new object[] { typeof(string), "abc", this, "InstanceProperty" };
                }
            }

            public static IEnumerable<object[]> FactoryStaticProperty
            {
                get
                {
                    yield return new object[] { typeof(int), 123, "StaticProperty" };
                    yield return new object[] { typeof(string), "abc", "StaticProperty" };
                }
            }

            public IEnumerable<object[]> FactoryInstanceMethod()
            {
                yield return new object[] { typeof(int), 123, this, "InstanceMethod" };
                yield return new object[] { typeof(string), "abc", this, "InstanceMethod" };
            }

            public static IEnumerable<object[]> FactoryStaticMethod()
            {
                yield return new object[] { typeof(int), 123, "StaticMethod" };
                yield return new object[] { typeof(string), "abc", "StaticMethod" };
            }

            [Test]
            [Factory("FactoryInstanceField", Order = 1)]
            [Factory("FactoryInstanceProperty", Order = 2)]
            [Factory("FactoryInstanceMethod", Order = 3)]
            public void InstanceFactoryTest<T>(T value, GenericUseCases<TOuter> instance, string source)
            {
                Assert.AreSame(this, instance);
                TestLog.WriteLine("{0}: {1}, {2}", source, typeof(T), value);
            }

            [Test]
            [Factory("FactoryStaticField", Order = 1)]
            [Factory("FactoryStaticProperty", Order = 2)]
            [Factory("FactoryStaticMethod", Order = 3)]
            [Factory(typeof(SimpleUseCases), "FactoryStaticField", Order = 4)]
            [Factory(typeof(SimpleUseCases), "FactoryStaticProperty", Order = 5)]
            [Factory(typeof(SimpleUseCases), "FactoryStaticMethod", Order = 6)]
            [Factory(typeof(ExternalFactories), "Field", Order = 7)]
            [Factory(typeof(ExternalFactories), "Property", Order = 8)]
            [Factory(typeof(ExternalFactories), "Method", Order = 9)]
            public void StaticFactoryTest<T>(T value, string source)
            {
                TestLog.WriteLine("{0}: {1}, {2}", source, typeof(T), value);
            }
        }

        internal static class ExternalFactories
        {
            public static IEnumerable<object[]> Field = new object[][]
            {
                new object[] { typeof(int), 123, "ExternalField" },
                new object[] { typeof(string), "abc", "ExternalField" }
            };

            public static IEnumerable<object[]> Property
            {
                get
                {
                    yield return new object[] { typeof(int), 123, "ExternalProperty" };
                    yield return new object[] { typeof(string), "abc", "ExternalProperty" };
                }
            }

            public static IEnumerable<object[]> Method()
            {
                yield return new object[] { typeof(int), 123, "ExternalMethod" };
                yield return new object[] { typeof(string), "abc", "ExternalMethod" };
            }
        }

        [Explicit("Sample")]
        internal class IndirectInstanceUseCases
        {
            public static IEnumerable<int> IndirectStaticMethod()
            {
                yield return 123;
                yield return 456;
                yield return 789;
            }

            [Factory(/*typeof(IndirectInstanceUseCases), */"IndirectStaticMethod")]
            public int IndirectFactoryField;

            [Test]
            public void IndirectFactoryTest()
            {
                TestLog.WriteLine("IndirectFactoryField: " + IndirectFactoryField);
            }
        }
    }
}
