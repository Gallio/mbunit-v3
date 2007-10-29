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
using MbUnit.Framework;

namespace Gallio.TestResources.MbUnit.Fixtures
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
        [TestFixtureSetUp]
        public void BaseTestFixtureSetUp()
        {
            Console.WriteLine("BaseTestFixtureSetUp");
        }

        [TestFixtureTearDown]
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
        public class DerivedFixture : FixtureInheritanceSample
        {
            [TestFixtureSetUp]
            public void DerivedTestFixtureSetUp()
            {
                Console.WriteLine("DerivedTestFixtureSetUp");
            }

            [TestFixtureTearDown]
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
}
