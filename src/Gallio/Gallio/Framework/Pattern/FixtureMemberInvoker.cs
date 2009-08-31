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
using System.Text;
using Gallio.Common;
using System.Reflection;
using Gallio.Common.Reflection;
using Gallio.Runtime.Conversions;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Finds and invokes a member of a test fixture, a nested type of the test fixture,
    /// or an external type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The member is searched among the fields, the properties, and the methods. It might be
    /// a public or a non-public member. If the member belongs directly to the test fixture, it
    /// might be an instance member. Otherwise, it must be a static member.
    /// </para>
    /// </remarks>
    /// <typeparam name="TOutput">The type of the result returned by the member invoked.</typeparam>
    public class FixtureMemberInvoker<TOutput>
    {
        private readonly Type type;
        private readonly IPatternScope scope;
        private readonly string memberName;
        private readonly BindingFlags bindingFlags;

        /// <summary>
        /// Constructs a invoker for a member of a test fixture, a nested type of the test fixture,
        /// or an external type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The member is searched among the fields, the properties, and the methods of the test fixture
        /// or the specified <paramref name="type"/>.
        /// </para>
        /// </remarks>
        /// <param name="type">The owning type of the searched member, or null if it is assumed to be the fixture class.</param>
        /// <param name="scope">The scope of the test.</param>
        /// <param name="memberName">The name of the searched member.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="scope"/> or <paramref name="memberName"/> are null.</exception>
        public FixtureMemberInvoker(Type type, IPatternScope scope, string memberName)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");
            if (memberName == null)
                throw new ArgumentNullException("memberName");

            this.type = type;
            this.scope = scope;
            this.memberName = memberName;
            this.bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                BindingFlags.FlattenHierarchy | ((type != null) ? 0 : BindingFlags.Instance);
        }

        /// <summary>
        /// Invokes the argument-less member and returns the resulting value.
        /// </summary>
        /// <returns>The resulting value.</returns>
        public TOutput Invoke()
        {
            return InvokeImpl(FixtureMemberInvokerTargets.Default, null);
        }

        /// <summary>
        /// Invokes the member with the specified arguments and returns the resulting value.
        /// </summary>
        /// <param name="args">The arguments to pass to the member.</param>
        /// <returns>The resulting value.</returns>
        public TOutput Invoke(params object[] args)
        {
            return InvokeImpl(FixtureMemberInvokerTargets.Default, args);
        }

        /// <summary>
        /// Invokes the member with the specified arguments and returns the resulting value.
        /// </summary>
        /// <param name="targets">The arguments to pass to the member.</param>
        /// <param name="args">The arguments to pass to the member.</param>
        /// <returns>The resulting value.</returns>
        public TOutput Invoke(FixtureMemberInvokerTargets targets, params object[] args)
        {
            return InvokeImpl(targets, args);
        }

        private TOutput InvokeImpl(FixtureMemberInvokerTargets targets, object[] args)
        {
            var ownerInfo = GetOwnerInfo();
            var function = TryGetMemberAsMethod(targets, ownerInfo)
                ?? TryGetMemberAsProperty(targets, ownerInfo)
                ?? TryGetMemberAsField(targets, ownerInfo);

            if (function == null)
                throw new PatternUsageErrorException(String.Format("Could not find a method, property or field named '{0}'.", memberName));

            return function(args);
        }

        private ITypeInfo GetOwnerInfo()
        {
            if (type != null)
            {
                return Reflector.Wrap(type);
            }
            else
            {
                var ownerInfo = ReflectionUtils.GetType(scope.TestBuilder.CodeElement);

                if (ownerInfo == null)
                    throw new PatternUsageErrorException("Cannot infer the declaring type of the searched member.");
                
                return ownerInfo;
            }
        }

        private Func<object[], TOutput> TryGetMemberAsMethod(FixtureMemberInvokerTargets targets, ITypeInfo ownerInfo)
        {
            if ((targets & FixtureMemberInvokerTargets.Method) != 0)
            {
                IMethodInfo info = ownerInfo.GetMethod(memberName, bindingFlags);

                if (info != null)
                {
                    return args =>
                    {
                        object fixtureInstance = GetFixtureInstance(info.IsStatic);
                        MethodInfo method = (type == null) ? GetFixtureType().GetMethod(memberName, bindingFlags) : info.Resolve(true);

                        if (method == null)
                            throw new TestFailedException(String.Format("Could not find method '{0}'.", memberName));

                        object[] convertedArgs = ConvertArguments(method.GetParameters(), args);
                        return (TOutput)method.Invoke(fixtureInstance, convertedArgs);
                    };
                }
            }

            return null;
        }

        private object[] ConvertArguments(ParameterInfo[] parameters, object[] args)
        {
            if ((args == null ? 0 : args.Length) != parameters.Length)
                throw new PatternUsageErrorException(String.Format("Unexpected number of " +
                    "arguments specified to invoke the method '{0}'. Found {1} while {2} expected.", 
                    memberName, (args == null ? 0 : args.Length), parameters.Length));

            if (args == null)
                return null;

            var converter = Converter.Instance;
            var result = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    result[i] = Object.ReferenceEquals(args[i], null) 
                        ? args[i] : converter.Convert(args[i], parameters[i].ParameterType);
                }
                catch (InvalidCastException exception)
                {
                    throw new PatternUsageErrorException(String.Format(
                        "Expected the argument #{0} of type '{1}' passed to the method '{2}' to be convertible to '{3}'.", 
                        i, args[i].GetType(), memberName, parameters[i].ParameterType), exception);
                }
                catch (FormatException exception)
                {
                    throw new PatternUsageErrorException(String.Format(
                        "Expected the argument #{0} of type '{1}' passed to the method '{2}' to be convertible to '{3}'.",
                        i, args[i].GetType(), memberName, parameters[i].ParameterType), exception);
                }
            }

            return result;
        }

        private Func<object[], TOutput> TryGetMemberAsProperty(FixtureMemberInvokerTargets targets, ITypeInfo ownerInfo)
        {
            if ((targets & FixtureMemberInvokerTargets.Property) != 0)
            {
                IPropertyInfo info = ownerInfo.GetProperty(memberName, bindingFlags);

                if (info != null && info.GetMethod != null)
                {
                    return args =>
                    {
                        object fixtureInstance = GetFixtureInstance(info.GetMethod.IsStatic);
                        PropertyInfo property = (type == null) ? GetFixtureType().GetProperty(memberName, bindingFlags) : info.Resolve(true);

                        if (property == null)
                            throw new TestFailedException(String.Format("Could not find property '{0}'.", memberName));

                        return (TOutput)property.GetValue(fixtureInstance, null);
                    };
                }
            }

            return null;
        }

        private Func<object[], TOutput> TryGetMemberAsField(FixtureMemberInvokerTargets targets, ITypeInfo ownerInfo)
        {
            if ((targets & FixtureMemberInvokerTargets.Field) != 0)
            {
                IFieldInfo info = ownerInfo.GetField(memberName, bindingFlags);

                if (info != null)
                {
                    return args =>
                    {
                        object fixtureInstance = GetFixtureInstance(info.IsStatic);
                        FieldInfo field = (type == null) ? GetFixtureType().GetField(memberName, bindingFlags) : info.Resolve(true);

                        if (field == null)
                            throw new TestFailedException(String.Format("Could not find field '{0}''.", memberName));

                        return (TOutput)field.GetValue(fixtureInstance);
                    };
                }
            }

            return null;
        }

        private Type GetFixtureType()
        {
            return GetCurrentTestInstanceState().FixtureType;
        }

        private object GetFixtureInstance(bool isStatic)
        {
            if (isStatic)
                return null;

            object instance = GetCurrentTestInstanceState().FixtureInstance;

            if (instance == null)
                throw new InvalidOperationException(String.Format("Cannot invoke member '{0}' because it is non-static and there is no fixture instance available for this test.", memberName));

            return instance;
        }

        private PatternTestInstanceState GetCurrentTestInstanceState()
        {
            var state = PatternTestInstanceState.FromContext(TestContext.CurrentContext);

            if (state == null)
                throw new NotSupportedException("Could not find the current pattern test instance state. The attribute probably cannot be used in this context.");

            return state;
        }
    }
}
