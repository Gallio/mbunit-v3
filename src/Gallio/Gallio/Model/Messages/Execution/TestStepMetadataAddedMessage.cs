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
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a test step has dynamically added metadata to itself.
    /// </summary>
    [Serializable]
    public class TestStepMetadataAddedMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the test step, not null.
        /// </summary>
        public string StepId { get; set; }
        
        /// <summary>
        /// Gets or sets the metadata key, not null.
        /// </summary>
        /// <seealso cref="MetadataKeys"/>
        public string MetadataKey { get; set; }

        /// <summary>
        /// Gets or sets the metadata value, not null.
        /// </summary>
        public string MetadataValue { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("stepId", StepId);
            ValidationUtils.ValidateNotNull("metadataKey", MetadataKey);
            ValidationUtils.ValidateNotNull("metadataValue", MetadataValue);
        }

        /// <inheritdoc />
        public override Message Normalize()
        {
            string normalizedStepId = ModelNormalizationUtils.NormalizeTestComponentId(StepId);
            string normalizedMetadataKey = ModelNormalizationUtils.NormalizeMetadataKey(MetadataKey);
            string normalizedMetadataValue = ModelNormalizationUtils.NormalizeMetadataValue(MetadataValue);

            if (ReferenceEquals(StepId, normalizedStepId)
                && ReferenceEquals(MetadataKey, normalizedMetadataKey)
                && ReferenceEquals(MetadataValue, normalizedMetadataValue))
                return this;

            return new TestStepMetadataAddedMessage()
            {
                StepId = normalizedStepId,
                MetadataKey = normalizedMetadataKey,
                MetadataValue = normalizedMetadataValue
            };
        }
    }
}
