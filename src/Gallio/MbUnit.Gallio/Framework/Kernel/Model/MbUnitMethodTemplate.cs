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

using System.Reflection;
using MbUnit.Hosting;
using MbUnit.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit method such as a test case.
    /// </summary>
    public class MbUnitMethodTemplate : MbUnitTemplate
    {
        private readonly MbUnitTypeTemplate typeTemplate;
        private readonly MethodInfo method;

        /// <summary>
        /// Initializes an MbUnit test method template model object.
        /// </summary>
        /// <param name="typeTemplate">The containing type template</param>
        /// <param name="method">The method from which the template was derived</param>
        public MbUnitMethodTemplate(MbUnitTypeTemplate typeTemplate, MethodInfo method)
            : base(method.Name, CodeReference.CreateFromMember(method))
        {
            this.typeTemplate = typeTemplate;
            this.method = method;

            Kind = ComponentKind.Test;

            string xmlDocumentation = Loader.XmlDocumentationResolver.GetXmlDocumentation(method);
            if (xmlDocumentation != null)
                Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }

        /// <summary>
        /// Gets the containing type template.
        /// </summary>
        public MbUnitTypeTemplate TypeTemplate
        {
            get { return typeTemplate; }
        }

        /// <summary>
        /// Gets the test method.
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
        }
    }
}