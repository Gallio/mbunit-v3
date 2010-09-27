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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using System.Reflection;
using Gallio.Runtime.Formatting;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region HasAttribute
        
        /// <summary>
        /// Verifies that the targeted object is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute(Type, MemberInfo)"/> instead.
        /// </para>
        /// </remarks>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute HasAttribute(Type expectedAttributeType, MemberInfo target)
        {
            return HasAttribute(expectedAttributeType, target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute(Type, MemberInfo, string, object[])"/> instead.
        /// </para>
        /// </remarks>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute HasAttribute(Type expectedAttributeType, MemberInfo target, string messageFormat, params object[] messageArgs)
        {
            Attribute[] array = HasAttributesImpl(expectedAttributeType, 1, target, messageFormat, messageArgs);
            return array[0];
        }

        /// <summary>
        /// Verifies that the targeted type is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute{TAttribute}(MemberInfo)"/> instead.
        /// </para>
        /// </remarks>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute HasAttribute<TAttribute>(MemberInfo target)
            where TAttribute : Attribute
        {
            return (TAttribute)HasAttribute(typeof(TAttribute), target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted type is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute{TAttribute}(MemberInfo, string, object[])"/> instead.
        /// </para>
        /// </remarks>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute HasAttribute<TAttribute>(MemberInfo target, string messageFormat, params object[] messageArgs)
            where TAttribute : Attribute
        {
            return (TAttribute)HasAttribute(typeof(TAttribute), target, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute(Type, MemberInfo)"/> instead.
        /// </para>
        /// </remarks>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute HasAttribute(Type expectedAttributeType, Mirror.MemberSet target)
        {
            return HasAttribute(expectedAttributeType, target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute(Type, MemberInfo, string, object[])"/> instead.
        /// </para>
        /// </remarks>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute HasAttribute(Type expectedAttributeType, Mirror.MemberSet target, string messageFormat, params object[] messageArgs)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            
            return HasAttribute(expectedAttributeType, target.MemberInfo, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the targeted type is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute{TAttribute}(MemberInfo)"/> instead.
        /// </para>
        /// </remarks>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute HasAttribute<TAttribute>(Mirror.MemberSet target)
            where TAttribute : Attribute
        {
            return (TAttribute)HasAttribute(typeof(TAttribute), target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted type is decorated once with the specified <see cref="Attribute"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion returns the instance of the actual attribute found.
        /// </para>
        /// <para>
        /// The assertion fails if the target object is decorated with multiple instances of the searched attribute. If several
        /// instances are expected, use <see cref="Assert.HasAttribute{TAttribute}(MemberInfo, string, object[])"/> instead.
        /// </para>
        /// </remarks>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>The instance of the actual attribute found.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute HasAttribute<TAttribute>(Mirror.MemberSet target, string messageFormat, params object[] messageArgs)
            where TAttribute : Attribute
        {
            return (TAttribute)HasAttribute(typeof(TAttribute), target, messageFormat, messageArgs);
        }

        #endregion

        #region HasAttributes

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, MemberInfo target)
        {
            return HasAttributes(expectedAttributeType, target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, MemberInfo target, string messageFormat, params object[] messageArgs)
        {
            return HasAttributesImpl(expectedAttributeType, 0, target, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(MemberInfo target)
            where TAttribute : Attribute
        {

            return HasAttributes<TAttribute>(target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(MemberInfo target, string messageFormat, params object[] messageArgs)
            where TAttribute : Attribute
        {
            return ToAttributeArray<TAttribute>(HasAttributes(typeof(TAttribute), target, messageFormat, messageArgs));
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, Mirror.MemberSet target)
        {
            return HasAttributes(expectedAttributeType, target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, Mirror.MemberSet target, string messageFormat, params object[] messageArgs)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            return HasAttributes(expectedAttributeType, target.MemberInfo, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(Mirror.MemberSet target)
            where TAttribute : Attribute
        {
            return HasAttributes<TAttribute>(target, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with one or several instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(Mirror.MemberSet target, string messageFormat, params object[] messageArgs)
            where TAttribute : Attribute
        {
            return ToAttributeArray<TAttribute>(HasAttributes(typeof(TAttribute), target, messageFormat, messageArgs));
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, MemberInfo target, int expectedCount)
        {
            return HasAttributes(expectedAttributeType, target, expectedCount, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, MemberInfo target, int expectedCount, string messageFormat, params object[] messageArgs)
        {
            if (expectedCount <= 0)
                throw new ArgumentOutOfRangeException("expectedCount", "Must be greater than zero.");

            return HasAttributesImpl(expectedAttributeType, expectedCount, target, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(MemberInfo target, int expectedCount)
            where TAttribute : Attribute
        {
            return HasAttributes<TAttribute>(target, expectedCount, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(MemberInfo target, int expectedCount, string messageFormat, params object[] messageArgs)
            where TAttribute : Attribute
        {
            return ToAttributeArray<TAttribute>(HasAttributes(typeof(TAttribute), target, expectedCount, messageFormat, messageArgs));
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, Mirror.MemberSet target, int expectedCount)
        {
            return HasAttributes(expectedAttributeType, target, expectedCount, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <param name="expectedAttributeType">The type of the searched <see cref="Attribute"/>.</param>
        /// <param name="target">The targeted object to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedAttributeType"/> or <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static Attribute[] HasAttributes(Type expectedAttributeType, Mirror.MemberSet target, int expectedCount, string messageFormat, params object[] messageArgs)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            return HasAttributes(expectedAttributeType, target.MemberInfo, expectedCount, messageFormat, messageArgs);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(Mirror.MemberSet target, int expectedCount)
            where TAttribute : Attribute
        {
            return HasAttributes<TAttribute>(target, expectedCount, null, null);
        }

        /// <summary>
        /// Verifies that the targeted object is decorated with the exact number of instances of the specified <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the searched <see cref="Attribute"/>.</typeparam>
        /// <param name="target">The target type to evaluate.</param>
        /// <param name="expectedCount">The expected number of attribute instances to be found.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <returns>An array of attribute instances.</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="target"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="expectedCount"/> is less than or equal to zero.</exception>
        public static TAttribute[] HasAttributes<TAttribute>(Mirror.MemberSet target, int expectedCount, string messageFormat, params object[] messageArgs)
            where TAttribute : Attribute
        {
            return ToAttributeArray<TAttribute>(HasAttributes(typeof(TAttribute), target, expectedCount, messageFormat, messageArgs));
        }

        #endregion

        private static TAttribute[] ToAttributeArray<TAttribute>(Attribute[] attributes) 
            where TAttribute : Attribute
        {
            return GenericCollectionUtils.ToArray(GenericCollectionUtils.Select(attributes, x => (TAttribute)x));
        }

        private static Attribute[] HasAttributesImpl(Type attributeType, int expectedCount, MemberInfo target, string messageFormat, params object[] messageArgs)
        {
            Attribute[] result = GenericCollectionUtils.ToArray(GetAttributes(attributeType, target));

            AssertionHelper.Verify(() =>
            {
                string message = null;
                
                if (result.Length == 0)
                    message = "Expected the searched attribute to decorate the target object; but none was found.";
                else if (expectedCount > 0 && result.Length != expectedCount)
                    message = String.Format("Expected to find {0} attribute instance{1} but found {2}.", 
                        expectedCount, expectedCount > 1 ? "s" : String.Empty, result.Length);

                if (message == null)
                    return null;

                return new AssertionFailureBuilder(message)
                    .AddRawLabeledValue("Target Object", target)
                    .AddRawLabeledValue("Attribute Type", attributeType)
                    .SetMessage(messageFormat, messageArgs)
                    .ToAssertionFailure();
            });

            return result;
        }

        private static IEnumerable<Attribute> GetAttributes(Type attributeType, MemberInfo target)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");
            if (target == null)
                throw new ArgumentNullException("target");
            if (!typeof(Attribute).IsAssignableFrom(attributeType))
                throw new ArgumentException(String.Format("The expected attribute type '{0}' must derive from System.Attribute.", attributeType));

            foreach (Attribute attribute in Attribute.GetCustomAttributes(target, true))
            {
                if (attributeType.IsAssignableFrom(attribute.GetType()))
                {
                    yield return attribute;
                }
            }
        }
    }
}
