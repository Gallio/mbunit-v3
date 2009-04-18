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
using System.Drawing;

using Gallio.Icarus.Controls;

using MbUnit.Framework;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;
using System.Windows.Forms;
using Gallio.Reflection;

namespace Gallio.Icarus.Tests.Controls
{
    [TestFixture, Category("Controls")]
    public class TestResultsListTest : TestResultsList
    {
        //private TestResultsList testResultsList;

    //    [SetUp]
    //    public void SetUp()
    //    {
    //        testResultsList = new TestResultsList();
    //    }

    //    [Test]
    //    public void AddTestStepRun_Passed_Test()
    //    {
    //        TestStepData testStepData = new TestStepData("id", "name", "fullName", "testId");
    //        testStepData.CodeReference = new CodeReference("assemblyName", "namespaceName", "typeName", "memberName", "parameterName");
    //        TestStepRun testStepRun = new TestStepRun(testStepData);
    //        testStepRun.Result.Outcome = new TestOutcome(TestStatus.Passed);
    //        Assert.AreEqual(0, testResultsList.Items.Count);
    //        string testKind = "testKind";
    //        testResultsList.AddTestStepRun(testKind, testStepRun, 0);
    //        Assert.AreEqual(1, testResultsList.Items.Count);
    //        ListViewItem lvi = testResultsList.Items[0];
    //        Assert.AreEqual(0, lvi.ImageIndex);
    //        Assert.AreEqual(testStepRun.Step.Name, lvi.Text);
    //        Assert.AreEqual(testKind, lvi.SubItems[1].Text);
    //        Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), lvi.SubItems[2].Text);
    //        Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), lvi.SubItems[3].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, lvi.SubItems[4].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, lvi.SubItems[5].Text);
    //        Assert.AreEqual(0, lvi.IndentCount);
    //    }

    //    [Test]
    //    public void AddTestStepRun_Failed_Test()
    //    {
    //        TestStepData testStepData = new TestStepData("id", "name", "fullName", "testId");
    //        testStepData.CodeReference = new CodeReference("assemblyName", "namespaceName", "typeName", "memberName", "parameterName");
    //        TestStepRun testStepRun = new TestStepRun(testStepData);
    //        testStepRun.Result.Outcome = new TestOutcome(TestStatus.Failed);
    //        Assert.AreEqual(0, testResultsList.Items.Count);
    //        string testKind = "testKind";
    //        testResultsList.AddTestStepRun(testKind, testStepRun, 0);
    //        Assert.AreEqual(1, testResultsList.Items.Count);
    //        ListViewItem lvi = testResultsList.Items[0];
    //        Assert.AreEqual(1, lvi.ImageIndex);
    //        Assert.AreEqual(testStepRun.Step.Name, lvi.SubItems[0].Text);
    //        Assert.AreEqual(testKind, lvi.SubItems[1].Text);
    //        Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), lvi.SubItems[2].Text);
    //        Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), lvi.SubItems[3].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, lvi.SubItems[4].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, lvi.SubItems[5].Text);
    //        Assert.AreEqual(0, lvi.IndentCount);
    //    }

    //    [Test]
    //    public void AddTestStepRun_Inconclusive_Test()
    //    {
    //        TestStepData testStepData = new TestStepData("id", "name", "fullName", "testId");
    //        testStepData.CodeReference = new CodeReference("assemblyName", "namespaceName", "typeName", "memberName", "parameterName");
    //        TestStepRun testStepRun = new TestStepRun(testStepData);
    //        testStepRun.Result.Outcome = new TestOutcome(TestStatus.Inconclusive);
    //        Assert.AreEqual(0, testResultsList.Items.Count);
    //        string testKind = "testKind";
    //        testResultsList.AddTestStepRun(testKind, testStepRun, 0);
    //        Assert.AreEqual(1, testResultsList.Items.Count);
    //        ListViewItem lvi = testResultsList.Items[0];
    //        Assert.AreEqual(2, lvi.ImageIndex);
    //        Assert.AreEqual(testStepRun.Step.Name, lvi.SubItems[0].Text);
    //        Assert.AreEqual(testKind, lvi.SubItems[1].Text);
    //        Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), lvi.SubItems[2].Text);
    //        Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), lvi.SubItems[3].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, lvi.SubItems[4].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, lvi.SubItems[5].Text);
    //        Assert.AreEqual(0, lvi.IndentCount);
    //    }

    //    [Test]
    //    public void AddTestStepRun_Skipped_Test()
    //    {
    //        TestStepData testStepData = new TestStepData("id", "name", "fullName", "testId");
    //        testStepData.CodeReference = new CodeReference("assemblyName", "namespaceName", "typeName", "memberName", "parameterName");
    //        TestStepRun testStepRun = new TestStepRun(testStepData);
    //        testStepRun.Result.Outcome = new TestOutcome(TestStatus.Skipped);
    //        Assert.AreEqual(0, testResultsList.Items.Count);
    //        string testKind = "testKind";
    //        testResultsList.AddTestStepRun(testKind, testStepRun, 0);
    //        Assert.AreEqual(1, testResultsList.Items.Count);
    //        ListViewItem lvi = testResultsList.Items[0];
    //        Assert.AreEqual(-1, lvi.ImageIndex);
    //        Assert.AreEqual(testStepRun.Step.Name, lvi.SubItems[0].Text);
    //        Assert.AreEqual(testKind, lvi.SubItems[1].Text);
    //        Assert.AreEqual(testStepRun.Result.Duration.ToString("0.000"), lvi.SubItems[2].Text);
    //        Assert.AreEqual(testStepRun.Result.AssertCount.ToString(), lvi.SubItems[3].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.TypeName, lvi.SubItems[4].Text);
    //        Assert.AreEqual(testStepRun.Step.CodeReference.AssemblyName, lvi.SubItems[5].Text);
    //        Assert.AreEqual(0, lvi.IndentCount);
    //    }

    //    [Test]
    //    public void Clear_Test()
    //    {
    //        Assert.AreEqual(0, testResultsList.Items.Count);
    //        testResultsList.Items.Add("test");
    //        Assert.AreEqual(1, testResultsList.Items.Count);
    //        testResultsList.Clear();
    //        Assert.AreEqual(0, testResultsList.Items.Count);
    //    }

    //    [Test]
    //    public void OnColumnClick_Test()
    //    {
    //        columnSorter.SortColumn = 0;
    //        columnSorter.Order = System.Windows.Forms.SortOrder.None;
    //        OnColumnClick(new ColumnClickEventArgs(0));
    //        Assert.AreEqual(System.Windows.Forms.SortOrder.Ascending, columnSorter.Order);
    //    }

    //    [Test]
    //    public void OnColumnClick_Reverse_Test()
    //    {
    //        columnSorter.SortColumn = 0;
    //        columnSorter.Order = System.Windows.Forms.SortOrder.Ascending;
    //        OnColumnClick(new ColumnClickEventArgs(0));
    //        Assert.AreEqual(System.Windows.Forms.SortOrder.Descending, columnSorter.Order);
    //        OnColumnClick(new ColumnClickEventArgs(0));
    //        Assert.AreEqual(System.Windows.Forms.SortOrder.Ascending, columnSorter.Order);
    //    }
    }
}