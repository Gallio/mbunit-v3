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

using System.Windows.Forms;
using Gallio.Icarus.Controls;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    [TestFixture, Category("Controls")]
    public class TestResultsListTest
    {
        [Test]
        public void TestResultsList_Test()
        {
            var testResultsList = new TestResultsList();

            Assert.AreEqual(6, testResultsList.Columns.Count);

            var stepName = testResultsList.Columns[0];
            Assert.AreEqual("Step name", stepName.Text);
            Assert.AreEqual(200, stepName.Width);

            // etc...

            Assert.AreEqual(true, testResultsList.VirtualMode);
            Assert.AreEqual(true, testResultsList.FullRowSelect);
            Assert.AreEqual(View.Details, testResultsList.View);
        }
    }
}