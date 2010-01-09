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
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Describes traits of an <see cref="IReportFormatter"/> component.
    /// </summary>
    public class ReportFormatterTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates report formatter traits.
        /// </summary>
        /// <param name="name">The unique name of the report format.</param>
        /// <param name="description">The description of report format.</param>
        public ReportFormatterTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the unique name of the report format.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of the report format.
        /// </summary>
        public string Description
        {
            get { return description; }
        }
    }
}
