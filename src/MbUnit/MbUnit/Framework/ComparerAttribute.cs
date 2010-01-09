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
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a custom type comparer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That attribute must be used on a static method which takes 2 parameters of the same type, and return a <see cref="Int32"/> value.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class MyComparers
    /// {
    ///     [Comparer]
    ///     public static int Compare(Foo x, Foo y)
    ///     {
    ///         return /* Insert comparison logic here... */
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="EqualityComparerAttribute"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ComparerAttribute : AbstractComparerAttribute
    {
        /// <inheritdoc />
        protected override void Verify(IMethodInfo methodInfo)
        {
            if (methodInfo.ReturnType.Resolve(true) != typeof(int))
                ThrowUsageErrorException(String.Format("Expected the custom conversion method '{0}' to return '{1}', but found '{2}'.", methodInfo.Name, typeof(int), methodInfo.ReturnType));

            if (methodInfo.Parameters.Count != 2)
                ThrowUsageErrorException(String.Format("Expected the custom conversion method '{0}' to take 2 parameters, but found {1}.", methodInfo.Name, methodInfo.Parameters.Count));

            if (!methodInfo.Parameters[0].ValueType.Equals(methodInfo.Parameters[1].ValueType))
                ThrowUsageErrorException(String.Format("Expected the custom conversion method '{0}' to take 2 parameters of the same type, but found '{1}' and '{2}'.", methodInfo.Name, methodInfo.Parameters[0], methodInfo.Parameters[1]));
        }

        /// <inheritdoc />
        protected override void Register(Type type, Func<object, object, object> operation)
        {
            CustomComparers.Register(type, (x, y) => (int)operation(x, y));
        }

        /// <inheritdoc />
        protected override void Unregister(Type type)
        {
            CustomComparers.Unregister(type);
        }
    }
}
