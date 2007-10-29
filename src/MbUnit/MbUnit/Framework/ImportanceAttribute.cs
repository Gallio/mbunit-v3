// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using MbUnit.Attributes;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates a <see cref="Framework.Importance" /> with a test fixture, test method, test parameter
    /// or other test component.
    /// </summary>
    public class ImportanceAttribute : MetadataPatternAttribute
    {
        private Importance importance;

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
        public override void Apply(TemplateTreeBuilder builder, ITemplateComponent component)
        {
            component.Metadata.Add(MetadataKeys.Description, importance.ToString());
        }
    }

}
