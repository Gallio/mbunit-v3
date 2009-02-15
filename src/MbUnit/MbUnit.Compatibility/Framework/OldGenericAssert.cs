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
using MbUnit.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// Assertion class
    /// </summary>
    [Obsolete("Use Assert instead.")]
    public sealed class OldGenericAssert
     {
        #region IsEmpty
         /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsEmpty<T>(ICollection<T> collection, string message, params object[] args)
        {
            if (collection.Count != 0)
            {
                if (args != null)
                    FailIsNotEmpty(collection, message, args);
                else
                    FailIsNotEmpty(collection, message);
            }
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        public static void IsEmpty<T>(ICollection<T> collection, string message)
        {
            IsEmpty(collection, message, null);
        }

        /// <summary>
        /// Assert that an array,list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsEmpty<T>(ICollection<T> collection)
        {
            IsEmpty(collection, string.Empty, null);
        }
        #endregion

        #region IsNotEmpty

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        /// <param name="args">Arguments to be used in formatting the message</param>
        public static void IsNotEmpty<T>(ICollection<T> collection, string message, params object[] args)
        {
            if (collection.Count == 0)
            {
                if (args != null)
                    FailIsEmpty(collection, message, args);
                else
                    FailIsEmpty(collection, message);
            }
        }

        /// <summary>
        /// Assert that an array, list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        /// <param name="message">The message to be displayed on failure</param>
        public static void IsNotEmpty<T>(ICollection<T> collection, string message)
        {
            IsNotEmpty(collection, message, null);
        }

        /// <summary>
        /// Assert that an array,list or other collection is empty
        /// </summary>
        /// <param name="collection">An array, list or other collection implementing ICollection</param>
        public static void IsNotEmpty<T>(ICollection<T> collection)
        {
            IsNotEmpty(collection, string.Empty, null);
        }
        #endregion

        #region Private Method

        static private void FailIsEmpty<T>(ICollection<T> expected, string format, params object[] args)
         {
             string formatted = string.Empty;
             if (format != null)
                 formatted = format + " ";
             OldAssert.Fail(formatted + "expected not to be empty", args);
         }

        static private void FailIsNotEmpty<T>(ICollection<T> expected, string format, params object[] args)
        {
            string formatted = string.Empty;
            if (format != null)
                formatted = format + " ";
            OldAssert.Fail(formatted + "expected to be empty", args);
        }

        #endregion

    }
}