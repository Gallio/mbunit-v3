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
using Gallio.Framework.Pattern;
using Gallio.Reflection;

#if false // Not implemented yet

namespace MbUnit.Framework
{
    /// <summary>
    /// Links a "mixin" into a test fixture by way of a field or property.  The linked
    /// mixin participates in the lifecycle of the fixture and can provide tests, setup/teardown
    /// methods, and other contributions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The field or property type must refer to a "mixin" class with the <see cref="MixinAttribute" />.
    /// </para>
    /// <para>
    /// Refer to the documentation of <see cref="MixinAttribute"/> for example usages.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LinkMixinAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override bool IsPrimary
        {
            get { return true; }
        }

        /// <inheritdoc />
        public override void Consume(IPatternScope containingScope, ICodeElementInfo codeElement, bool skipChildren)
        {
            if (!IsReadableFieldOrProperty(codeElement))
                ThrowUsageErrorException("This attribute may only be applied to fields and properties with getters.");

            ISlotInfo slot = (ISlotInfo) codeElement;
            ITypeInfo mixinType = slot.ValueType;
            if (! mixinType.IsClass || !AttributeUtils.HasAttribute(mixinType, typeof(MixinAttribute), true))
                ThrowUsageErrorException(String.Format("The field or property value type must be a class with the [Mixin] attribute applied.  "
                    + "The type {0} does not appear to be a valid mixin class.", mixinType));

            // TODO: Detect cycles.
            // TODO: Modify how fixture types and instances are interpreted in the mixin.

            IPatternScope mixinScope = containingScope.CreateScope(codeElement,
                containingScope.TestBuilder, null, containingScope.TestDataContextBuilder.CreateChild(), false);
            mixinScope.Consume(mixinType, skipChildren, null);
        }

        private static bool IsReadableFieldOrProperty(ICodeElementInfo codeElement)
        {
            IFieldInfo field = codeElement as IFieldInfo;
            if (field != null)
                return true;

            IPropertyInfo property = codeElement as IPropertyInfo;
            if (property != null)
                return property.GetMethod != null;

            return false;
        }
    }
}

#endif