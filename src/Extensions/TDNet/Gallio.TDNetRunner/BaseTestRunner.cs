using System;
using Gallio.TDNetRunner.Core;

namespace Gallio.TDNetRunner
{
    public abstract class BaseTestRunner : IDisposable
    {
        private IProxyTestRunner testRunner;

        public BaseTestRunner()
        {
            testRunner = new LocalProxyTestRunner();
        }

        /// <inheritdoc />
        internal protected IProxyTestRunner TestRunner
        {
            get
            {
                return testRunner;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                testRunner = value;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            testRunner.Dispose();
        }
    }
}
