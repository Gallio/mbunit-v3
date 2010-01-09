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

namespace Gallio.Model
{
    /// <summary>
    /// Describes the function of a code element in a test.
    /// </summary>
    public class TestPart
    {
        /// <summary>
        /// Gets whether the part represents a test case or test container.
        /// </summary>
        public bool IsTest
        {
            get { return IsTestCase || IsTestContainer; }
        }

        /// <summary>
        /// Gets or sets whether the part represents a test case.
        /// </summary>
        public bool IsTestCase { get; set; }

        /// <summary>
        /// Gets or sets whether the part represents a test container.
        /// </summary>
        public bool IsTestContainer { get; set; }

        /// <summary>
        /// Gets or sets whether the part represents some contribution to a test
        /// such as a setup or teardown method, a test parameter, or a factory.
        /// </summary>
        public bool IsTestContribution { get; set; }
    }
}
