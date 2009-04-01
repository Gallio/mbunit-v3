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

        public FacadeTestRunState Run(IFacadeTestListener testListener, string assemblyPath, string cref, FacadeOptions facadeOptions)
        {
            try
            {
                return RunImpl(testListener, assemblyPath, cref, facadeOptions);
            }
            catch (Exception ex)
            {
                throw SafeException.Wrap(ex);
            }
        }

        protected abstract void Dispose(bool disposing);
        protected abstract void AbortImpl();
        protected abstract FacadeTestRunState RunImpl(IFacadeTestListener testListener, string assemblyPath, string cref, FacadeOptions facadeOptions);

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
