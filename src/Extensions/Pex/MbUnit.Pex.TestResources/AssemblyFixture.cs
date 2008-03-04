// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

// ==++==
// 
//   Copyright (c) Pex Extensions Project (http://www.codeplex.com/pex). All rights reserved.
//   License: MS-Pl 
//
// ==--==

using System;
using MbUnit.Framework;

namespace MbUnit.Pex.TestResources
{
    [AssemblyFixture]
    public static class AssemblyFixture
    {
        public static bool SetUpCalled = false;

        [FixtureSetUp]
        public static void SetUp()
        {
            SetUpCalled = true;
            Console.WriteLine("assembly setup");
        }

        public static bool TearDownCalled = false;

        [FixtureTearDown]
        public static void TearDown()
        {
            Assert.IsTrue(MbUnitTestSampleForPex.ClassCleanupTouched);
            TearDownCalled = true;
            Console.WriteLine("assembly teardown");
        }
    }
}
