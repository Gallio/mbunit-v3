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
using Gallio.Common.Collections;
using Gallio.Common.Normalization;

namespace Gallio.Model
{
    /// <summary>
    /// Utilities for normalizing test model contents.
    /// </summary>
    public static class ModelNormalizationUtils
    {
        /// <summary>
        /// Normalizes a test, step or parameter id.
        /// </summary>
        /// <param name="id">The id, or null if none.</param>
        /// <returns>The normalized id, or null if none.  May be the same instance if <paramref name="id"/>
        /// was already normalized.</returns>
        public static string NormalizeTestComponentId(string id)
        {
            return NormalizationUtils.NormalizePrintableASCII(id);
        }

        /// <summary>
        /// Normalizes a test, step or parameter name.
        /// </summary>
        /// <param name="name">The name, or null if none.</param>
        /// <returns>The normalized name, or null if none.  May be the same instance if <paramref name="name"/>
        /// was already normalized.</returns>
        public static string NormalizeTestComponentName(string name)
        {
            return NormalizationUtils.NormalizeName(name);
        }

        /// <summary>
        /// Normalizes a lifecycle phase.
        /// </summary>
        /// <param name="lifecyclePhase">The lifecycle phase, or null if none.</param>
        /// <returns>The normalized lifecycle phase, or null if none.  May be the same instance if <paramref name="lifecyclePhase"/>
        /// was already normalized.</returns>
        public static string NormalizeLifecyclePhase(string lifecyclePhase)
        {
            return NormalizationUtils.NormalizeName(lifecyclePhase);
        }

        /// <summary>
        /// Normalizes a metadata key.
        /// </summary>
        /// <param name="metadataKey">The metadata key, or null if none.</param>
        /// <returns>The normalized metadata key, or null if none.  May be the same instance if <paramref name="metadataKey"/>
        /// was already normalized.</returns>
        public static string NormalizeMetadataKey(string metadataKey)
        {
            return NormalizationUtils.NormalizeName(metadataKey);
        }

        /// <summary>
        /// Normalizes a metadata value.
        /// </summary>
        /// <param name="metadataValue">The metadata value, or null if none.</param>
        /// <returns>The normalized metadata value, or null if none.  May be the same instance if <paramref name="metadataValue"/>
        /// was already normalized.</returns>
        public static string NormalizeMetadataValue(string metadataValue)
        {
            return NormalizationUtils.NormalizeXmlText(metadataValue);
        }

        /// <summary>
        /// Normalizes the message or details of an annotation.
        /// </summary>
        /// <param name="text">The text, or null if none.</param>
        /// <returns>The normalized text, or null if none.  May be the same instance if <paramref name="text"/>
        /// was already normalized.</returns>
        public static string NormalizeAnnotationText(string text)
        {
            return NormalizationUtils.NormalizeXmlText(text);
        }

        /// <summary>
        /// Normalizes a log message.
        /// </summary>
        /// <param name="text">The text, or null if none.</param>
        /// <returns>The normalized text, or null if none.  May be the same instance if <paramref name="text"/>
        /// was already normalized.</returns>
        public static string NormalizeLogMessage(string text)
        {
            return NormalizationUtils.NormalizeXmlText(text);
        }

        /// <summary>
        /// Normalizes a metadata collection.
        /// </summary>
        /// <param name="metadata">The metadata collection, or null if none.</param>
        /// <returns>The normalized metadata collection, or null if none.  May be the same instance if <paramref name="metadata"/>
        /// was already normalized.</returns>
        public static PropertyBag NormalizeMetadata(PropertyBag metadata)
        {
            return NormalizationUtils.NormalizeCollection<PropertyBag, KeyValuePair<string, IList<string>>>(
                metadata,
                () => new PropertyBag(),
                NormalizeMetadataKeyValueList,
                (x, y) => ReferenceEquals(x.Key, y.Key) && ReferenceEquals(x.Value, y.Value));
        }

        private static KeyValuePair<string, IList<string>> NormalizeMetadataKeyValueList(KeyValuePair<string, IList<string>> keyValueList)
        {
            return new KeyValuePair<string, IList<string>>(
                NormalizeMetadataKey(keyValueList.Key),
                NormalizeMetadataValueList(keyValueList.Value));
        }

        private static IList<string> NormalizeMetadataValueList(IList<string> metadataValues)
        {
            return NormalizationUtils.NormalizeCollection<IList<string>, string>(
                metadataValues,
                () => new List<string>(metadataValues.Count),
                NormalizeMetadataValue,
                ReferenceEquals);
        }
    }
}