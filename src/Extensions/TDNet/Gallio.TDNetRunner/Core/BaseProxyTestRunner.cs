using System;
using Gallio.Loader;
using Gallio.TDNetRunner.Facade;

namespace Gallio.TDNetRunner.Core
{
    /// <summary>
    /// <para>
    /// Abstract base class for proxy test runners.
    /// </para>
    /// <para>
    /// Wraps exceptions to safely smuggle them back to TDNet.
    /// </para>
    /// </summary>
    public abstract class BaseProxyTestRunner : MarshalByRefObject, IProxyTestRunner
    {
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        public void Abort()
        {
            try
            {
                AbortImpl();
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        public FacadeTestRunState Run(IFacadeTestListener testListener, string assemblyPath, string cref)
        {
            try
            {
                return RunImpl(testListener, assemblyPath, cref);
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        protected abstract void Dispose(bool disposing);
        protected abstract void AbortImpl();
        protected abstract FacadeTestRunState RunImpl(IFacadeTestListener testListener, string assemblyPath, string cref);
    }
}
