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
using Gallio.Common.Reflection;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a test step has started execution and provides its definition.
    /// </summary>
    [Serializable]
    public class TestStepStartedMessage : Message
    {
        [NonSerialized]
        private ICodeElementInfo codeElement;

        /// <summary>
        /// Gets or sets information about the test step that is about to start, not null.
        /// </summary>
        public TestStepData Step { get; set; }
        
        /// <summary>
        /// Gets or sets the code element associated with the test step, or null if none.
        /// </summary>
        public ICodeElementInfo CodeElement
        {
            get { return codeElement; }
            set { codeElement = value; }
        }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("step", Step);
        }

        /// <inheritdoc />
        public override Message Normalize()
        {
            TestStepData normalizedStep = Step.Normalize();

            if (ReferenceEquals(Step, normalizedStep))
                return this;

            return new TestStepStartedMessage()
            {
                Step = normalizedStep,
                CodeElement = codeElement
            };
        }
    }
}
