// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Model.Execution;
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// An error test object is used as a placeholder in the test tree whenever
    /// an error is encountered during test exploration.  An error test has
    /// no parameters.  When executed, it just emits a runtime error.
    /// </summary>
    public class ErrorTest : BaseTest
    {
        /// <summary>
        /// Creates an error test.
        /// </summary>
        /// <param name="codeElement">The code element that was being processed
        /// when the error was encountered, or null if unknown</param>
        /// <param name="shortDescription">The short description of the error to use as part of the test name</param>
        /// <param name="longDescription">The long description of the error to use as metadata, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="shortDescription"/> is null</exception>
        public ErrorTest(ICodeElementInfo codeElement, string shortDescription, string longDescription)
            : base(String.Format("Error!  {0}", shortDescription), codeElement)
        {
            if (shortDescription == null)
                throw new ArgumentNullException("shortDescription");

            Kind = TestKinds.Error;

            if (longDescription != null)
                Metadata.SetValue(MetadataKeys.Description, longDescription);
        }

        /// <inheritdoc />
        public override Factory<ITestController> TestControllerFactory
        {
            get { return CreateTestController; }
        }

        private static ITestController CreateTestController()
        {
            return new ErrorTestController();
        }
    }
}
