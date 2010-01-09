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

using System;
using System.Collections.Generic;
using Gallio.Common.Concurrency;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestResultsController))]
    internal class TestResultsControllerTest
    {
        private TestResultsController testResultsController;
        private ITestController testController;
        private ITestTreeModel testTreeModel;

        private void EstablishContext()
        {
            testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new LockBox<IList<TestTreeNode>>(new List<TestTreeNode>()));
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            testResultsController = new TestResultsController(testController, testTreeModel);
        }

        [Test]
        public void ElapsedTime_should_be_zero_before_test_run_starts()
        {
            EstablishContext();

            Assert.AreEqual(new TimeSpan(), testResultsController.ElapsedTime);
        }

        [Test]
        public void ElapsedTime_should_be_greater_than_zero_once_test_run_has_started()
        {
            EstablishContext();
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);

            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());
        }

        [Test]
        public void ElapsedTime_should_stop_increasing_once_test_run_has_finished()
        {
            EstablishContext();            
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);
            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());

            testController.Raise(tc => tc.RunFinished += null, testController, EventArgs.Empty);
            var elapsed = testResultsController.ElapsedTime;
            Assert.AreEqual(elapsed, testResultsController.ElapsedTime);
        }

        //[SyncTest]
        //public void ResultsCount_is_set_to_zero_when_test_run_is_started_and_notification_is_raised()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    var testResultsController = new TestResultsController(testController);
        //    var propertyChangedFlag = false;
        //    testResultsController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
        //    {
        //        if (e.PropertyName != "ResultsCount")
        //            return;

        //        Assert.AreEqual(0, testResultsController.ResultsCount.Value);
        //        propertyChangedFlag = true;
        //    };

        //    testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);

        //    Assert.AreEqual(true, propertyChangedFlag);
        //}

        //[SyncTest]
        //public void ExploreStarted_from_TestController_should_reset_results()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    testController.Stub(tc => tc.SelectedTests).Return(new LockBox<IList<TestTreeNode>>(new List<TestTreeNode>()));
        //    testResultsController = new TestResultsController(testController);
        //    var resultsCountFlag = false;
        //    var elapsedTimeFlag = false;
        //    testResultsController.PropertyChanged += (sender, e) =>
        //    {
        //        switch (e.PropertyName)
        //        {
        //            case "ResultsCount":
        //                Assert.AreEqual(0, testResultsController.TestCount);
        //                resultsCountFlag = true;
        //                break;

        //            case "ElapsedTime":
        //                Assert.AreEqual(new TimeSpan(), testResultsController.ElapsedTime);
        //                elapsedTimeFlag = true;
        //                break;
        //        }
        //    };

        //    testController.Raise(tc => tc.ExploreStarted += null, testController, EventArgs.Empty);
        //    Assert.AreEqual(true, resultsCountFlag);
        //    Assert.AreEqual(true, elapsedTimeFlag);
        //}

        [SyncTest]
        public void ElapsedTime_should_be_greater_than_zero_when_explore_has_finished()
        {
            EstablishContext();
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);
            var elapsedTimeFlag = false;
            testResultsController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "ElapsedTime")
                    return;

                Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan()); 
                elapsedTimeFlag = true;
            };

            testController.Raise(tc => tc.ExploreFinished += null, testController, EventArgs.Empty);

            Assert.AreEqual(true, elapsedTimeFlag);
        }

        //[SyncTest]
        //public void PropertyChanged_from_TestController_should_bubble_up()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    var testResultsController = new TestResultsController(testController);
        //    var propChangedFlag = false;
        //    var eventArgs = new PropertyChangedEventArgs("test");
        //    testResultsController.PropertyChanged += (sender, e) =>
        //    {
        //        Assert.AreEqual(eventArgs, e);
        //        propChangedFlag = true;
        //    };

        //    testController.Raise(tc => tc.PropertyChanged += null, testController, eventArgs);

        //    Assert.AreEqual(true, propChangedFlag);
        //}

        //[SyncTest]
        //public void If_test_model_root_is_null_CountResults_should_return_zero()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
        //    testController.Stub(tc => tc.Model).Return(testTreeModel);
        //    var root = new TestTreeNode("root", "root");
        //    testTreeModel.Stub(ttm => ttm.Root).Return(root);
        //    testResultsController = new TestResultsController(testController);
        //    var resultsCountFlag = false;
        //    testResultsController.PropertyChanged += (sender, e) =>
        //    {
        //        if (e.PropertyName != "ResultsCount")
        //            return;

        //        Assert.AreEqual(0, testResultsController.ResultsCount.Value);
        //        resultsCountFlag = true;
        //    };

        //    testController.Raise(tc => tc.TestStepFinished += null, testController,
        //        new TestStepFinishedEventArgs(new Report(),
        //        new TestData("id", "name", "fullName"),
        //        new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))));

        //    Assert.AreEqual(true, resultsCountFlag);
        //}

        //[SyncTest]
        //public void CountResults_should_return_the_number_of_TestStepRuns_in_the_model()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
        //    var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
        //    testController.Stub(tc => tc.Model).Return(testTreeModel);
        //    testTreeModel.Stub(ttm => ttm.Root).Return(Root);
        //    var optionsController = MockRepository.GenerateStub<IOptionsController>();
        //    var testResultsController = new TestResultsController(testController, optionsController);
        //    var resultsCountFlag = false;
        //    var firstTime = true;
        //    testResultsController.PropertyChanged += (sender, e) =>
        //    {
        //        if (e.PropertyName != "ResultsCount")
        //            return;

        //        if (firstTime)
        //        {
        //            Assert.AreEqual(0, testResultsController.ResultsCount);
        //            firstTime = false;
        //        }
        //        else
        //        {
        //            Assert.AreEqual(3, testResultsController.ResultsCount);
        //            resultsCountFlag = true;
        //        }
        //    };

        //    testController.Raise(tc => tc.TestStepFinished += null, testController, 
        //        new TestStepFinishedEventArgs(new Report(), 
        //        new TestData("id", "name", "fullName"), 
        //        new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))));

        //    Assert.AreEqual(true, resultsCountFlag);
        //}

        [Test]
        public void Cache_and_then_retrieve_item()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(0, 3);
            var listViewItem = testResultsController.RetrieveVirtualItem(0);

            Assert.AreEqual("test1", listViewItem.Text);
            Assert.AreEqual(0, listViewItem.ImageIndex); // passed
            Assert.AreEqual(TestKinds.Test, listViewItem.SubItems[1].Text); // testkind
            Assert.AreEqual("0.500", listViewItem.SubItems[2].Text); // duration
            Assert.AreEqual("5", listViewItem.SubItems[3].Text); // assert count
            Assert.AreEqual("", listViewItem.SubItems[4].Text); // code ref
            Assert.AreEqual("", listViewItem.SubItems[5].Text); // file name
            Assert.AreEqual(2, listViewItem.IndentCount);
        }

        //[Test]
        //public void Cache_and_then_narrow_viewport()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
        //    testController.Stub(tc => tc.Model).Return(testTreeModel);
        //    testTreeModel.Stub(ttm => ttm.Root).Return(Root);
        //    var testResultsController = new TestResultsController(testController);

        //    testResultsController.CacheVirtualItems(0, 3);
        //    testResultsController.CacheVirtualItems(0, 2);
        //}

        [Test]
        public void Smaller_cache_than_number_of_test_runs()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(1, 3);
        }

        //[Test]
        //public void Retrieve_item_when_test_is_selected()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
        //    testController.Stub(tc => tc.Model).Return(testTreeModel);
        //    var root = Root;
        //    testTreeModel.Stub(ttm => ttm.Root).Return(root);
        //    var testResultsController = new TestResultsController(testController);
        //    var test3 = (TestTreeNode)root.Nodes[0].Nodes[2];
        //    var selectedTests = new List<TestTreeNode>(new[] { test3 });
        //    //testController.Stub(tc => tc.SelectedTests).Return(selectedTests).Repeat.Any();

        //    var listViewItem = testResultsController.RetrieveVirtualItem(0);

        //    Assert.AreEqual("test3", listViewItem.Text);
        //    Assert.AreEqual(2, listViewItem.ImageIndex); // passed
        //    Assert.AreEqual(TestKinds.Test, listViewItem.SubItems[1].Text); // testkind
        //    Assert.AreEqual("0.200", listViewItem.SubItems[2].Text); // duration
        //    Assert.AreEqual("2", listViewItem.SubItems[3].Text); // assert count
        //    Assert.AreEqual("", listViewItem.SubItems[4].Text); // code ref
        //    Assert.AreEqual("", listViewItem.SubItems[5].Text); // file name
        //    Assert.AreEqual(0, listViewItem.IndentCount);
        //}

        //[SyncTest]
        //public void Count_items_when_test_is_selected()
        //{
        //    var testController = MockRepository.GenerateStub<ITestController>();
        //    testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
        //    var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
        //    testController.Stub(tc => tc.Model).Return(testTreeModel);
        //    var root = Root;
        //    testTreeModel.Stub(ttm => ttm.Root).Return(root);
        //    var test3 = (TestTreeNode)root.Nodes[0].Nodes[2];
        //    var selectedTests = new List<TestTreeNode>(new[] { test3 });
        //    testController.Stub(tc => tc.SelectedTests).Return(selectedTests).Repeat.Any();
        //    var optionsController = MockRepository.GenerateStub<IOptionsController>();
        //    var testResultsController = new TestResultsController(testController, optionsController);
        //    var resultsCountFlag = false;
        //    var firstTime = true;
        //    testResultsController.PropertyChanged += (sender, e) =>
        //    {
        //        if (e.PropertyName != "ResultsCount")
        //            return;

        //        if (firstTime)
        //        {
        //            Assert.AreEqual(0, testResultsController.ResultsCount);
        //            firstTime = false;
        //        }
        //        else
        //        {
        //            Assert.AreEqual(1, testResultsController.ResultsCount);
        //            resultsCountFlag = true;
        //        }
        //    };

        //    testController.Raise(tc => tc.PropertyChanged += null, testController, new PropertyChangedEventArgs("SelectedTests"));
        //    Assert.AreEqual(true, resultsCountFlag);
        //}

        private static TestTreeNode Root
        {
            get
            {
                var root = new TestDataNode(new TestData("root", "root", "root") { Metadata = { { MetadataKeys.TestKind, TestKinds.Root } } });

                var fixture = new TestDataNode(new TestData("fixture", "fixture", "fixture") { Metadata = { { MetadataKeys.TestKind, TestKinds.Fixture } } });
                root.Nodes.Add(fixture);

                var test1 = new TestDataNode(new TestData("test1", "test1", "test1") { Metadata = { { MetadataKeys.TestKind, TestKinds.Test } } });
                fixture.Nodes.Add(test1);
                var testStepRun1 = new TestStepRun(new TestStepData("test1", "test1", "test1", "test1") { Metadata = { { MetadataKeys.TestKind, TestKinds.Test } } })
                {
                    Result = new TestResult
                    {
                        AssertCount = 5,
                        DurationInSeconds = 0.5,
                        Outcome = TestOutcome.Passed
                    }
                };
                test1.AddTestStepRun(testStepRun1);

                var test2 = new TestDataNode(new TestData("test2", "test2", "test2") { Metadata = { { MetadataKeys.TestKind, TestKinds.Test } } });
                fixture.Nodes.Add(test2);
                var testStepRun2 = new TestStepRun(new TestStepData("test2", "test2", "test2", "test2") { Metadata = { { MetadataKeys.TestKind, TestKinds.Test } } })
                {
                    Result = new TestResult
                    {
                        AssertCount = 1,
                        DurationInSeconds = 0.1,
                        Outcome = TestOutcome.Failed
                    }
                };
                test2.AddTestStepRun(testStepRun2);

                var test3 = new TestDataNode(new TestData("test3", "test3", "test3") { Metadata = { { MetadataKeys.TestKind, TestKinds.Test } } });
                fixture.Nodes.Add(test3);
                var testStepRun3 = new TestStepRun(new TestStepData("test3", "test3", "test3", "test3") { Metadata = { { MetadataKeys.TestKind, TestKinds.Test } } })
                {
                    Result = new TestResult
                    {
                        AssertCount = 2,
                        DurationInSeconds = 0.2,
                        Outcome = TestOutcome.Inconclusive
                    }
                };
                test3.AddTestStepRun(testStepRun3);

                return root;
            }
        }

        [Test]
        public void Sort_list_by_int()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(0, 3);
            testResultsController.SetSortColumn(3);

            Assert.AreEqual("test2", testResultsController.RetrieveVirtualItem(0).Text);
            Assert.AreEqual("test3", testResultsController.RetrieveVirtualItem(1).Text);
            Assert.AreEqual("test1", testResultsController.RetrieveVirtualItem(2).Text);
        }

        [Test]
        public void Sort_list_by_string_desc()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(0, 3);
            testResultsController.SetSortColumn(0);
            testResultsController.SetSortColumn(0);

            Assert.AreEqual("test3", testResultsController.RetrieveVirtualItem(0).Text);
            Assert.AreEqual("test2", testResultsController.RetrieveVirtualItem(1).Text);
            Assert.AreEqual("test1", testResultsController.RetrieveVirtualItem(2).Text);
        }

        [Test]
        public void Sort_list_by_double()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(0, 3);
            testResultsController.SetSortColumn(2);

            Assert.AreEqual("test2", testResultsController.RetrieveVirtualItem(0).Text);
            Assert.AreEqual("test3", testResultsController.RetrieveVirtualItem(1).Text);
            Assert.AreEqual("test1", testResultsController.RetrieveVirtualItem(2).Text);
        }

        [Test]
        public void Cache_miss_low()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(1, 2);
            var listViewItem = testResultsController.RetrieveVirtualItem(0);

            Assert.AreEqual("test1", listViewItem.Text);
        }

        [Test]
        public void Cache_miss_high()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(0, 1);
            var listViewItem = testResultsController.RetrieveVirtualItem(2);

            Assert.AreEqual("test3", listViewItem.Text);
        }    
    }
}
