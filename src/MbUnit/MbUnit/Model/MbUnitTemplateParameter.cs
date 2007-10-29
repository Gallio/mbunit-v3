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
using System.Reflection;
using Gallio.Hosting;
using Gallio.Model;

namespace MbUnit.Model
{
    /// <summary>
    /// Represents a template parameter.
    /// </summary>
    public class MbUnitTemplateParameter : BaseTemplateParameter
    {
        private readonly Slot slot;

        /// <summary>
        /// Initializes an MbUnit test parameter model object.
        /// </summary>
        /// <param name="slot">The slot</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="slot"/> is null</exception>
        public MbUnitTemplateParameter(Slot slot)
            : base(ValidateSlotArgument(slot).Name, slot.CodeReference, slot.ValueType)
        {
            this.slot = slot;

            Index = slot.Position;

            MemberInfo member = slot.Member;
            if (member != null)
            {
                string xmlDocumentation = Loader.XmlDocumentationResolver.GetXmlDocumentation(member);
                if (xmlDocumentation != null)
                    Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
            }
        }

        /// <summary>
        /// Gets the associated slot.
        /// </summary>
        public Slot Slot
        {
            get { return slot; }
        }

        private static Slot ValidateSlotArgument(Slot slot)
        {
            if (slot == null)
                throw new ArgumentNullException(@"slot");
            return slot;
        }

    }
}