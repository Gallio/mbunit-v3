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
