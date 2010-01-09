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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using Gallio.Runtime;

namespace Gallio.Runner
{
    /// <summary>
    /// Provides helper functions for test runner tools.
    /// </summary>
    public static class TestRunnerUtils
    {
        /// <summary>
        /// Presents a generated report to the user using the default viewing
        /// application for the report's document type.
        /// </summary>
        /// <param name="reportDocumentPath">The path of the report.</param>
        /// <returns>True if the report document was successfully opened.</returns>
        public static bool ShowReportDocument(string reportDocumentPath)
        {
            if (reportDocumentPath == null)
                throw new ArgumentNullException("reportDocumentPath");

            try
            {
                Process.Start(reportDocumentPath);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a test runner given its factory name.
        /// </summary>
        /// <param name="factoryName">The test runner factory name.</param>
        /// <returns>The test runner.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="factoryName"/> is null.</exception>
        public static ITestRunner CreateTestRunnerByName(string factoryName)
        {
            return RuntimeAccessor.ServiceLocator.Resolve<ITestRunnerManager>().CreateTestRunner(factoryName);
        }
    }
}
