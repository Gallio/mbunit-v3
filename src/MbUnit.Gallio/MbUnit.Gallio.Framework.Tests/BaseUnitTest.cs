extern alias MbUnit2;
using MbUnit2::MbUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;

namespace MbUnit.Tests
{
    /// <summary>
    /// Base unit test.
    /// All unit tests that require certain common facilities like Mock Objects
    /// inherit from this class.
    /// </summary>
    [TestFixture]
    public abstract class BaseUnitTest
    {
        private MockRepository mocks;

        /// <summary>
        /// Gets the mock object repository.
        /// </summary>
        public MockRepository Mocks
        {
            get
            {
                if (mocks == null)
                    mocks = new MockRepository();

                return mocks;
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (mocks != null)
            {
                try
                {
                    mocks.ReplayAll();
                    mocks.VerifyAll();
                }
                finally
                {
                    mocks = null;
                }
            }
        }
    }
}
