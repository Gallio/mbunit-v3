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
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;

namespace Gallio.Model.Messages.Exploration
{
    /// <summary>
    /// Notifies that an annotation has been discovered by the test exploration process.
    /// </summary>
    [Serializable]
    public class AnnotationDiscoveredMessage : Message
    {
        /// <summary>
        /// Gets or sets the annotation, not null.
        /// </summary>
        public AnnotationData Annotation { get; set; }
        
        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("annotation", Annotation);
        }

        /// <inheritdoc />
        public override Message Normalize()
        {
            AnnotationData normalizedAnnotation = Annotation.Normalize();

            if (ReferenceEquals(Annotation, normalizedAnnotation))
                return this;

            return new AnnotationDiscoveredMessage()
            {
                Annotation = normalizedAnnotation
            };
        }
    }
}
