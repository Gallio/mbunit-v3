// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns.StandardExceptionConstructor
{
    /// <summary>
    /// Represents specifications for the invocation 
    /// of an exception type constructor.
    /// </summary>
    internal sealed class ExceptionConstructorSpec
    {
        private object[] parameters;

        /// <summary>
        /// Obtains an instance of the exception type based on
        /// the parameters specified in the specifications.
        /// </summary>
        /// <param name="ctor">Information about the constructor.</param>
        /// <returns></returns>
        public Exception GetInstance(ConstructorInfo ctor)
        {
            return (Exception)ctor.Invoke(parameters);
        }

        /// <summary>
        /// Gets the message parameter.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the inner exception parameter.
        /// </summary>
        public Exception InnerException
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs specifications for a default parameter-less constructor.
        /// </summary>
        public ExceptionConstructorSpec()
        {
        }

        /// <summary>
        /// Constructs specifications for a single parameter constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExceptionConstructorSpec(string message)
        {
            this.parameters = new object[] { message };
            this.Message = message;
        }

        /// <summary>
        /// Constructs specifications for a two parameters constructor.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExceptionConstructorSpec(string message, Exception innerException)
        {
            this.parameters = new object[] { message, innerException };
            this.Message = message;
            this.InnerException = innerException;
        }
    }
}
