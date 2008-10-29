// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Declares that a property, field, method parameter, constructor parameter,
    /// generic type parameter or generic method parameter represents a test parameter.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// This attribute is optional for a method parameter, constructor parameter,
    /// generic type parameter or generic method parameter.  For a property or field,
    /// this attribute is required unless the property or field has at least one associated data
    /// source, in which case the attribute can be omitted.
    /// </para>
    /// <para>
    /// In other words, this attribute only needs to be specified on a property
    /// or field that does not have an associated explicit data binding attribute.
    /// This is typically the case when the parameter obtains its values from a
    /// data source defined by the test fixture.
    /// </para>
    /// </remarks>
    /// <example>
    /// [Header("Parameter1", "Parameter2")]
    /// [Row(1, "a")]
    /// [Row(2, "b")]
    /// public class Fixture
    /// {
    ///     [TestParameter]
    ///     public int Parameter1;
    ///     
    ///     [TestParameter]
    ///     public string Parameter2 { get; set; }
    /// }
    /// </example>
    public class ParameterAttribute : TestParameterPatternAttribute
    {
    }
}
