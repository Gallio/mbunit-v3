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
using Gallio.Common.Markup;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Execution
{
    /// <summary>
    /// Notifies that a section block has been started within a test step log stream.
    /// </summary>
    [Serializable]
    public class TestStepLogStreamBeginSectionBlockMessage : Message
    {
        /// <summary>
        /// Gets or sets the id of the test step, not null.
        /// </summary>
        public string StepId { get; set; }
        
        /// <summary>
        /// Gets or sets the stream name, not null.
        /// </summary>
        public string StreamName { get; set; }

        /// <summary>
        /// Gets or sets the section name, not null.
        /// </summary>
        public string SectionName { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("stepId", StepId);
            ValidationUtils.ValidateNotNull("streamName", StreamName);
            ValidationUtils.ValidateNotNull("sectionName", SectionName);
        }

        /// <inheritdoc />
        public override Message Normalize()
        {
            string normalizedStepId = ModelNormalizationUtils.NormalizeTestComponentId(StepId);
            string normalizedStreamName = MarkupNormalizationUtils.NormalizeStreamName(StreamName);
            string normalizedSectionName = MarkupNormalizationUtils.NormalizeSectionName(SectionName);

            if (ReferenceEquals(StepId, normalizedStepId)
                && ReferenceEquals(StreamName, normalizedStreamName)
                && ReferenceEquals(SectionName, normalizedSectionName))
                return this;

            return new TestStepLogStreamBeginSectionBlockMessage()
            {
                StepId = normalizedStepId,
                SectionName = normalizedSectionName,
                StreamName = normalizedStreamName
            };
        }
    }
}
