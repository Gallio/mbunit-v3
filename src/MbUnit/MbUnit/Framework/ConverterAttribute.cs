// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Runtime.Conversions;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a custom type converter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That attribute must be used on a static method taking one single parameter of the source type, and returning a value of the target type.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class MyConverters
    /// {
    ///     [Converter]
    ///     public static Pen KnownColorToPen(KnownColor knownColor)
    ///     {
    ///         return new Pen(Color.FromKnownColor(knownColor));
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public class ConverterAttribute : ExtensionPointPatternAttribute
    {
        /// <inheritdoc />
        protected override void Validate(IPatternScope containingScope, IMethodInfo method)
        {
            base.Validate(containingScope, method);

            if (method.ReturnType.Resolve(true) == typeof(void))
                ThrowUsageErrorException(String.Format("Expected the custom conversion method '{0}' to not return void.", method.Name));

            if (method.Parameters.Count != 1)
                ThrowUsageErrorException(String.Format("Expected the custom conversion method '{0}' to take only one parameter, but found {1}.", method.Name, method.Parameters.Count));
        }

        /// <inheritdoc />
        protected override void DecorateContainingScope(IPatternScope containingScope, IMethodInfo methodInfo)
        {
            Type sourceType = methodInfo.Parameters[0].Resolve(true).ParameterType;
            Type targetType = methodInfo.ReturnType.Resolve(true);
            MethodInfo method = methodInfo.Resolve(true);
            CustomTestEnvironment.SetUpThreadChain.Before(() => CustomConverters.Register(sourceType, targetType, x => method.Invoke(this, new[] { x })));
            CustomTestEnvironment.TeardownThreadChain.After(() => CustomConverters.Unregister(sourceType, targetType));
        }
    }
}
