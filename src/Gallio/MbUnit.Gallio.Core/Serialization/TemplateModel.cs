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

using System;
using System.Collections.Generic;

namespace MbUnit.Core.Serialization
{
    /// <summary>
    /// The template model captures the root of the template tree along with an index by id.
    /// </summary>
    /// <remarks>
    /// This class is safe for used by multiple threads.
    /// </remarks>
    [Serializable]
    public class TemplateModel
    {
        [NonSerialized]
        private Dictionary<string, TemplateInfo> templates;

        private TemplateInfo rootTemplate;

        /// <summary>
        /// Creates a template model.
        /// </summary>
        /// <param name="rootTemplate">The root template</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootTemplate"/> is null</exception>
        public TemplateModel(TemplateInfo rootTemplate)
        {
            if (rootTemplate == null)
                throw new ArgumentNullException("rootTemplate");

            this.rootTemplate = rootTemplate;
        }


        /// <summary>
        /// Gets the root template in the model.
        /// </summary>
        public TemplateInfo RootTemplate
        {
            get { return rootTemplate; }
        }

        /// <summary>
        /// Gets a dictionary of templates indexed by id.
        /// </summary>
        public IDictionary<string, TemplateInfo> Templates
        {
            get
            {
                lock (this)
                {
                    if (templates == null)
                    {
                        templates = new Dictionary<string, TemplateInfo>();
                        PopulateTemplates(rootTemplate);
                    }

                    return templates;
                }
            }
        }

        private void PopulateTemplates(TemplateInfo template)
        {
            templates[template.Id] = template;

            foreach (TemplateInfo child in template.Children)
                PopulateTemplates(child);
        }
    }
}