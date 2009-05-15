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
using System.ComponentModel;
using System.Drawing;
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Tests.Utilities;
using Gallio.Icarus.Utilities;
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [MbUnit.Framework.Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestResultsController))]
    internal class TestResultsControllerTest
    {
        [Test]
        public void Ctor_should_throw_if_TestController_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TestResultsController(null, null, null));
        }

        [Test]
        public void Ctor_should_throw_if_OptionsController_is_null()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            Assert.Throws<ArgumentNullException>(() => new TestResultsController(testController, null, null));
        }

        [Test]
        public void Ctor_should_throw_if_TaskManager_is_null()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            Assert.Throws<ArgumentNullException>(() => new TestResultsController(testController, optionsController, null));
        }

        [Test]
        public void TestStatusBarStyle_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            string testStatusBarStyle = "testStatusBarStyle";
            optionsController.TestStatusBarStyle = testStatusBarStyle;
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(testStatusBarStyle, testResultsController.TestStatusBarStyle);
        }

        [Test]
        public void PassedColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var passedColour = Color.Green;
            optionsController.PassedColor = passedColour;
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(passedColour, testResultsController.PassedColor);
        }

        [Test]
        public void FailedColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var failedColour = Color.Red;
            optionsController.FailedColor = failedColour;
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(failedColour, testResultsController.FailedColor);
        }

        [Test]
        public void InconclusiveColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var inconclusiveColour = Color.Gold;
            optionsController.InconclusiveColor = inconclusiveColour;
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(inconclusiveColour, testResultsController.InconclusiveColor);
        }

        [Test]
        public void SkippedColor_should_come_from_OptionsController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var skippedColour = Color.Gold;
            optionsController.SkippedColor = skippedColour;
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(skippedColour, testResultsController.SkippedColor);
        }

        [Test]
        public void PassedTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int passedTestCount = 5;
            testController.Stub(tc => tc.Passed).Return(passedTestCount);
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(passedTestCount, testResultsController.PassedTestCount);
        }

        [Test]
        public void FailedTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int failedTestCount = 5;
            testController.Stub(tc => tc.Failed).Return(failedTestCount);
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(failedTestCount, testResultsController.FailedTestCount);
        }

        [Test]
        public void SkippedTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int skippedTestCount = 5;
            testController.Stub(tc => tc.Skipped).Return(skippedTestCount);
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(skippedTestCount, testResultsController.SkippedTestCount);
        }

        [Test]
        public void InconclusiveTestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int inconclusiveTestCount = 5;
            testController.Stub(tc => tc.Inconclusive).Return(inconclusiveTestCount);
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(inconclusiveTestCount, testResultsController.InconclusiveTestCount);
        }

        [Test]
        public void ElapsedTime_should_be_zero_before_test_run_starts()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(new TimeSpan(), testResultsController.ElapsedTime);
        }

        [Test]
        public void ElapsedTime_should_be_greater_than_zero_once_test_run_has_started()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);

            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());
        }

        [Test]
        public void ElapsedTime_should_stop_increasing_once_test_run_has_finished()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            
            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);
            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());

            testController.Raise(tc => tc.RunFinished += null, testController, EventArgs.Empty);
            var elapsed = testResultsController.ElapsedTime;
            Assert.AreEqual(elapsed, testResultsController.ElapsedTime);
        }

        [SyncTest]
        public void ResultsCount_is_set_to_zero_when_test_run_is_started_and_notification_is_raised()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var propertyChangedFlag = false;
            testResultsController.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName != "ResultsCount")
                    return;

                Assert.AreEqual(0, testResultsController.ResultsCount);
                propertyChangedFlag = true;
            };

            testController.Raise(tc => tc.RunStarted += null, testController, EventArgs.Empty);

            Assert.AreEqual(true, propertyChangedFlag);
        }

        [Test]
        public void TestCount_should_come_from_TestController()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            const int testCount = 5;
            testController.Stub(tc => tc.TestCount).Return(testCount);
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            Assert.AreEqual(testCount, testResultsController.TestCount);
        }

        [SyncTest]
        public void ExploreStarted_from_TestController_should_reset_results()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var resultsCountFlag = false;
            var elapsedTimeFlag = false;
            testResultsController.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case "ResultsCount":
                        Assert.AreEqual(0, testResultsController.TestCount);
                        resultsCountFlag = true;
                        break;

                    case "ElapsedTime":
                        Assert.AreEqual(new TimeSpan(), testResultsController.ElapsedTime);
                        elapsedTimeFlag = true;
                        break;
                }
            };

            testController.Raise(tc => tc.ExploreStarted += null, testController, EventArgs.Empty);
            Assert.AreEqual(true, resultsCountFlag);
            Assert.AreEqual(true, elapsedTimeFlag);
        }

        [SyncTest]
        public void ElapsedTime_should_be_greater_than_zero_when_explore_has_finished()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
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

        [SyncTest]
        public void PropertyChanged_from_TestController_should_bubble_up()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var propChangedFlag = false;
            var eventArgs = new PropertyChangedEventArgs("test");
            testResultsController.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual(eventArgs, e);
                propChangedFlag = true;
            };

            testController.Raise(tc => tc.PropertyChanged += null, testController, eventArgs);

            Assert.AreEqual(true, propChangedFlag);
        }

        [SyncTest]
        public void PropertyChanged_from_OptionsController_should_bubble_up()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var propChangedFlag = false;
            var eventArgs = new PropertyChangedEventArgs("test");
            testResultsController.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual(eventArgs, e);
                propChangedFlag = true;
            };

            optionsController.Raise(oc => oc.PropertyChanged += null, testController, eventArgs);

            Assert.AreEqual(true, propChangedFlag);
        }

        [SyncTest]
        public void If_test_model_root_is_null_CountResults_should_return_zero()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            var root = new TestTreeNode("root", "root", "root");
            testTreeModel.Stub(ttm => ttm.Root).Return(root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = new TestTaskManager();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var resultsCountFlag = false;
            testResultsController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "ResultsCount")
                    return;

                Assert.AreEqual(0, testResultsController.ResultsCount);
                resultsCountFlag = true;
            };

            testController.Raise(tc => tc.TestStepFinished += null, testController,
                new TestStepFinishedEventArgs(new Report(),
                new TestData("id", "name", "fullName"),
                new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))));

            Assert.AreEqual(true, resultsCountFlag);
        }

        [SyncTest]
        public void CountResults_should_return_the_number_of_TestStepRuns_in_the_model()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = new TestTaskManager();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var resultsCountFlag = false;
            testResultsController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "ResultsCount")
                    return;

                Assert.AreEqual(3, testResultsController.ResultsCount);
                resultsCountFlag = true;
            };

            testController.Raise(tc => tc.TestStepFinished += null, testController, 
                new TestStepFinishedEventArgs(new Report(), 
                new TestData("id", "name", "fullName"), 
                new TestStepRun(new TestStepData("id", "name", "fullName", "testId"))));

            Assert.AreEqual(true, resultsCountFlag);
        }

        [Test]
        public void Cache_and_then_retrieve_item()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(0, 3);
            var listViewItem = testResultsController.RetrieveVirtualItem(0);

            Assert.AreEqual("test1", listViewItem.Text);
            Assert.AreEqual(0, listViewItem.ImageIndex); // passed
            Assert.AreEqual("test", listViewItem.SubItems[1].Text); // testkind
            Assert.AreEqual("0.500", listViewItem.SubItems[2].Text); // duration
            Assert.AreEqual("5", listViewItem.SubItems[3].Text); // assert count
            Assert.AreEqual("", listViewItem.SubItems[4].Text); // code ref
            Assert.AreEqual("", listViewItem.SubItems[5].Text); // assembly name
            Assert.AreEqual(2, listViewItem.IndentCount);
        }

        [Test]
        public void Cache_and_then_narrow_viewport()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(0, 3);
            testResultsController.CacheVirtualItems(0, 2);
        }

        [Test]
        public void Smaller_cache_than_number_of_test_runs()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(1, 3);
        }

        [Test]
        public void Retrieve_item_when_test_is_selected()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            var root = Root;
            testTreeModel.Stub(ttm => ttm.Root).Return(root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var test3 = (TestTreeNode)root.Nodes[0].Nodes[2];
            var selectedTests = new List<TestTreeNode>(new[] { test3 });
            testController.Stub(tc => tc.SelectedTests).Return(selectedTests).Repeat.Any();

            var listViewItem = testResultsController.RetrieveVirtualItem(0);

            Assert.AreEqual("test3", listViewItem.Text);
            Assert.AreEqual(2, listViewItem.ImageIndex); // passed
            Assert.AreEqual("test", listViewItem.SubItems[1].Text); // testkind
            Assert.AreEqual("0.200", listViewItem.SubItems[2].Text); // duration
            Assert.AreEqual("2", listViewItem.SubItems[3].Text); // assert count
            Assert.AreEqual("", listViewItem.SubItems[4].Text); // code ref
            Assert.AreEqual("", listViewItem.SubItems[5].Text); // assembly name
            Assert.AreEqual(0, listViewItem.IndentCount);
        }

        [SyncTest]
        public void Count_items_when_test_is_selected()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            var root = Root;
            testTreeModel.Stub(ttm => ttm.Root).Return(root);
            var test3 = (TestTreeNode)root.Nodes[0].Nodes[2];
            var selectedTests = new List<TestTreeNode>(new[] { test3 });
            testController.Stub(tc => tc.SelectedTests).Return(selectedTests).Repeat.Any();
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = new TestTaskManager();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);
            var resultsCountFlag = false;
            testResultsController.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != "ResultsCount")
                    return;

                Assert.AreEqual(1, testResultsController.ResultsCount);
                resultsCountFlag = true;
            };

            testController.Raise(tc => tc.PropertyChanged += null, testController, new PropertyChangedEventArgs("SelectedTests"));
            Assert.AreEqual(true, resultsCountFlag);
        }

        private static TestTreeNode Root
        {
            get
            {
                var root = new TestTreeNode("root", "root", "root");

                var fixture = new TestTreeNode("fixture", "fixture", "fixture");
                root.Nodes.Add(fixture);

                var test1 = new TestTreeNode("test1", "test1", "test");
                fixture.Nodes.Add(test1);
                var testStepRun1 = new TestStepRun(new TestStepData("test1", "test1", "test1", "test1"))
                {
                    Result = new TestResult
                    {
                        AssertCount = 5,
                        Duration = 0.5,
                        Outcome = TestOutcome.Passed
                    }
                };
                test1.AddTestStepRun(testStepRun1);

                var test2 = new TestTreeNode("test2", "test2", "test");
                fixture.Nodes.Add(test2);
                var testStepRun2 = new TestStepRun(new TestStepData("test2", "test2", "test2", "test2"))
                {
                    Result = new TestResult
                    {
                        AssertCount = 1,
                        Duration = 0.1,
                        Outcome = TestOutcome.Failed
                    }
                };
                test2.AddTestStepRun(testStepRun2);

                var test3 = new TestTreeNode("test3", "test3", "test");
                fixture.Nodes.Add(test3);
                var testStepRun3 = new TestStepRun(new TestStepData("test3", "test3", "test3", "test3"))
                {
                    Result = new TestResult
                    {
                        AssertCount = 2,
                        Duration = 0.2,
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
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(0, 3);
            testResultsController.SetSortColumn(3);

            Assert.AreEqual("test2", testResultsController.RetrieveVirtualItem(0).Text);
            Assert.AreEqual("test3", testResultsController.RetrieveVirtualItem(1).Text);
            Assert.AreEqual("test1", testResultsController.RetrieveVirtualItem(2).Text);
        }

        [Test]
        public void Sort_list_by_string_desc()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

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
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(0, 3);
            testResultsController.SetSortColumn(2);

            Assert.AreEqual("test2", testResultsController.RetrieveVirtualItem(0).Text);
            Assert.AreEqual("test3", testResultsController.RetrieveVirtualItem(1).Text);
            Assert.AreEqual("test1", testResultsController.RetrieveVirtualItem(2).Text);
        }

        [Test]
        public void Cache_miss_low()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(1, 2);
            var listViewItem = testResultsController.RetrieveVirtualItem(0);

            Assert.AreEqual("test1", listViewItem.Text);
        }

        [Test]
        public void Cache_miss_high()
        {
            var testController = MockRepository.GenerateStub<ITestController>();
            testController.Stub(tc => tc.SelectedTests).Return(new BindingList<TestTreeNode>());
            var testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testController.Stub(tc => tc.Model).Return(testTreeModel);
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            var optionsController = MockRepository.GenerateStub<IOptionsController>();
            var taskManager = MockRepository.GenerateStub<ITaskManager>();
            var testResultsController = new TestResultsController(testController, optionsController, taskManager);

            testResultsController.CacheVirtualItems(0, 1);
            var listViewItem = testResultsController.RetrieveVirtualItem(2);

            Assert.AreEqual("test3", listViewItem.Text);
        }
    }
}
