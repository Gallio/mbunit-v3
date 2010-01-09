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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model.Environments
{
    /// <summary>
    /// The test environment configures global behavior related to preparing
    /// the test execution environment for running tests and tearing them down.
    /// </summary>
    public interface ITestEnvironment
    {
        /// <summary>
        /// Sets up the current test AppDomain.
        /// </summary>
        /// <returns>Returns an object that when disposed causes the test environment to be torn down, or null if none.</returns>
        IDisposable SetUpAppDomain();

        /// <summary>
        /// Sets up the current test Thread.
        /// </summary>
        /// <returns>Returns an object that when disposed causes the test environment to be torn down, or null if none.</returns>
        IDisposable SetUpThread();
    }
}
