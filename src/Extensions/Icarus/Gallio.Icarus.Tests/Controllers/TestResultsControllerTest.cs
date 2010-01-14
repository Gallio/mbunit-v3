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
using Gallio.Icarus.Controllers;
using Gallio.Icarus.Events;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controllers
{
    [Category("Controllers"), Author("Graham Hay"), TestsOn(typeof(TestResultsController))]
    public class TestResultsControllerTest
    {
        private TestResultsController testResultsController;
        private ITestTreeModel testTreeModel;

        [SetUp]
        public void EstablishContext()
        {
            testTreeModel = MockRepository.GenerateStub<ITestTreeModel>();
            testTreeModel.Stub(ttm => ttm.Root).Return(Root);
            testResultsController = new TestResultsController(testTreeModel);
        }

        [Test]
        public void ElapsedTime_should_be_zero_before_test_run_starts()
        {
            Assert.AreEqual(new TimeSpan(), testResultsController.ElapsedTime);
        }

        [Test]
        public void ElapsedTime_should_be_greater_than_zero_once_test_run_has_started()
        {
            testResultsController.Handle(new RunStarted());
            testResultsController.Handle(new TestSelectionChanged(new TestTreeNode[0]));

            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());
        }

        [Test, Ignore]
        public void ElapsedTime_should_stop_increasing_once_test_run_has_finished()
        {
            testResultsController.Handle(new RunStarted());
            testResultsController.Handle(new TestSelectionChanged(new TestTreeNode[0]));
            Assert.GreaterThan(testResultsController.ElapsedTime, new TimeSpan());

            testResultsController.Handle(new RunFinished());
            var elapsed = testResultsController.ElapsedTime;
            Assert.AreEqual(elapsed, testResultsController.ElapsedTime);
        }

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

        [Test]
        public void Smaller_cache_than_number_of_test_runs()
        {
            EstablishContext();

            testResultsController.CacheVirtualItems(1, 3);
        }

        private static TestTreeNode Root
        {
            get
            {
                var root = new TestDataNode(new TestData("root", "root", "root")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Root } }
                });

                var fixture = new TestDataNode(new TestData("fixture", "fixture", "fixture")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Fixture } }
                });
                root.Nodes.Add(fixture);

                var test1 = new TestDataNode(new TestData("test1", "test1", "test1")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Test } }
                });
                fixture.Nodes.Add(test1);
                var testStepRun1 = new TestStepRun(new TestStepData("test1", "test1", "test1", "test1")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Test } }
                })
                {
                    Result = new TestResult
                    {
                        AssertCount = 5,
                        DurationInSeconds = 0.5,
                        Outcome = TestOutcome.Passed
                    }
                };
                test1.AddTestStepRun(testStepRun1);

                var test2 = new TestDataNode(new TestData("test2", "test2", "test2")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Test } }
                });
                fixture.Nodes.Add(test2);
                var testStepRun2 = new TestStepRun(new TestStepData("test2", "test2", "test2", "test2")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Test } }
                })
                {
                    Result = new TestResult
                    {
                        AssertCount = 1,
                        DurationInSeconds = 0.1,
                        Outcome = TestOutcome.Failed
                    }
                };
                test2.AddTestStepRun(testStepRun2);

                var test3 = new TestDataNode(new TestData("test3", "test3", "test3")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Test } }
                });
                fixture.Nodes.Add(test3);
                var testStepRun3 = new TestStepRun(new TestStepData("test3", "test3", "test3", "test3")
                {
                    Metadata = { { MetadataKeys.TestKind, TestKinds.Test } }
                })
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
