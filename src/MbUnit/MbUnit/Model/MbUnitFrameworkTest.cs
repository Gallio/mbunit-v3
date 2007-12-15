// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio;
using Gallio.Model.Execution;
using MbUnit.Model;
using Gallio.Model;
using MbUnit.Properties;

namespace MbUnit.Model
{
    /// <summary>
    /// The the ancestor of all tests declared by a particular
    /// version of the MbUnit test framework.
    /// </summary>
    public class MbUnitFrameworkTest : MbUnitTest
    {
        private readonly Version version;

        /// <summary>
        /// Initializes the MbUnit framework test model object.
        /// </summary>
        /// <param name="version">The MbUnit framework version</param>
        public MbUnitFrameworkTest(Version version)
            : base(String.Format(Resources.MbUnitFrameworkTemplate_MbUnitGallioFrameworkVersionFormat, version), null)
        {
            this.version = version;

            Kind = ComponentKind.Framework;
        }

        /// <summary>
        /// Gets the MbUnit framework version.
        /// </summary>
        public Version Version
        {
            get { return version; }
        }

        /// <inheritdoc />
        public override Factory<ITestController> TestControllerFactory
        {
            get { return CreateTestController; }
        }

        private static ITestController CreateTestController()
        {
            return new MbUnitTestController();
        }
    }
}