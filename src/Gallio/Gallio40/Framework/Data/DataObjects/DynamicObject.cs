// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Original code was copied from MSDN, with complete overhaul performed by Carey's team.
    /// Dictionary replaced with a List of KeyValuePair to retain the original ordering.
    /// </summary>
    public class  DynamicObject : System.Dynamic.DynamicObject
    {
        /// <summary>
        /// Internal storage of dynamically accessible data
        /// </summary>
         protected List<KeyValuePair<string, object>> storage = new List<KeyValuePair<string, object>>();

        /// <summary>
        /// This property returns the number of elements in the inner dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                return storage.Count;
            }
        }

        /// <summary>
        /// Dynamic Get method called by the Dot Net Framework when an attempt is made to access 
        /// a property not defined in this class at compile time.
        /// If a dynamic property doesn't exists on the object, the result will be NULL.
        /// </summary>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMemberWorker(binder.Name, out result);
        }

        /// <summary>
        /// Dynamic Get method called by the Dot Net Framework when an attempt is made to access 
        /// a property not defined in this class at compile time.
        /// If a dynamic property doesn't exists on the object, the result will be NULL.
        /// </summary>
        public bool TryGetMember(string name, out object result)
        {
            return TryGetMemberWorker(name, out result);
        }

        /// <summary>
        /// Get method enables late-binding type of behavior when the Member Name is known at 
        /// compile time.
        /// </summary>
        public object GetMember(string name)
        {
            object result;
            bool retval = TryGetMemberWorker(name, out result);
            return result;
        }

        /// <summary>
        /// Retrieve DynamicObject member by name.
        /// </summary>
        /// <returns></returns>
        private bool TryGetMemberWorker(string name, out object result)
        {
            // Attempt to locate the member identified in the DLR binder name
            if (storage.Exists(x => x.Key == name))
            {
                result = storage.First<KeyValuePair<string, object>>(x => x.Key == name).Value;
            }
            else
            {
                // Can't find property - therefore return null
                result = null;
            }

            return true;
        }

        /// <summary>
        /// Returns list of the Member Names
        /// </summary>
        public IEnumerable<string> GetMemberNames()
        {
            foreach (KeyValuePair<string, object> member in this.storage)
                yield return member.Key;
        }

        /// <summary>
        /// Dynamic Set method called by the Dot Net Framework when an attempt is made to set
        /// a property not defined in this class.
        /// </summary>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            TrySetMemberWorker(binder.Name, value);
            return true;
        }

        /// <summary>
        /// Method allows properties to set by name, which is useful for dynamic situations.
        /// </summary>
        public bool TrySetMember(string name, object value)
        {
            TrySetMemberWorker(name, value);
            return true;
        }

        /// <summary>
        /// Worker method does the actual setting of values
        /// </summary>
        private void TrySetMemberWorker(string name, object value)
        {
            // Replacing invalid characters
            name = XmlNodeNameToCSharpSafe(name);

            // Check to see if the value exists in the storage
            if (storage.Exists(x => x.Key == name))
                storage.Remove(storage.First<KeyValuePair<string, object>>(x => x.Key == name));
            storage.Add(new KeyValuePair<string, object>(name, value));
        }

        /// <summary>
        /// Translates Xml node names to CSharp-safe names, so that they can be accessed dynamically
        /// </summary>
        public static string XmlNodeNameToCSharpSafe(string name)
        {
            // Exception-worthy cases
            if (name == null)
                throw new NullReferenceException();
            if (name == "")
                throw new ArgumentException("name is an empty string");
            if (Char.IsNumber(name[0]))
                throw new ArgumentException("The first character is non-alphabetic");

            // Remove leading and trailing whitespaces
            name = name.Trim();

            // Replace dashes with underscores
            name = name.Replace("-", "_");

            // Replace non-alphanumeric characters with underscore
            name = Regex.Replace(name, @"[\W]", "_");

            return name;
        }

        /// <summary>
        /// Dumps every member
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // The attributes and the actual inner text are the members which are stored as 
            // string value in the Dictionary
            foreach (KeyValuePair<string, object> Member in this.storage)
            {
                sb.Append(Member.Key + ": " + Member.Value + "  ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Easy-to-read format dumps one KeyValuePair per line
        /// </summary>
        public virtual string ToStringWithNewLines()
        {
            StringBuilder sb = new StringBuilder();

            // The attributes and the actual inner text are the members which are stored as 
            // string value in the Dictionary
            foreach (KeyValuePair<string, object> Member in this.storage)
            {
                sb.Append(Member.Key + ": " + Member.Value + "  " + Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}

