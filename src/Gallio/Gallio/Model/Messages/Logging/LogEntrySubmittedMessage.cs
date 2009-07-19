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
using Gallio.Common.Diagnostics;
using Gallio.Common.Validation;
using Gallio.Model.Schema;
using Gallio.Common.Messaging;
using Gallio.Runtime.Logging;

namespace Gallio.Model.Messages.Logging
{
    /// <summary>
    /// Notifies that a diagnostic log message has been submitted.
    /// </summary>
    [Serializable]
    public class LogEntrySubmittedMessage : Message
    {
        /// <summary>
        /// Gets or sets the log severity.
        /// </summary>
        public LogSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception, or null if none.
        /// </summary>
        public ExceptionData ExceptionData { get; set; }

        /// <inheritdoc />
        public override void Validate()
        {
            ValidationUtils.ValidateNotNull("message", Message);
        }

        /// <inheritdoc />
        public override Message Normalize()
        {
            string normalizedMessage = ModelNormalizationUtils.NormalizeLogMessage(Message);
            ExceptionData normalizedExceptionData = ExceptionData != null ? ExceptionData.Normalize() : null;

            if (ReferenceEquals(Message, normalizedMessage)
                && ReferenceEquals(ExceptionData, normalizedExceptionData))
                return this;

            return new LogEntrySubmittedMessage()
            {
                Severity = Severity,
                Message = normalizedMessage,
                ExceptionData = normalizedExceptionData
            };
        }
    }
}
