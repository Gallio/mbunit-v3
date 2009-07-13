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
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Exploration
{
    /// <summary>
    /// Notifies that a test has been discovered by the test exploration process.
    /// </summary>
    [Serializable]
    public class TestDiscoveredMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the parent test, or null if the test is the root.
        /// </summary>
        public string ParentTestId { get; set; }

        /// <summary>
        /// Gets or sets information about the test that was discovered, not null.
        /// </summary>
        public TestData Test { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("test", Test);
        }
    }
}
