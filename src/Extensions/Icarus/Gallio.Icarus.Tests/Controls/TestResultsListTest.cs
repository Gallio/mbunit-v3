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
using System.Drawing;

using Gallio.Icarus.Controls;

using MbUnit.Framework;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using System.Windows.Forms;
using Gallio.Reflection;

namespace Gallio.Icarus.Controls.Tests
{
    [TestFixture]
    public class TestResultsListTest
    {
        private TestResultsList testResultsList;

        [SetUp]
        public void SetUp()
        {
            testResultsList = new TestResultsList();
        }

        [Test]
        public void UpdateTestResults_Test_Failed()
        {
            Assert.AreEqual(0, testResultsList.Items.Count);
            TestData testData = CreateTestData();
            TestStepRun testStepRun = CreateTestStepRun(TestStatus.Failed);
            testResultsList.UpdateTestResults(testData, testStepRun);
            Assert.AreEqual(1, testResultsList.Items.Count);
            ListViewItem listViewItem = testResultsList.Items[0];
            Assert.AreEqual(1, listViewItem.ImageIndex);
            Assert.AreEqual(testStepRun.Step.Name, listViewItem.SubItems[1].Text);
            Assert.AreEqual(testData.Metadata.GetValue(MetadataKeys.TestKind), listViewItem.SubItems[2].Text);
            Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), listViewItem.SubItems[3].Text);
            Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), listViewItem.SubItems[4].Text);
            Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, listViewItem.SubItems[5].Text);
            Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, listViewItem.SubItems[6].Text);
        }

        [Test]
        public void UpdateTestResults_Test_Inconclusive()
        {
            Assert.AreEqual(0, testResultsList.Items.Count);
            TestData testData = CreateTestData();
            TestStepRun testStepRun = CreateTestStepRun(TestStatus.Inconclusive);
            testResultsList.UpdateTestResults(testData, testStepRun);
            Assert.AreEqual(1, testResultsList.Items.Count);
            ListViewItem listViewItem = testResultsList.Items[0];
            Assert.AreEqual(2, listViewItem.ImageIndex);
            Assert.AreEqual(testStepRun.Step.Name, listViewItem.SubItems[1].Text);
            Assert.AreEqual(testData.Metadata.GetValue(MetadataKeys.TestKind), listViewItem.SubItems[2].Text);
            Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), listViewItem.SubItems[3].Text);
            Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), listViewItem.SubItems[4].Text);
            Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, listViewItem.SubItems[5].Text);
            Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, listViewItem.SubItems[6].Text);
        }

        [Test]
        public void UpdateTestResults_Test_Passed()
        {
            Assert.AreEqual(0, testResultsList.Items.Count);
            TestData testData = CreateTestData();
            TestStepRun testStepRun = CreateTestStepRun(TestStatus.Passed);
            testResultsList.UpdateTestResults(testData, testStepRun);
            Assert.AreEqual(1, testResultsList.Items.Count);
            ListViewItem listViewItem = testResultsList.Items[0];
            Assert.AreEqual(0, listViewItem.ImageIndex);
            Assert.AreEqual(testStepRun.Step.Name, listViewItem.SubItems[1].Text);
            Assert.AreEqual(testData.Metadata.GetValue(MetadataKeys.TestKind), listViewItem.SubItems[2].Text);
            Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), listViewItem.SubItems[3].Text);
            Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), listViewItem.SubItems[4].Text);
            Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, listViewItem.SubItems[5].Text);
            Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, listViewItem.SubItems[6].Text);
        }

        private TestData CreateTestData()
        {
            TestData testData = new TestData("id", "name", "fullname");
            testData.Metadata.SetValue(MetadataKeys.TestKind, "testkind");
            return testData;
        }

        private TestStepRun CreateTestStepRun(TestStatus testStatus)
        {
            TestStepData testStepData = new TestStepData("id", "name", "fullname", "testid");
            testStepData.CodeReference = new CodeReference("assembly", "namespace", "type", "member", "parameter");
            TestStepRun testStepRun = new TestStepRun(testStepData);
            testStepRun.Result.Outcome = new TestOutcome(testStatus);
            testStepRun.Result.Duration = 1.0;
            testStepRun.Result.AssertCount = 1;
            return testStepRun;
        }

        [Test]
        public void Clear_Test()
        {
            testResultsList.Items.Add("test");
            Assert.AreEqual(1, testResultsList.Items.Count);
            testResultsList.Clear();
            Assert.AreEqual(0, testResultsList.Items.Count);
        }
    }
}
