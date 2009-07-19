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
using Gallio.Common.Normalization;
using Gallio.Common.Validation;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a test step has changed lifecycle phase.
    /// </summary>
    [Serializable]
    public class TestStepLifecyclePhaseChangedMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the test step, not null.
        /// </summary>
        public string StepId { get; set; }
        
        /// <summary>
        /// Gets or sets the lifecycle phase name, not null.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        public string LifecyclePhase { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("stepId", StepId);
            ValidationUtils.ValidateNotNull("lifecyclePhase", LifecyclePhase);
        }

        /// <inheritdoc />
        public override Message Normalize()
        {
            string normalizedStepId = ModelNormalizationUtils.NormalizeTestComponentId(StepId);
            string normalizedLifecyclePhase = ModelNormalizationUtils.NormalizeLifecyclePhase(LifecyclePhase);

            if (ReferenceEquals(StepId, normalizedStepId)
                && ReferenceEquals(LifecyclePhase, normalizedLifecyclePhase))
                return this;

            return new TestStepLifecyclePhaseChangedMessage()
            {
                StepId = normalizedStepId,
                LifecyclePhase = normalizedLifecyclePhase
            };
        }
    }
}
