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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Represents a template derived from an MbUnit type such as a fixture class.
    /// </summary>
    public class MbUnitTypeTemplate : MbUnitTemplate
    {
        private readonly MbUnitAssemblyTemplate assemblyTemplate;
        private readonly Type type;

        /// <summary>
        /// Initializes an MbUnit type template model object.
        /// </summary>
        /// <param name="assemblyTemplate">The containing assembly template</param>
        /// <param name="type">The type from which the template was derived</param>
        public MbUnitTypeTemplate(MbUnitAssemblyTemplate assemblyTemplate, Type type)
            : base(MakeTemplateName(type), CodeReference.CreateFromType(type))
        {
            this.assemblyTemplate = assemblyTemplate;
            this.type = type;

            Kind = ComponentKind.Fixture;

            string xmlDocumentation = Runtime.XmlDocumentationResolver.GetXmlDocumentation(type);
            if (xmlDocumentation != null)
                Metadata.Entries.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }

        /// <summary>
        /// Gets the containing assembly template.
        /// </summary>
        public MbUnitAssemblyTemplate AssemblyTemplate
        {
            get { return assemblyTemplate; }
        }

        /// <summary>
        /// Gets the type from which the template was derived.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the list of method templates that are children of this template.
        /// </summary>
        public IList<MbUnitMethodTemplate> MethodTemplates
        {
            get { return ModelUtils.FilterChildrenByType<ITemplate, MbUnitMethodTemplate>(this); }
        }

        /// <summary>
        /// Adds a method template as a child of this template.
        /// </summary>
        /// <param name="methodTemplate">The method template</param>
        public void AddMethodTemplate(MbUnitMethodTemplate methodTemplate)
        {
            AddChild(methodTemplate);
        }

        private static string MakeTemplateName(Type type)
        {
            if (type.IsNested)
                return MakeTemplateName(type.DeclaringType) + "." + type.Name;
            return type.Name;
        }
    }
}
