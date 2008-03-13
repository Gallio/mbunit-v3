// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Framework.Pattern;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// The abstract base type for MbUnit attributes that contribute values to data sources
    /// along with metadata such a description or expected exception type.
    /// </para>
    /// </summary>
    /// <seealso cref="DataPatternAttribute"/> for more information about data binding attributes in general.
    public abstract class DataAttribute : DataPatternAttribute
    {
        private string description;
        private Type expectedException;

        /// <summary>
        /// Gets or sets a description of the values provided by the data source.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets or sets the type of exception that should be thrown when the
        /// values provided by the data source are consumed by test.
        /// </summary>
        public Type ExpectedException
        {
            get { return expectedException; }
            set { expectedException = value; }
        }

        /// <summary>
        /// Gets the metadata for the data source.
        /// </summary>
        /// <returns>The metadata keys and values</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            if (description != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.Description, description);
            if (expectedException != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.ExpectedException, expectedException.FullName);
        }
    }
}
