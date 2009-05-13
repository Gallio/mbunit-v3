using System.Collections.Generic;
using Gallio.Model;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(ImportanceAttribute))]
    internal class ImportanceAttributeTest
    {
        private class TestImportanceAttribute : ImportanceAttribute
        {
            public TestImportanceAttribute(Importance importance) : base(importance)
            { }

            public new IEnumerable<KeyValuePair<string, string>> GetMetadata()
            {
                return base.GetMetadata();
            }
        }

        [Test]
        public void GetMetadata_should_return_Importance()
        {
            const Importance importance = Importance.NoOneReallyCaresAbout;
            var testImportanceAttribute = new TestImportanceAttribute(importance);
            int count = 0;
            foreach (var pair in testImportanceAttribute.GetMetadata())
            {
                Assert.AreEqual(MetadataKeys.Importance, pair.Key);
                Assert.AreEqual(importance.ToString(), pair.Value);
                count++;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void Importance_should_return_value_from_ctor()
        {
            const Importance importance = Importance.NoOneReallyCaresAbout;
            var testImportanceAttribute = new TestImportanceAttribute(importance);
            Assert.AreEqual(importance, testImportanceAttribute.Importance);
        }
    }
}
