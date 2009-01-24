using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    public class KeyedMemoizerTest
    {
        [Test]
        public void WhenValueNotPresentPopulatesAndMemoizesItByKey()
        {
            KeyedMemoizer<string, int> memoizer = new KeyedMemoizer<string, int>();
            Assert.AreEqual(42, memoizer.Memoize("42", () => 42));
            Assert.AreEqual(42, memoizer.Memoize("42", () => { throw new InvalidOperationException("Should not be called"); }));

            Assert.AreEqual(31, memoizer.Memoize("31", () => 31));
            Assert.AreEqual(31, memoizer.Memoize("31", () => { throw new InvalidOperationException("Should not be called"); }));

            Assert.AreEqual(-1, memoizer.Memoize(null, () => -1));

            Assert.AreEqual(42, memoizer.Memoize("42", () => { throw new InvalidOperationException("Should not be called"); }));
            Assert.AreEqual(-1, memoizer.Memoize(null, () => { throw new InvalidOperationException("Should not be called"); }));
            Assert.AreEqual(31, memoizer.Memoize("31", () => { throw new InvalidOperationException("Should not be called"); }));
        }
    }
}
