using System.Windows.Forms;
using Gallio.Common.Reflection;
using Gallio.Icarus.Models.TestTreeNodes;
using Gallio.Model;
using Gallio.Model.Schema;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Models.TestTreeNodes
{
    [Category("TestTreeNodes"), TestsOn(typeof(TestDataNode))]
    public class TestDataNodeTest
    {
        [Test]
        public void Name_should_be_id_from_test_data()
        {
            var testData = new TestData("id", "name", "fullName");
            
            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(testData.Id, testDataNode.Name);
        }

        [Test]
        public void Text_should_be_name_from_test_data()
        {
            var testData = new TestData("id", "name", "fullName");

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(testData.Name, testDataNode.Text);
        }

        [Test]
        public void Test_kind_should_come_from_metadata_if_available()
        {
            var testData = new TestData("id", "name", "fullName");
            const string testKind = TestKinds.Assembly;
            testData.Metadata.Add(MetadataKeys.TestKind, testKind);

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(testKind, testDataNode.TestKind);
        }

        [Test]
        public void Test_kind_should_be_group_if_metadata_unavailable()
        {
            var testData = new TestData("id", "name", "fullName");

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(TestKinds.Group, testDataNode.TestKind);
        }

        [Test]
        public void File_name_should_come_from_metadata_if_available()
        {
            var testData = new TestData("id", "name", "fullName");
            const string file = "blahblah";
            testData.Metadata.Add(MetadataKeys.File, file);

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(file, testDataNode.FileName);
        }

        [Test]
        public void File_name_should_be_null_if_metadata_unavailable()
        {
            var testData = new TestData("id", "name", "fullName");

            var testDataNode = new TestDataNode(testData);

            Assert.IsNull(testDataNode.FileName);
        }

        [Test]
        public void SourceCodeAvailable_should_be_false_if_code_location_is_unknown()
        {
            var testData = new TestData("id", "name", "fullName")
            {
                CodeLocation = CodeLocation.Unknown
            };

            var testDataNode = new TestDataNode(testData);

            Assert.IsFalse(testDataNode.SourceCodeAvailable);
        }

        [Test]
        public void SourceCodeAvailable_should_be_true_if_code_location_is_set()
        {
            var testData = new TestData("id", "name", "fullName")
            {
                CodeLocation = new CodeLocation("", 1, 1)
            };

            var testDataNode = new TestDataNode(testData);

            Assert.IsTrue(testDataNode.SourceCodeAvailable);
        }

        [Test]
        public void IsTest_should_be_same_as_test_data()
        {
            var testData = new TestData("id", "name", "fullName")
            {
                IsTestCase = true
            };

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(testData.IsTestCase, testDataNode.IsTest);
        }

        [Test]
        public void CheckState_should_be_unchecked_if_test_is_ignored()
        {
            var testData = new TestData("id", "name", "fullName");
            testData.Metadata.Add(MetadataKeys.IgnoreReason, "");

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(CheckState.Unchecked, testDataNode.CheckState);
        }

        [Test]
        public void CheckState_should_be_unchecked_if_test_is_pending()
        {
            var testData = new TestData("id", "name", "fullName");
            testData.Metadata.Add(MetadataKeys.PendingReason, "");

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(CheckState.Unchecked, testDataNode.CheckState);
        }

        [Test]
        public void CheckState_should_be_checked_otherwise()
        {
            var testData = new TestData("id", "name", "fullName");

            var testDataNode = new TestDataNode(testData);

            Assert.AreEqual(CheckState.Checked, testDataNode.CheckState);
        }
    }
}
