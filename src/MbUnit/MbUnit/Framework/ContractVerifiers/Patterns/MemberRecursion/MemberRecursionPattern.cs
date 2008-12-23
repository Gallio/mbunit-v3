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
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Framework.Assertions;
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns.MemberRecursion
{
    /// <summary>
    /// General purpose test pattern for contract verifiers.
    /// Traverses recursively all the type members with some specific
    /// binding flags and type, then performs some custom verification
    /// on them. Common system types are not explored.
    /// </summary>
    /// <typeparam name="TTarget">The target type to test.</typeparam>
    /// <typeparam name="TMemberInfo"></typeparam>
    internal class MemberRecursionPattern<TTarget, TMemberInfo> : ContractVerifierPattern
        where TMemberInfo : MemberInfo
    {
        private MemberRecursionPatternSettings<TMemberInfo> settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings.</param>
        internal MemberRecursionPattern(MemberRecursionPatternSettings<TMemberInfo> settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.settings = settings;
        }

        /// <inheritdoc />
        protected override string Name
        {
            get
            {
                return settings.Name;
            }
        }

        /// <inheritdoc />
        protected internal override void Run(IContractVerifierPatternInstanceState state)
        {
            var visitedTypes = new List<Type>
            {   
                typeof(Int16),
                typeof(Int32),
                typeof(Int64),
                typeof(IntPtr),
                typeof(UInt16),
                typeof(UInt32),
                typeof(UInt64),
                typeof(UIntPtr),
                typeof(Single),
                typeof(Double),
                typeof(Decimal),
                typeof(Byte),
                typeof(Char),
                typeof(String),
                typeof(DateTime),
                typeof(TimeSpan),
            };

            Assert.Multiple(() =>
            {
                CheckType(typeof(TTarget), visitedTypes);
            });
        }

        private void CheckType(Type type, ICollection<Type> visitedTypes)
        {
            if (!visitedTypes.Contains(type) && !type.IsEnum)
            {
                visitedTypes.Add(type);

                foreach (var memberInfo in type.GetMembers(settings.BindingFlags))
                {
                    var item = memberInfo as TMemberInfo;

                    if (item != null)
                    {
                        AssertionHelper.Verify(() =>
                        {
                            return settings.AssertionFunc(item);
                        });

                        if (memberInfo.MemberType == MemberTypes.Field)
                        {
                            CheckType(((FieldInfo)memberInfo).FieldType, visitedTypes);
                        }
                        else if (memberInfo.MemberType == MemberTypes.Property)
                        {
                            CheckType(((PropertyInfo)memberInfo).PropertyType, visitedTypes);
                        }
                    }
                }
            }
        }
    }
}


