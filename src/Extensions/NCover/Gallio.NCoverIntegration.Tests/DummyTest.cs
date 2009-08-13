using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Gallio.NCoverIntegration.Tests
{
    /// <summary>
    /// This is just an extra test fixture for our integration test to run.
    /// </summary>
    [Explicit("Sample")]
    public class DummyTest
    {
        [Test]
        public void Test()
        {
        }
    }
}
