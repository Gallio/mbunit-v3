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
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using System.Collections.Generic;
using Gallio.Model.Environments;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;

namespace MbUnit.Framework
{
    /// <summary>
    /// An abstract base class for custom comparer attributes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The custom comparer attributes expose an extension point of <see cref="IComparisonSemantics"/>
    /// for defining custom object comparison and equality for types that do not implement built-in
    /// comparison mechanisms such as <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="ComparerAttribute"/>
    /// <seealso cref="EqualityComparerAttribute"/>
    [AttributeUsage(PatternAttributeTargets.ContributionMethod, AllowMultiple = false, Inherited = true)]
    public abstract class AbstractComparerAttribute : ExtensionPointPatternAttribute
    {
        private IExtensionPoints extensionPoints;

        /// <summary>
        /// Gets the entry point for the registration of 
        /// custom actions that extend the framework.
        /// </summary>
        protected IExtensionPoints ExtensionPoints
        {
            get
            {
                return extensionPoints;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractComparerAttribute()
        {
            extensionPoints = (IExtensionPoints)RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.ExtensionPoints");
        }

        /// <inheritdoc />
        protected override void DecorateContainingScope(IPatternScope containingScope, IMethodInfo methodInfo)
        {
            Type comparableType = methodInfo.Parameters[0].Resolve(true).ParameterType;
            CustomTestEnvironment.SetUpThreadChain.Before(() => Register(comparableType, (x, y) => methodInfo.Resolve(true).Invoke(this, new[] { x, y })));
            CustomTestEnvironment.TeardownThreadChain.After(() => Unregister(comparableType));
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
