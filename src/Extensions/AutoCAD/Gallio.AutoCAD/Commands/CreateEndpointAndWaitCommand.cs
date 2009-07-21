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
using System.Diagnostics;
using System.Globalization;
using Gallio.Runtime;

namespace Gallio.AutoCAD.Commands
{
    /// <summary>
    /// Maps to the <c>CREATEENDPOINTANDWAIT</c> command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>CREATEENDPOINTANDWAIT</c> command is provided by the plugin assembly.
    /// If it is not loaded into AutoCAD this command will not be available.
    /// </para>
    /// </remarks>
    internal class CreateEndpointAndWaitCommand : AcadCommand
    {
        /// <summary>
        /// Initializes a new <see cref="CreateEndpointAndWaitCommand"/> object.
        /// </summary>
        public CreateEndpointAndWaitCommand()
            : base("CREATEENDPOINTANDWAIT")
        {
        }

        /// <inheritdoc/>
        public override IEnumerable<string> Arguments
        {
            get
            {
                yield return IpcPortName;
            }
        }

        /// <summary>
        /// Gets or sets the IPC port name that the endpoint should create.
        /// </summary>
        public string IpcPortName
        {
            get;
            set;
        }
    }
}
