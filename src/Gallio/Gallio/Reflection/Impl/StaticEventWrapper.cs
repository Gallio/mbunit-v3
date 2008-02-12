// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> event wrapper.
    /// </summary>
    public sealed class StaticEventWrapper : StaticMemberWrapper, IEventInfo
    {
        private readonly Memoizer<EventAttributes> eventAttributesMemoizer = new Memoizer<EventAttributes>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// or <paramref name="declaringType"/> is null</exception>
        public StaticEventWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType)
            : base(policy, handle, declaringType)
        {
            if (declaringType == null)
                throw new ArgumentNullException("declaringType");
        }

        /// <inheritdoc />
        public override CodeElementKind Kind
        {
            get { return CodeElementKind.Event; }
        }

        /// <inheritdoc />
        public EventAttributes EventAttributes
        {
            get
            {
                return eventAttributesMemoizer.Memoize(delegate
                {
                    return Policy.GetEventAttributes(this);
                });
            }
        }

        /// <inheritdoc />
        public StaticMethodWrapper AddMethod
        {
            get { return Policy.GetEventAddMethod(this); }
        }
        IMethodInfo IEventInfo.AddMethod
        {
            get { return AddMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper RaiseMethod
        {
            get { return Policy.GetEventRaiseMethod(this); }
        }
        IMethodInfo IEventInfo.RaiseMethod
        {
            get { return RaiseMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper RemoveMethod
        {
            get { return Policy.GetEventRemoveMethod(this); }
        }
        IMethodInfo IEventInfo.RemoveMethod
        {
            get { return RemoveMethod; }
        }

        /// <inheritdoc />
        public ITypeInfo EventHandlerType
        {
            get { return Substitution.Apply(Policy.GetEventHandlerType(this)); }
        }

        /// <summary>
        /// Gets the events that this one overrides or hides.
        /// Only includes overrides that appear on class types, not interfaces.
        /// </summary>
        /// <param name="overridesOnly">If true, only returns overrides</param>
        public IEnumerable<StaticEventWrapper> GetOverridenOrHiddenEvents(bool overridesOnly)
        {
            StaticMethodWrapper discriminator = GetDiscriminatorMethod(this);
            if (overridesOnly && !discriminator.IsOverride)
                yield break;

            string eventName = Name;
            foreach (StaticDeclaredTypeWrapper baseType in DeclaringType.GetAllBaseTypes())
            {
                foreach (StaticEventWrapper other in Policy.GetTypeEvents(baseType))
                {
                    if (eventName == other.Name)
                    {
                        if (overridesOnly)
                        {
                            StaticMethodWrapper otherDiscriminator = GetDiscriminatorMethod(other);
                            if (otherDiscriminator == null)
                                yield break;

                            if (discriminator.HidesMethod(otherDiscriminator))
                            {
                                yield return other;

                                if (!otherDiscriminator.IsOverride)
                                    yield break;
                                break;
                            }
                        }
                        else
                        {
                            yield return other;
                        }
                    }
                }
            }
        }

        private StaticMethodWrapper GetDiscriminatorMethod(StaticEventWrapper @event)
        {
            if (AddMethod != null)
                return @event.AddMethod;
            if (RemoveMethod != null)
                return @event.RemoveMethod;
            if (RaiseMethod != null)
                return @event.RaiseMethod;
            return null;
        }

        /// <inheritdoc />
        public EventInfo Resolve(bool throwOnError)
        {
            return ReflectorResolveUtils.ResolveEvent(this, throwOnError);
        }

        /// <inheritdoc />
        public bool Equals(IEventInfo other)
        {
            return Equals((object)other);
        }

        /// <inheritdoc />
        protected override MemberInfo ResolveMemberInfo(bool throwOnError)
        {
            return Resolve(throwOnError);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder sig = new StringBuilder();

            sig.Append(GetTypeNameForSignature(EventHandlerType));
            sig.Append(' ');
            sig.Append(Name);

            return sig.ToString();
        }

        /// <inheritdoc />
        protected override IEnumerable<ICodeElementInfo> GetInheritedElements()
        {
            foreach (StaticEventWrapper element in GetOverridenOrHiddenEvents(true))
                yield return element;
        }
    }
}
