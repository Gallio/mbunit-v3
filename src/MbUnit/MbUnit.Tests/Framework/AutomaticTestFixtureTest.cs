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
using System.Reflection;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Reflection;
using Gallio.Tests.Integration;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    /// <summary>
    /// Verifies the behavior of types that do not possess [TestFixture] attributes.
    /// The framework may determine that a type is a test fixture despite not having
    /// a [TestFixture] attribute applied.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(TestTypePatternAttribute))]
    public class AutomaticTestFixtureTest : BaseSampleTest
    {
        [FixtureSetUp]
        public void RunSample()
        {
            InitializeRunner();
            Runner.AddAssembly(typeof(AutomaticTestFixtureTest).Assembly);
            Runner.Explore();
        }

        [Test]
        public void ShouldInferTestFixtureIfTypeHasNonPrimaryPattern()
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(typeof(AutomaticTestFixtureSampleWithNonPrimaryPattern)));
            Assert.IsNotNull(fixture);

            TestData nestedFixture = Runner.GetTestData(CodeReference.CreateFromType(typeof(AutomaticTestFixtureSampleWithNonPrimaryPattern.NestedFixture)));
            Assert.IsNotNull(nestedFixture);
            Assert.Contains(fixture.Children, nestedFixture);
        }

        [Test]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromMethod))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromProperty))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromField))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromConstructor))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromGenericParameter<>))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromStaticMethodOnAbstractClass))]
        public void ShouldInferTestFixtureIfTypeHasMemberOrGenericParameterWithPatterns(Type sampleType)
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(sampleType));
            Assert.IsNotNull(fixture);

            MethodInfo testMethod = sampleType.GetMethod("Test");
            if (testMethod != null)
            {
                TestData test = Runner.GetTestData(CodeReference.CreateFromMember(testMethod));
                Assert.IsNotNull(test);
                Assert.Contains(fixture.Children, test);
            }

            TestData nestedFixture = Runner.GetTestData(CodeReference.CreateFromType(sampleType.GetNestedType("NestedFixture")));
            Assert.IsNotNull(nestedFixture);
            Assert.Contains(fixture.Children, nestedFixture);
        }

        [Test]
        [Row(typeof(AutomaticTestFixtureSampleNotInferred))]
        [Row(typeof(AutomaticTestFixtureSampleNotInferredFromInstanceMethodOnAbstractClass))]
        public void ShouldNotInferTestFixtureIfTypeHasNoPatternsOrMembersWithPatterns(Type sampleType)
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(sampleType));
            Assert.IsNull(fixture);

            TestData nestedFixture = Runner.GetTestData(CodeReference.CreateFromType(sampleType.GetNestedType("NestedFixture")));
            Assert.IsNotNull(nestedFixture);
        }

        /// <summary>
        /// This is a sample test fixture that has no [TestFixture] attribute but does
        /// have some other non-primary pattern attribute, [Explicit].
        /// </summary>
        [Explicit("Sample")]
        internal class AutomaticTestFixtureSampleWithNonPrimaryPattern
        {
            [TestFixture]
            public class NestedFixture
            {
            }
        }

        /// <summary>
        /// This is a sample test fixture that has no [TestFixture] attribute and no
        /// other non-primary pattern attributes and no test methods either.
        /// </summary>
        internal class AutomaticTestFixtureSampleNotInferred
        {
            public void NonTestMethod()
            {
            }

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        /// <summary>
        /// This is a sample test fixture that has no [TestFixture] attribute and no
        /// other non-primary pattern attributes but has test methods.
        /// </summary>
        internal class AutomaticTestFixtureSampleInferredFromMethod
        {
            [Test]
            public void Test()
            {
            }

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromField
        {
            [Parameter]
            public int Field = 0;

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromProperty
        {
            [Parameter]
            public int Property { get { return 0; } set { } }

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromEvent
        {
            [Annotation(AnnotationType.Info, "Foo")]
            public event EventHandler Event;

            public void Notify()
            {
                Event(null, null);
            }

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromConstructor
        {
            [Annotation(AnnotationType.Info, "Foo")]
            public AutomaticTestFixtureSampleInferredFromConstructor()
            {
            }

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromGenericParameter<[Parameter] T>
        {
            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal abstract class AutomaticTestFixtureSampleNotInferredFromInstanceMethodOnAbstractClass
        {
            [Test]
            public void Test()
            {
            }

            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal abstract class AutomaticTestFixtureSampleInferredFromStaticMethodOnAbstractClass
        {
            [Test]
            public static void Test()
            {
            }

            [TestFixture]
            public class NestedFixture
            {
            }
        }
    }
}
