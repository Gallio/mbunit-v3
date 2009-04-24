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

namespace Gallio.Schema
{
    /// <summary>
    /// Validation utilities.
    /// </summary>
    public static class ValidationUtils
    {
        /// <summary>
        /// Validates that an item is not null.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="itemName">The name of the item</param>
        /// <param name="item">The item to validate</param>
        public static void ValidateNotNull<T>(string itemName, T item)
            where T : class
        {
            if (item == null)
                throw new ValidationException(string.Format("{0} should not be null.", itemName));
        }

        /// <summary>
        /// Validates that an item is a valid assembly name.
        /// </summary>
        /// <param name="itemName">The name of the item</param>
        /// <param name="value">The value to validate</param>
        public static void ValidateAssemblyName(string itemName, string value)
        {
            if (value != null)
            {
                try
                {
                    new AssemblyName(value);
                    return;
                }
                catch (ArgumentException)
                {
                }
            }

            throw new ValidationException(string.Format("{0} should be a valid assembly name.", itemName));
        }

        /// <summary>
        /// Validates that all items in a collection are not null.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="collectionName">The name of the collection</param>
        /// <param name="collection">The collection to validate</param>
        /// <exception cref="ValidationException">Thrown if the validation fails</exception>
        public static void ValidateElementsAreNotNull<T>(string collectionName, IEnumerable<T> collection)
            where T : class
        {
            foreach (T item in collection)
                if (item == null)
                    throw new ValidationException(string.Format("{0} should not contain any null elements.", collectionName));
        }

        /// <summary>
        /// Validates all non-null items of an enumeration of validatable elements.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="collection">The items to validate</param>
        /// <exception cref="ValidationException">Thrown if the validation fails</exception>
        public static void ValidateAll<T>(IEnumerable<T> collection)
            where T : IValidatable
        {
            foreach (IValidatable item in collection)
                if (item != null)
                    item.Validate();
        }
    }
}
