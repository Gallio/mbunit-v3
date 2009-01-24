using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
{
    public class MemoizerTest
    {
        [Test]
        public void WhenValueNotPresentPopulatesAndMemoizesIt()
        {
            Memoizer<int> memoizer = new Memoizer<int>();
            Assert.AreEqual(42, memoizer.Memoize(() => 42));
            Assert.AreEqual(42, memoizer.Memoize(() => { throw new InvalidOperationException("Should not be called"); }));
        }
    }
}
