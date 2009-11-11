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
using System.Reflection;
using Gallio.Common;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Runtime.Conversions;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a container class for one or several custom type converters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That attribute must be used on a type implementing the interface <see cref="ICustomConverter{TSource, TTarget}"/>.
    /// </para>
    /// <para>
    /// It is possible for a container class to define more than one custom converter. 
    /// Implement <see cref="ICustomConverter{TSource, TTarget}"/> as many times as it is necessary for every type
    /// you need to convert.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// [CustomConverter]
    /// public class ColorToPenConverter : ICustomConverter<Color, Pen>
    /// {
    ///     public Pen Convert(Color source)
    ///     {
    ///         return new Pen(source);
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="ICustomConverter{TSource, TTarget}"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CustomConverterAttribute : ExtensionPointPatternAttribute
    {
        /// <inheritdoc />
        protected override void Extend(IPatternScope containingScope)
        {
            RegisterCustomConverters(containingScope, FindCustomConverters());
        }

        private IEnumerable<Type> FindCustomConverters()
        {
            Type[] interfaces = ContainerType.FindInterfaces(Module.FilterTypeName, typeof(ICustomConverter<,>).Name);

            if (interfaces.Length == 0)
                ThrowUsageErrorException(String.Format(
                    "The attribute '{0}' must be used on a class that implements '{1}', but '{2}' does not implement that interface.", GetType().Name, typeof(ICustomConverter<,>).Name, ContainerType));

            return interfaces;
        }

        private void RegisterCustomConverters(IPatternScope containingScope, IEnumerable<Type> interfaces)
        {
            foreach (Type @interface in interfaces)
            {
                Type sourceType = @interface.GetGenericArguments()[0];
                Type targetType = @interface.GetGenericArguments()[1];
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance;
                MethodInfo methodInfo = ContainerType.GetMethod("Convert", bindingFlags, null, new[] { sourceType }, null);
                containingScope.TestBuilder.TestActions.BeforeTestChain.After(state =>
                    CustomConverters.Register(sourceType, targetType, source => methodInfo.Invoke(ContainerInstance, new[] { source })));
                containingScope.TestBuilder.TestActions.AfterTestChain.Before(state =>
                    CustomConverters.Unregister(sourceType, targetType));
            }
        }
    }
}
