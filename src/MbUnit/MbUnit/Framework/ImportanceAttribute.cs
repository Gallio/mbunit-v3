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
using Gallio.Model;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates a <see cref="Framework.Importance" /> with a test fixture, test method,
    /// test parameter or other test component.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestComponent, AllowMultiple = true, Inherited = true)]
    public class ImportanceAttribute : MetadataPatternAttribute
    {
        private readonly Importance importance;

        /// <summary>
        /// Associates a <see cref="Framework.Importance" />  with the test component annotated by this attribute.
        /// </summary>
        /// <param name="importance">The importance to associate</param>
        public ImportanceAttribute(Importance importance)
        {
            this.importance = importance;
        }

        /// <summary>
        /// Gets or sets the importance.
        /// </summary>
        public Importance Importance
        {
            get { return importance; }
        }

        /// <inheritdoc />
        protected override IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            yield return new KeyValuePair<string, string>(MetadataKeys.Importance, importance.ToString());
        }
    }
}
