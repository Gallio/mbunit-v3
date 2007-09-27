using System;
using System.Collections.Generic;
using MbUnit.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// Assertion class
    /// </summary>
     public sealed class GenericAssert
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
                    GenericAssert.FailIsNotEmpty(collection, message, args);
                else
                    GenericAssert.FailIsNotEmpty(collection, message);
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
                    GenericAssert.FailIsEmpty(collection, message, args);
                else
                    GenericAssert.FailIsEmpty(collection, message);
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
             Assert.Fail(formatted + "expected not to be empty", args);
         }

        static private void FailIsNotEmpty<T>(ICollection<T> expected, string format, params object[] args)
        {
            string formatted = string.Empty;
            if (format != null)
                formatted = format + " ";
            Assert.Fail(formatted + "expected to be empty", args);
        }

        #endregion

    }
}