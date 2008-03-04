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

using System;
using System.Drawing;

using Gallio.Icarus.Controls;

using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    [TestFixture]
    public class TestResultsListTest
    {
        private TestResultsList testResultsList;

        [SetUp]
        public void SetUp()
        {
            testResultsList = new TestResultsList();
            testResultsList.UpdateTestResults("test1", "Passed", Color.Green, "10", "type", "namespace", "assembly");
            testResultsList.UpdateTestResults("test2", "Failed", Color.Red, "10", "type", "namespace", "assembly");
            testResultsList.UpdateTestResults("test3", "Inconclusive", Color.Yellow, "10", "type", "namespace", "assembly");
            Assert.AreEqual(3, testResultsList.Items.Count);
        }

        [Test]
        public void FilterPassed_Test()
        {
            testResultsList.Filter = "Passed";
            Assert.AreEqual(1, testResultsList.Items.Count);
        }

        [Test]
        public void FilterFailed_Test()
        {
            testResultsList.Filter = "Failed";
            Assert.AreEqual(1, testResultsList.Items.Count);
        }

        [Test]
        public void FilterInconclusive_Test()
        {
            testResultsList.Filter = "Inconclusive";
            Assert.AreEqual(1, testResultsList.Items.Count);
        }

        [Test]
        public void RemoveFilter_Test()
        {
            testResultsList.Filter = string.Empty;
            Assert.AreEqual(3, testResultsList.Items.Count);
        }

        [Test]
        public void Clear_Test()
        {
            testResultsList.Clear();
            Assert.AreEqual(0, testResultsList.Items.Count);
        }
    }
}
