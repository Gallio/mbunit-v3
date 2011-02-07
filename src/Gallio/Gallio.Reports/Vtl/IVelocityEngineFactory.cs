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
using NVelocity;
using NVelocity.App;
using Gallio.Runner.Reports;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Factory which builds and initializes a Velocity templace engine.
    /// </summary>
    internal interface IVelocityEngineFactory
    {
        /// <summary>
        /// Creates a Velocity template engine.
        /// </summary>
        /// <returns>A newly created Velocity templace engine.</returns>
        VelocityEngine CreateVelocityEngine();

        /// <summary>
        /// Creates and initializes a contextual data container for the Velocity engine.
        /// </summary>
        /// <param name="reportWriter">The current report writer.</param>
        /// <param name="helper">A format helper class.</param>
        /// <returns>A fully initialized context.</returns>
        VelocityContext CreateVelocityContext(IReportWriter reportWriter, FormatHelper helper);
    }
}
