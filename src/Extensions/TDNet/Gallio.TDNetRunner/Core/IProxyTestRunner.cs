// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Runtime.Remoting;
using Gallio.Loader;
using Gallio.TDNetRunner.Facade;

namespace Gallio.TDNetRunner.Core
{
    /// <summary>
    /// Provides test execution facilities.
    /// </summary>
    public interface IProxyTestRunner : IDisposable
    {
        /// <summary>
        /// Aborts the test run.
        /// </summary>
        /// <exception cref="ServerException">Thrown if the operation could not be performed.</exception>
        void Abort();

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="testListener">The test listener.</param>
        /// <param name="assemblyPath">The test assembly.</param>
        /// <param name="cref">The code reference for the test to run.</param>
        /// <param name="facadeOptions">The TDNet options.</param>
        /// <exception cref="ServerException">Thrown if the operation could not be performed.</exception>
        FacadeTestRunState Run(IFacadeTestListener testListener, string assemblyPath, string cref, FacadeOptions facadeOptions);
    }
}
