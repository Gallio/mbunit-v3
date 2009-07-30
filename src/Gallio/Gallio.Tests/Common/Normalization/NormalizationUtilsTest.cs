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
using Gallio.Common;
using Gallio.Common.Normalization;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Normalization
{
    [TestsOn(typeof(NormalizationUtils))]
    public class NormalizationUtilsTest
    {
        [Test]
        public void NormalizeCollection_WhenCollectionIsNull_ReturnsNull()
        {
            List<string> collection = null;
            Gallio.Common.Func<IList<string>> collectionFactory = () => new List<string>();
            Gallio.Common.Func<string, string> normalize = x => x + "*";
            EqualityComparison<string> compare = ReferenceEquals;

            IList<string> normalizedCollection = NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare);

            Assert.IsNull(normalizedCollection);
        }

        [Test]
        public void NormalizeCollection_WhenCollectionFactoryIsNull_Throws()
        {
            List<string> collection = null;
            Gallio.Common.Func<IList<string>> collectionFactory = null;
            Gallio.Common.Func<string, string> normalize = x => x + "*";
            EqualityComparison<string> compare = ReferenceEquals;

            Assert.Throws<ArgumentNullException>(() => NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare));
        }

        [Test]
        public void NormalizeCollection_WhenNormalizeIsNull_Throws()
        {
            List<string> collection = null;
            Gallio.Common.Func<IList<string>> collectionFactory = () => new List<string>();
            Gallio.Common.Func<string, string> normalize = null;
            EqualityComparison<string> compare = ReferenceEquals;

            Assert.Throws<ArgumentNullException>(() => NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare));
        }

        [Test]
        public void NormalizeCollection_WhenCompareIsNull_Throws()
        {
            List<string> collection = null;
            Gallio.Common.Func<IList<string>> collectionFactory = () => new List<string>();
            Gallio.Common.Func<string, string> normalize = x => x + "*";
            EqualityComparison<string> compare = null;

            Assert.Throws<ArgumentNullException>(() => NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare));
        }

        [Test]
        public void NormalizeCollection_WhenAllNormalizedValuesAreUnchanged_ReturnsTheSameCollection()
        {
            IList<string> collection = new[] { "abc", "def", "ghi" };
            Gallio.Common.Func<IList<string>> collectionFactory = () => new List<string>();
            Gallio.Common.Func<string, string> normalize = x => x;
            EqualityComparison<string> compare = ReferenceEquals;

            IList<string> normalizedCollection = NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare);

            Assert.AreSame(collection, normalizedCollection);
        }

        [Test]
        public void NormalizeCollection_WhenAllNormalizedValuesAreChanged_ReturnsANewCollectionOfNormalizedValues()
        {
            IList<string> collection = new[] { "abc", "def", "ghi" };
            Gallio.Common.Func<IList<string>> collectionFactory = () => new List<string>();
            Gallio.Common.Func<string, string> normalize = x => x + "*";
            EqualityComparison<string> compare = ReferenceEquals;

            IList<string> normalizedCollection = NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare);

            Assert.AreNotSame(collection, normalizedCollection);
            Assert.AreElementsEqual(new[] { "abc*", "def*", "ghi*" }, normalizedCollection);
        }

        [Test]
        public void NormalizeCollection_WhenSecondAndThirdNormalizedValuesAreChanged_ReturnsANewCollectionOfNormalizedValues()
        {
            IList<string> collection = new[] { "abc", "def", "ghi" };
            Gallio.Common.Func<IList<string>> collectionFactory = () => new List<string>();
            Gallio.Common.Func<string, string> normalize = x => x == "abc" ? x : x + "*";
            EqualityComparison<string> compare = ReferenceEquals;

            IList<string> normalizedCollection = NormalizationUtils.NormalizeCollection<IList<string>, string>(
                collection, collectionFactory, normalize, compare);

            Assert.AreNotSame(collection, normalizedCollection);
            Assert.AreElementsEqual(new[] { "abc", "def*", "ghi*" }, normalizedCollection);
        }

        [Test]
        public void NormalizeString_WhenStringIsNull_ReturnsNull()
        {
            Assert.IsNull(NormalizationUtils.NormalizeString(null, c => true, c => ""));
        }

        [Test]
        public void NormalizeXmlText_WhenStringIsEmpty_ReturnsEmpty()
        {
            Assert.IsEmpty(NormalizationUtils.NormalizeString("", c => true, c => ""));
        }

        [Test]
        public void NormalizeString_WhenValidIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => NormalizationUtils.NormalizeString("foo", null, c => ""));
        }

        [Test]
        public void NormalizeString_WhenReplaceIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => NormalizationUtils.NormalizeString("foo", c => true, null));
        }

        [Test]
        public void NormalizeXmlText_WhenTextIsNull_ReturnsNull()
        {
            Assert.IsNull(NormalizationUtils.NormalizeXmlText(null));
        }

        [Test]
        public void NormalizeXmlText_WhenTextIsEmpty_ReturnsEmpty()
        {
            Assert.IsEmpty(NormalizationUtils.NormalizeXmlText(""));
        }

        [Test]
        [Row("abc DEF 123")]
        [Row("\t\r\n\u0020\ud7ff\ue000\ufffd", Description = "Range extremes.")]
        [Row("\ud800\udc00", Description = "Surrogate pair.")]
        public void NormalizeXmlText_WhenTextIsAlreadyNormalized_ReturnsTheSameInstance(string text)
        {
            Assert.AreSame(text, NormalizationUtils.NormalizeXmlText(text));
        }

        [Test]
        [Row("\0", "?")]
        [Row("\ufffe", "?")]
        [Row("\uffff", "?")]
        public void NormalizeXmlText_WhenTextIsNotNormalized_ReturnsNormalizedInstance(string text,
            string normalizedText)
        {
            Assert.AreEqual(normalizedText, NormalizationUtils.NormalizeXmlText(text));
        }
    }
}
