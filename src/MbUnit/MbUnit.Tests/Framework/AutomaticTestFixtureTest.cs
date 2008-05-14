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
using Gallio.Framework.Pattern;
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
            CollectionAssert.Contains(fixture.Children, nestedFixture);
        }

        [Test]
        public void ShouldInferTestFixtureIfTypeHasNoPatternsButHasMethodsWithPatterns()
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(typeof(AutomaticTestFixtureSampleWithTests)));
            Assert.IsNotNull(fixture);

            TestData test = Runner.GetTestData(CodeReference.CreateFromMember(typeof(AutomaticTestFixtureSampleWithTests).GetMethod("Test")));
            Assert.IsNotNull(test);
            CollectionAssert.Contains(fixture.Children, test);

            TestData nestedFixture = Runner.GetTestData(CodeReference.CreateFromType(typeof(AutomaticTestFixtureSampleWithTests.NestedFixture)));
            Assert.IsNotNull(nestedFixture);
            CollectionAssert.Contains(fixture.Children, nestedFixture);
        }

        [Test]
        public void ShouldNotInferTestFixtureIfTypeHasNoPatternsOrMethodsWithPatterns()
        {
            TestData fixture = Runner.GetTestData(CodeReference.CreateFromType(typeof(AutomaticTestFixtureSampleWithNoTests)));
            Assert.IsNull(fixture);

            TestData nestedFixture = Runner.GetTestData(CodeReference.CreateFromType(typeof(AutomaticTestFixtureSampleWithNoTests.NestedFixture)));
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
        internal class AutomaticTestFixtureSampleWithNoTests
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
        internal class AutomaticTestFixtureSampleWithTests
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
    }
}
