// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares that a property, field or parameter is a test parameter and
    /// specifies its properties.  At most one attribute of this type may appear on
    /// any given test fixture property or field.  If the attribute is omitted from
    /// test method parameters and test fixture constructor parameters the parameter
    /// will be declared with default values (which are usually just fine).
    /// </summary>
    public class ParameterAttribute : TestParameterPatternAttribute
    {
        private string name;
        private int? index;

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// If set to null, the parameter is named the same as the property,
        /// field or parameter to which the attribute has been applied.
        /// </summary>
        /// <value>
        /// The default value is null.
        /// </value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the zero-based index of the parameter.  The index is used
        /// instead of the parameter name in unlabeled table-like data sources
        /// (such as row-tests and headerless CSV files) to select the column to
        /// which the parameter will be bound.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The index does not necessarily correspond to the sequence in which
        /// the parameter appears in its parameter set.
        /// </para>
        /// <para>
        /// Exotic data sources that do not bind by name or by index may use metadata
        /// associated with the parameter to specify how data binding will occur.
        /// For example, metadata containing an XPath expression could be used by XML data sources.
        /// </para>
        /// </remarks>
        /// <value>
        /// The default value is null which causes the parameter's index to be set to 0 for fields
        /// and properties or the parameter's actual positional index for the combined list
        /// of generic parameters and method parameters with the generic parameters counted first
        /// followed by the method parameters in left-to-right order.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 0</exception>
        public int? Index
        {
            get { return index; }
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentOutOfRangeException("value");

                index = value;
            }
        }

        /// <inheritdoc />
        protected override void InitializeTestParameter(IPatternTestParameterBuilder testParameterBuilder, ISlotInfo slot)
        {
            if (name != null)
                testParameterBuilder.TestParameter.Name = name;

            if (index.HasValue)
                testParameterBuilder.TestParameter.Index = index.Value;

            base.InitializeTestParameter(testParameterBuilder, slot);
        }
    }
}
