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
using System.Collections.Specialized;
using System.Text;
using Gallio.Collections;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Provides options to report formatters.
    /// </summary>
    [Serializable]
    public sealed class ReportFormatterOptions
    {
        private readonly PropertySet properties;

        /// <summary>
        /// Creates a default set of options.
        /// </summary>
        public ReportFormatterOptions()
        {
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets a mutable collection of key/value pairs that specify configuration properties
        /// for the report formatter.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy</returns>
        public ReportFormatterOptions Copy()
        {
            ReportFormatterOptions copy = new ReportFormatterOptions();
            copy.properties.AddAll(properties);

            return copy;
        }
    }
}
