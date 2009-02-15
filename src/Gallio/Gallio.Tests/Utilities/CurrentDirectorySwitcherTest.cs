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
using System.IO;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    [TestFixture]
    [TestsOn(typeof(CurrentDirectorySwitcher))]
    public class CurrentDirectorySwitcherTest
    {
        [Test, ExpectedArgumentNullException]
        public void ShouldThrowIfDirectoryIsNull()
        {
            new CurrentDirectorySwitcher(null);
        }

        [Test]
        public void ShouldSetAndResetCurrentDirectory()
        {
            string originalDirectory = Environment.CurrentDirectory;
            string newDirectory = Path.GetTempPath().TrimEnd('\\');

            using (new CurrentDirectorySwitcher(newDirectory))
            {
                Assert.AreEqual(newDirectory, Environment.CurrentDirectory);
            }

            Assert.AreEqual(originalDirectory, Environment.CurrentDirectory);
        }
    }
}
