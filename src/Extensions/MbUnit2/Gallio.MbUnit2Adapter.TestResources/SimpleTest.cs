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
using MbUnit.Framework;

namespace Gallio.MbUnit2Adapter.TestResources
{
    /// <summary>
    /// A simple test fixture.
    /// </summary>
    [TestFixture]
    public class SimpleTest
    {
        /// <summary>
        /// Fixture setup.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Console.WriteLine("TestFixtureSetUp");
        }

        /// <summary>
        /// Fixture teardown.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            Console.WriteLine("TestFixtureTearDown");
        }

        /// <summary>
        /// Setup.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            Console.WriteLine("SetUp");
        }

        /// <summary>
        /// Teardown.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("TearDown");
        }

        /// <summary>
        /// A passing test.
        /// </summary>
        [Test]
        public void Pass()
        {
        }

        /// <summary>
        /// A failing test.
        /// </summary>
        [Test]
        public void Fail()
        {
            Assert.Fail("Boom");
        }
    }
}
