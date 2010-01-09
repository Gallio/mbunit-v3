// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Tests;
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
    public class AutomaticTestFixtureTest : BaseTestWithSampleRunner
    {
        [FixtureSetUp]
        public void ExploreAssembly()
        {
            Runner.AddAssembly(typeof(AutomaticTestFixtureTest).Assembly);
            Runner.Explore();
        }

        [Test]
        [Row(typeof(AutomaticTestFixtureSampleWithNonPrimaryPattern))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromNestedFixture))]
        [Row(typeof(AutomaticTestFixtureSampleInferredFromNestedAutomaticFixture))]
        public void ShouldInferTestFixtureIfTypeHasNonPrimaryPatternOrNestedTypeIsAFixture(Type type)
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(type));
            Assert.IsNotNull(fixture);
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
        }

        [Test]
        [Row(typeof(AutomaticTestFixtureSampleNotInferred))]
        [Row(typeof(AutomaticTestFixtureSampleNotInferredFromInstanceMethodOnAbstractClass))]
        public void ShouldNotInferTestFixtureIfTypeHasNoPatternsOrMembersWithPatterns(Type sampleType)
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(sampleType));
            Assert.IsNull(fixture);
        }

        /// <summary>
        /// This is a sample test fixture that has no [TestFixture] attribute but does
        /// have some other non-primary pattern attribute, [Explicit].
        /// </summary>
        [Explicit("Sample")]
        internal class AutomaticTestFixtureSampleWithNonPrimaryPattern
        {
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
        }

        internal class AutomaticTestFixtureSampleInferredFromField
        {
            [Parameter]
            public int Field = 0;
        }

        internal class AutomaticTestFixtureSampleInferredFromProperty
        {
            [Parameter]
            public int Property { get { return 0; } set { } }
        }

        internal class AutomaticTestFixtureSampleInferredFromEvent
        {
            [Annotation(AnnotationType.Info, "Foo")]
            public event EventHandler Event;

            public void Notify()
            {
                Event(null, null);
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromConstructor
        {
            [Annotation(AnnotationType.Info, "Foo")]
            public AutomaticTestFixtureSampleInferredFromConstructor()
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromGenericParameter<[Parameter] T>
        {
        }

        internal abstract class AutomaticTestFixtureSampleNotInferredFromInstanceMethodOnAbstractClass
        {
            [Test]
            public void Test()
            {
            }
        }

        internal abstract class AutomaticTestFixtureSampleInferredFromStaticMethodOnAbstractClass
        {
            [Test]
            public static void Test()
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromNestedFixture
        {
            [TestFixture]
            public class NestedFixture
            {
            }
        }

        internal class AutomaticTestFixtureSampleInferredFromNestedAutomaticFixture
        {
            public class NestedFixture
            {
                [Test]
                public void Test()
                {
                }
            }
        }
    }
}
