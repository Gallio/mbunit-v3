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
using System.Text;
using Gallio.Collections;
using Gallio.Utilities;

namespace Gallio.Reflection.Impl
{
    /// <summary>
    /// A <see cref="StaticReflectionPolicy"/> event wrapper.
    /// </summary>
    public sealed class StaticEventWrapper : StaticReflectedMemberWrapper, IEventInfo
    {
        private readonly Memoizer<EventAttributes> eventAttributesMemoizer = new Memoizer<EventAttributes>();
        private readonly Memoizer<StaticMethodWrapper> raiseMethodMemoizer = new Memoizer<StaticMethodWrapper>();
        private readonly Memoizer<StaticMethodWrapper> addMethodMemoizer = new Memoizer<StaticMethodWrapper>();
        private readonly Memoizer<StaticMethodWrapper> removeMethodMemoizer = new Memoizer<StaticMethodWrapper>();
        private readonly Memoizer<ITypeInfo> eventHandlerTypeMemoizer = new Memoizer<ITypeInfo>();
        private readonly KeyedMemoizer<bool, EventInfo> resolveMemoizer = new KeyedMemoizer<bool, EventInfo>();

        /// <summary>
        /// Creates a wrapper.
        /// </summary>
        /// <param name="policy">The reflection policy</param>
        /// <param name="handle">The underlying reflection object</param>
        /// <param name="declaringType">The declaring type</param>
        /// <param name="reflectedType">The reflected type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="policy"/>, <paramref name="handle"/>,
        /// <paramref name="declaringType"/>, or <paramref name="reflectedType"/> is null</exception>
        public StaticEventWrapper(StaticReflectionPolicy policy, object handle, StaticDeclaredTypeWrapper declaringType,
            StaticDeclaredTypeWrapper reflectedType)
            : base(policy, handle, declaringType, reflectedType)
        {
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
            get { return addMethodMemoizer.Memoize(() => Policy.GetEventAddMethod(this)); }
        }
        IMethodInfo IEventInfo.AddMethod
        {
            get { return AddMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper RaiseMethod
        {
            get { return raiseMethodMemoizer.Memoize(() => Policy.GetEventRaiseMethod(this)); }
        }
        IMethodInfo IEventInfo.RaiseMethod
        {
            get { return RaiseMethod; }
        }

        /// <inheritdoc />
        public StaticMethodWrapper RemoveMethod
        {
            get { return removeMethodMemoizer.Memoize(() => Policy.GetEventRemoveMethod(this)); }
        }
        IMethodInfo IEventInfo.RemoveMethod
        {
            get { return RemoveMethod; }
        }

        /// <inheritdoc />
        public ITypeInfo EventHandlerType
        {
            get { return eventHandlerTypeMemoizer.Memoize(() =>
                Substitution.Apply(Policy.GetEventHandlerType(this))); }
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
                foreach (StaticEventWrapper other in Policy.GetTypeEvents(baseType, ReflectedType))
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
            return resolveMemoizer.Memoize(throwOnError,
                () => ReflectorResolveUtils.ResolveEvent(this, throwOnError));
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
        protected override IEnumerable<Attribute> GetPseudoCustomAttributes()
        {
            return EmptyArray<Attribute>.Instance;
        }
    }
}
