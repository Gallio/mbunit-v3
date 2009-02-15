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
using MbUnit.Framework;

namespace MbUnit.TestResources.Fixtures
{
    /// <summary>
    /// This fixture is used to ensure that test fixtures with inherited attributes
    /// compose in the desired manner.
    /// </summary>
    /// <remarks>
    /// This is also a useful sample for testing fixtures in nested types.
    /// </remarks>
    [TestFixture]
    public class FixtureInheritanceSample
    {
        [FixtureSetUp]
        public void BaseTestFixtureSetUp()
        {
            Console.WriteLine("BaseTestFixtureSetUp");
        }

        [FixtureTearDown]
        public void BaseTestFixtureTearDown()
        {
            Console.WriteLine("BaseTestFixtureTearDown");
        }

        [SetUp]
        public void BaseSetUp()
        {
            Console.WriteLine("BaseSetUp");
        }

        [TearDown]
        public void BaseTearDown()
        {
            Console.WriteLine("BaseTearDown");
        }

        [Test]
        public void BaseTest()
        {
            Console.WriteLine("BaseTest");
        }

        [TestFixture]
        public class NestedFixture
        {
            [FixtureSetUp]
            public void NestedTestFixtureSetUp()
            {
                Console.WriteLine("NestedTestFixtureSetUp");
            }

            [FixtureTearDown]
            public void NestedTestFixtureTearDown()
            {
                Console.WriteLine("NestedTestFixtureTearDown");
            }

            [Test]
            public void NestedTest()
            {
                Console.WriteLine("NestedTest");
            }
        }
    }

    [TestFixture]
    public class DerivedFixture : FixtureInheritanceSample
    {
        [FixtureSetUp]
        public void DerivedTestFixtureSetUp()
        {
            Console.WriteLine("DerivedTestFixtureSetUp");
        }

        [FixtureTearDown]
        public void DerivedTestFixtureTearDown()
        {
            Console.WriteLine("DerivedTestFixtureTearDown");
        }

        [SetUp]
        public void DerivedSetUp()
        {
            Console.WriteLine("DerivedSetUp");
        }

        [TearDown]
        public void DerivedTearDown()
        {
            Console.WriteLine("DerivedTearDown");
        }

        [Test]
        public void DerivedTest()
        {
            Console.WriteLine("DerivedTest");
        }
    }
}
