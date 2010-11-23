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
using Gallio.MbUnitCppAdapter.Model.Bridge;
using Gallio.Model;
using Gallio.Model.Tree;

namespace Gallio.MbUnitCppAdapter.Model
{
    /// <summary>
    /// Represents an MbUnitCpp test.
    /// </summary>
    public class MbUnitCppTest : Test
    {
        private readonly TestInfoData testInfoData;

        /// <summary>
        /// Gets MbUnitCpp specific information about the current test.
        /// </summary>
        public TestInfoData TestInfoData
        {
            get
            {
                return testInfoData;
            }
        }

        /// <summary>
        /// Constructs an MbUnitCpp tests.
        /// </summary>
        /// <param name="testInfoData">Information about the test.</param>
        /// <param name="resolver"></param>
        public MbUnitCppTest(TestInfoData testInfoData, IStringResolver resolver)
            : base(testInfoData.Name, testInfoData.FakeCodeElement)
        {
            this.testInfoData = testInfoData;
            Id = testInfoData.GetId();
            Kind = testInfoData.IsTestFixture ? TestKinds.Fixture : TestKinds.Test;
            IsTestCase = !testInfoData.IsTestFixture;
            Metadata.AddAll(testInfoData.GetMetadata(resolver));
        }
    }
}
