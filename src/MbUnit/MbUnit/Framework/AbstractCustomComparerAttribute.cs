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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using System.Collections.Generic;

namespace MbUnit.Framework
{
    /// <summary>
    /// An abstract base class for custom comparer attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The custom comparer attributes expose an extension point of <see cref="ComparisonSemantics"/>
    /// for defining custom object comparison and equality for types that do not implement built-in
    /// comparison mechanisms such as <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="CustomComparerAttribute"/>
    /// <seealso cref="CustomEqualityComparerAttribute"/>
    [SystemInternal]
    public abstract class AbstractCustomComparerAttribute : ExtensionPointPatternAttribute
    {
        private readonly string comparisonMethodName;
        private readonly Type genericComparerType;
        private readonly Type comparisonReturnType;

        /// <summary>
        /// Constructs an abstract attribute for custom comparison operations.
        /// </summary>
        /// <param name="genericComparerType">The type of the generic companion comparer interface.</param>
        /// <param name="comparisonMethodName">The name of the genertic comparison method.</param>
        /// <param name="comparisonReturnType">The type of the result returned by the comparison method.</param>
        protected AbstractCustomComparerAttribute(Type genericComparerType, string comparisonMethodName, Type comparisonReturnType)
        {
            this.genericComparerType = genericComparerType;
            this.comparisonMethodName = comparisonMethodName;
            this.comparisonReturnType = comparisonReturnType;
        }

        /// <inheritdoc />
        protected override void Extend(IPatternScope containingScope)
        {
            RegisterCustomComparers(containingScope, FindCustomComparers());
        }

        private IEnumerable<Type> FindCustomComparers()
        {
            Type[] interfaces = ContainerType.FindInterfaces(Module.FilterTypeName, genericComparerType.Name);

            if (interfaces.Length == 0)
                ThrowUsageErrorException(String.Format(
                    "The attribute '{0}' must be used on a class that implements '{1}', but '{2}' does not implement that interface.", GetType().Name, genericComparerType.Name, ContainerType));

            return interfaces;
        }

        private void RegisterCustomComparers(IPatternScope containingScope, IEnumerable<Type> interfaces)
        {
            foreach (Type @interface in interfaces)
            {
                Type comparableType = @interface.GetGenericArguments()[0];
                const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance;
                MethodInfo methodInfo = ContainerType.GetMethod(comparisonMethodName, bindingFlags, null, new[] { comparableType, comparableType }, null);
                containingScope.TestBuilder.TestActions.BeforeTestChain.After(state => 
                    Register(comparableType, (x, y) => methodInfo.Invoke(ContainerInstance, new[] {x, y})));
                containingScope.TestBuilder.TestActions.AfterTestChain.Before(state => 
                    Unregister(comparableType));
            }
        }

        /// <summary>
        /// Registers a custom comparison operation for the specified type.
        /// </summary>
        /// <param name="type">The type on which the comparer operates.</param>
        /// <param name="operation">The comparison operation.</param>
        protected abstract void Register(Type type, Func<object, object, object> operation);

        /// <summary>
        /// Registers the custom comparison operation for the specified type.
        /// </summary>
        /// <param name="type">The searched type.</param>
        protected abstract void Unregister(Type type);
    }
}
