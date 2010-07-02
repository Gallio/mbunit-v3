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
using System.Collections.Generic;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// A rule-based converter uses a set of <see cref="IConversionRule" />s to
    /// perform conversions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It caches the best path it determines for each conversion
    /// so that it only needs to compute the conversion cost once.
    /// </para>
    /// </remarks>
    public class RuleBasedConverter : BaseConverter
    {
        private readonly List<IConversionRule> rules;
        private readonly Dictionary<ConversionKey, ConversionInfo> conversions;
        private readonly IExtensionPoints extensionPoints;

        /// <summary>
        /// Creates a rule-based converter.
        /// </summary>
        /// <param name="extensionPoints">The entry point for framework extensions.</param>
        /// <param name="rules">The rules to use.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rules"/> is null.</exception>
        public RuleBasedConverter(IExtensionPoints extensionPoints, IConversionRule[] rules)
        {
            if (extensionPoints == null)
                throw new ArgumentNullException("extensionPoints");
            if (rules == null)
                throw new ArgumentNullException("rules");

            this.rules = new List<IConversionRule>(rules);
            this.conversions = new Dictionary<ConversionKey, ConversionInfo>();
            this.extensionPoints = extensionPoints;
        }

        /// <inheritdoc />
        protected override ConversionCost GetConversionCostImpl(Type sourceType, Type targetType)
        {
            sourceType = GetUnderlyingTypeOfNullableIfPresent(sourceType);
            targetType = GetUnderlyingTypeOfNullableIfPresent(targetType);

            if (sourceType.Equals(targetType))
                return ConversionCost.Zero;

            ConversionInfo conversion = GetConversion(sourceType, targetType);
            return conversion.Cost;
        }

        /// <inheritdoc />
        protected override object ConvertImpl(object sourceValue, Type targetType)
        {
            if (sourceValue == null)
                return ConvertNull(targetType);

            bool nullable = ReflectionUtils.IsNullable(targetType);
            targetType = GetUnderlyingTypeOfNullableIfPresent(targetType);
            if (targetType.IsInstanceOfType(sourceValue))
                return sourceValue;

            Type sourceType = sourceValue.GetType();
            ConversionInfo conversion = GetConversion(sourceType, targetType);
            if (conversion.Cost.IsInvalid)
                throw new InvalidOperationException(String.Format("There is no registered conversion rule to convert a value of type '{0}' to type '{1}'.", sourceType, targetType));

            return conversion.Rule.Convert(sourceValue, targetType, this, nullable);
        }

        private static object ConvertNull(Type targetType)
        {
            if (!ReflectionUtils.IsNullable(targetType))
                throw new InvalidOperationException(String.Format("Cannot convert a null value to the non-nullable value type '{0}'.", targetType));

            return null;
        }

        private ConversionInfo GetConversion(Type sourceType, Type targetType)
        {
            lock (conversions)
            {
                var key = new ConversionKey(sourceType, targetType);
                ConversionInfo conversion;

                if (!conversions.TryGetValue(key, out conversion))
                {
                    // Try to get a custom user converter.
                    Conversion operation = extensionPoints.CustomConverters.Find(key);

                    if (operation != null)
                    {
                        var rule = new CustomConversionRule(operation);
                        conversion = new ConversionInfo(ConversionCost.Best, rule);
                    }
                    else
                    {
                        // Note: We add a null info record while populating the cache so as to handle
                        // potentially recursive lookups by preventing them from being satisfied.
                        conversions.Add(key, ConversionInfo.Null);
                        conversion = GetConversionWithoutCache(sourceType, targetType);
                    }
                
                    conversions[key] = conversion;
                }

                return conversion;
            }
        }

        private ConversionInfo GetConversionWithoutCache(Type sourceType, Type targetType)
        {
            ConversionInfo best = ConversionInfo.Null;

            foreach (IConversionRule rule in rules)
            {
                ConversionCost cost = rule.GetConversionCost(sourceType, targetType, this);
                if (cost.CompareTo(best.Cost) < 0 && ! cost.IsInvalid)
                    best = new ConversionInfo(cost, rule);
            }

            return best;
        }

        private static Type GetUnderlyingTypeOfNullableIfPresent(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        private struct ConversionInfo
        {
            public readonly ConversionCost Cost;
            public readonly IConversionRule Rule;

            public static readonly ConversionInfo Null = new ConversionInfo(ConversionCost.Invalid, null);

            public ConversionInfo(ConversionCost cost, IConversionRule rule)
            {
                Cost = cost;
                Rule = rule;
            }
        }
    }
}
