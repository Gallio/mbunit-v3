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

extern alias MbUnit2;
using Gallio.MbUnit2Adapter.Model;
using Gallio.MbUnit2Adapter.TestResources;
using Gallio.MbUnit2Adapter.TestResources.Metadata;
using Gallio.Model;
using Gallio.Tests.Model;
using MbUnit.Framework;

namespace Gallio.MbUnit2Adapter.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(MbUnit2TestExplorer))]
    [Author("Julian", "julian.hidalgo@gallio.org")]
    public class MbUnit2TestExplorerTest : BaseTestExplorerTest<SimpleTest>
    {
        protected override ITestFramework CreateFramework()
        {
            return new MbUnit2TestFramework();
        }

        [Test]
        public void MetadataImport_AuthorName()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(AuthorNameSample).Name);
            Assert.AreEqual("joe", test.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual(null, test.Metadata.GetValue(MetadataKeys.AuthorEmail));
            Assert.AreEqual(null, test.Metadata.GetValue(MetadataKeys.AuthorHomepage));
        }

        [Test]
        public void MetadataImport_AuthorNameAndEmail()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(AuthorNameAndEmailSample).Name);
            Assert.AreEqual("joe", test.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual("joe@example.com", test.Metadata.GetValue(MetadataKeys.AuthorEmail));
            Assert.AreEqual(null, test.Metadata.GetValue(MetadataKeys.AuthorHomepage));
        }

        [Test]
        public void MetadataImport_AuthorNameAndEmailAndHomepage()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(AuthorNameAndEmailAndHomepageSample).Name);
            Assert.AreEqual("joe", test.Metadata.GetValue(MetadataKeys.AuthorName));
            Assert.AreEqual("joe@example.com", test.Metadata.GetValue(MetadataKeys.AuthorEmail));
            Assert.AreEqual("http://www.example.com/", test.Metadata.GetValue(MetadataKeys.AuthorHomepage));
        }

        [Test]
        public void MetadataImport_FixtureCategory()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(FixtureCategorySample).Name);
            Assert.AreEqual("samples", test.Metadata.GetValue(MetadataKeys.CategoryName));
        }

        [Test]
        public void MetadataImport_Importance()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(ImportanceSample).Name);
            Assert.AreEqual(MbUnit2::MbUnit.Framework.TestImportance.Critical.ToString(), test.Metadata.GetValue(MetadataKeys.Importance));
        }

        [Test]
        public void MetadataImport_TestsOn()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(TestsOnSample).Name);
            Assert.AreEqual(typeof(TestsOnSample).AssemblyQualifiedName, test.Metadata.GetValue(MetadataKeys.TestsOn));
        }

        [Test]
        public void MetadataImport_FixtureDescription()
        {
            PopulateTestTree();

            MbUnit2Test test = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(FixtureDescriptionSample).Name);
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        }

        [Test]
        public void MetadataImport_TestDescription()
        {
            PopulateTestTree();

            MbUnit2Test fixture = (MbUnit2Test)GetDescendantByName(testModel.RootTest, typeof(TestDescriptionSample).Name);
            MbUnit2Test test = (MbUnit2Test)fixture.Children[0];
            Assert.AreEqual("A sample description.", test.Metadata.GetValue(MetadataKeys.Description));
        } 
    }
}
