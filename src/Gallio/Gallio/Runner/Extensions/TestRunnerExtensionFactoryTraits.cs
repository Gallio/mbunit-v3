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
using Gallio.Common;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Provides traits for <see cref="ITestRunnerExtensionFactory" />.
    /// </summary>
    public class TestRunnerExtensionFactoryTraits : Traits
    {
        /// <summary>
        /// Gets or sets a condition (using the syntax of <see cref="Condition" /> that
        /// governs whether the extension provided by the factory should be automatically
        /// installed on every run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The condition may contain references to properties of the form ${env:VariableName}
        /// which test whether a given environment variable has been defined.
        /// </para>
        /// </remarks>
        public Condition AutoActivationCondition { get; set; }
    }
}
