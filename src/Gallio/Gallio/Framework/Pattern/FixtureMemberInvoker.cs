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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public class FixtureMemberInvoker<TOutput>
    {
        private readonly Type type;
        private readonly IPatternScope scope;
        private readonly string memberName;
        private readonly BindingFlags bindingFlags;
        private readonly Type[] argsTypes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public FixtureMemberInvoker(Type type, IPatternScope scope, string memberName)
            : this(type, scope, memberName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <param name="memberName"></param>
        /// <param name="argsTypes"></param>
        /// <returns></returns>
        public FixtureMemberInvoker(Type type, IPatternScope scope, string memberName, Type[] argsTypes)
        {
            if (scope == null)
                throw new ArgumentNullException("scope");

            if (memberName == null)
                throw new ArgumentNullException("memberName");

            this.type = type;
            this.scope = scope;
            this.memberName = memberName;
            this.argsTypes = argsTypes;
            this.bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                BindingFlags.FlattenHierarchy | ((type != null) ? 0 : BindingFlags.Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TOutput Invoke()
        {
            return InvokeImpl(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public TOutput Invoke(params object[] args)
        {
            return InvokeImpl(args);
        }

        private bool ParameterLess
        {
            get
            {
                return argsTypes == null || argsTypes.Length == 0;
            }
        }

        private TOutput InvokeImpl(object[] args)
        {
            var ownerInfo = GetOwnerInfo();
            var function = TryGetMemberAsMethod(ownerInfo) ?? TryGetMemberAsProperty(ownerInfo) ?? TryGetMemberAsField(ownerInfo);

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

        private Func<object[], TOutput> TryGetMemberAsMethod(ITypeInfo ownerInfo)
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

                    return (TOutput)method.Invoke(fixtureInstance, ParameterLess ? null : args);
                };
            }

            return null;
        }

        private Func<object[], TOutput> TryGetMemberAsProperty(ITypeInfo ownerInfo)
        {
            if (ParameterLess)
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

        private Func<object[], TOutput> TryGetMemberAsField(ITypeInfo ownerInfo)
        {
            if (ParameterLess)
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
