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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model;

namespace Gallio.RSpecAdapter.Tests.Integration
{
    [TestFixture]
    [RunSampleFile(@"..\Scripts\nested_example_groups_spec.rb")]
    public class RunNestedExampleGroupsSpec : BaseTestWithSampleRunner
    {
        [Test]
        public void Adapter_GeneratesCorrectTestModel()
        {
            TestModelData testModel = Runner.Report.TestModel;

            Assert.Count(1, testModel.RootTest.Children, "Root test contain top level test.");

            TestData fileTest = testModel.RootTest.Children[0];
            Assert.AreEqual("nested_example_groups_spec", fileTest.Name, "Top level test is named for the file.");
            Assert.EndsWith(fileTest.Metadata.GetValue(MetadataKeys.File), "nested_example_groups_spec.rb", "Top level test has correct file path metadata.");
            Assert.AreEqual("RSpec File", fileTest.Metadata.GetValue(MetadataKeys.TestKind), "Top level test should have correct kind.");
            Assert.Count(1, fileTest.Children, "Top level test contains example group.");

            TestData outerExampleGroupTest = fileTest.Children[0];
            Assert.AreEqual("Outer", outerExampleGroupTest.Name, "Example group is named as in 'describe' syntax.");
            Assert.AreEqual("RSpec Example Group", outerExampleGroupTest.Metadata.GetValue(MetadataKeys.TestKind), "Example group test should have correct kind.");
            Assert.Count(4, outerExampleGroupTest.Children, "Example group test contains other example groups and examples.");

            TestData outerExample1Test = outerExampleGroupTest.Children[0];
            Assert.AreEqual("Outer Example1", outerExample1Test.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", outerExample1Test.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData outerExample2Test = outerExampleGroupTest.Children[1];
            Assert.AreEqual("Outer Example2", outerExample2Test.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", outerExample2Test.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData innerExampleGroup1Test = outerExampleGroupTest.Children[2];
            Assert.AreEqual("Inner1", innerExampleGroup1Test.Name, "Example group is named as in 'describe' syntax.");
            Assert.AreEqual("RSpec Example Group", innerExampleGroup1Test.Metadata.GetValue(MetadataKeys.TestKind), "Example group test should have correct kind.");
            Assert.Count(2, innerExampleGroup1Test.Children, "Example group test contains other example groups and examples.");

            TestData inner1Example1Test = innerExampleGroup1Test.Children[0];
            Assert.AreEqual("Inner1 Example1", inner1Example1Test.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", inner1Example1Test.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData inner1Example2Test = innerExampleGroup1Test.Children[1];
            Assert.AreEqual("Inner1 Example2", inner1Example2Test.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", inner1Example2Test.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData innerExampleGroup2Test = outerExampleGroupTest.Children[3];
            Assert.AreEqual("Inner2", innerExampleGroup2Test.Name, "Example group is named as in 'describe' syntax.");
            Assert.AreEqual("RSpec Example Group", innerExampleGroup2Test.Metadata.GetValue(MetadataKeys.TestKind), "Example group test should have correct kind.");
            Assert.Count(2, innerExampleGroup2Test.Children, "Example group test contains other example groups and examples.");

            TestData inner2Example1Test = innerExampleGroup2Test.Children[0];
            Assert.AreEqual("Inner2 Example1", inner2Example1Test.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", inner2Example1Test.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData inner2Example2Test = innerExampleGroup2Test.Children[1];
            Assert.AreEqual("Inner2 Example2", inner2Example2Test.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", inner2Example2Test.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");
        }
    }
}
