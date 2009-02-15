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
using Gallio.Runner;
using Gallio.Runner.Drivers;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Provides support for creating <see cref="ITestDriver"/> instances inside an AutoCAD process.
    /// </summary>
    public class AcadTestDriverFactory : ITestDriverFactory
    {
        private IAcadProcessFactory processFactory;

        /// <summary>
        /// Intializes a new AutoCAD test driver factory.
        /// </summary>
        /// <param name="processFactory">A <see cref="IAcadProcessFactory"/> instance.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="processFactory"/> is null.</exception>
        public AcadTestDriverFactory(IAcadProcessFactory processFactory)
        {
            if (processFactory == null)
                throw new ArgumentNullException("processFactory");
            this.processFactory = processFactory;
        }

        /// <inheritdoc/>
        public ITestDriver CreateTestDriver()
        {
            IAcadProcess process = processFactory.CreateProcess();
            if (process == null)
                throw new RunnerException("Unable to create ITestDriver because IAcadProcessFactory returned a null IAcadProcess.");

            IRemoteTestDriver remoteDriver = process.GetRemoteTestDriver();
            return new AcadTestDriver(process, remoteDriver);
        }
    }
}
