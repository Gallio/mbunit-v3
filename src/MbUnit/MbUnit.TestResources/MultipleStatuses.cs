using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;

namespace MbUnit.TestResources
{
    [TestFixture]
    public class MultipleStatuses
    {
        [Test]
        public void Passed()
        {
        }

        [Test]
        public void Failed()
        {
            throw new TestFailedException();
        }

        [Test]
        public void Inconclusive()
        {
            throw new TestInconclusiveException();
        }

        [Test, Ignore("Skipped")]
        public void Skipped()
        {
        }
    }
}
