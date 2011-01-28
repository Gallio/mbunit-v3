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
using System.Text;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// A formatting rule for <see cref="System.Reflection.MemberInfo" /> and all the derived types but <see cref="System.Type"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Formats values like: "{Type}, {MemberType}: {Name}" with 
    /// <list type="bullet">{Type} is the declarative type formatted by <see cref="TypeFormattingRule"/>.</list>
    /// <list type="bullet">{MemberType} is type of member (See <see cref="System.Reflection.MemberTypes"/>).</list>
    /// <list type="bullet">{Name} is the name of the member.</list>
    /// <example>E.g. "The.Namespace.TheType, Property: MyProperty"</example>
    /// </para>
    /// </remarks>
    public sealed class MemberInfoFormattingRule : TypeFormattingRule
    {
        /// <inheritdoc />
        public override int? GetPriority(Type type)
        {
            bool isType = typeof(Type).IsAssignableFrom(type);
            bool isMemberInfo = typeof(MemberInfo).IsAssignableFrom(type);

            if (isMemberInfo && !isType)
                return FormattingRulePriority.Best;

            return null;
        }

        /// <inheritdoc />
        public override string Format(object obj, IFormatter formatter)
        {
            IMemberInfo value = Reflector.Wrap((MemberInfo)obj);
            return String.Format("{0}, {1}:{2}", base.Format(value.DeclaringType.Resolve(true), formatter), value.Kind, value.Name);
        }
    }
}