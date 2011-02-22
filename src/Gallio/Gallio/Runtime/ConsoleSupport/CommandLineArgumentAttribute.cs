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
using Gallio.Common.Collections;
using Gallio.Properties;

namespace Gallio.Runtime.ConsoleSupport
{
    /// <summary>
    /// Allows control of command line parsing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Attach this attribute to instance fields of types used
    /// as the destination of command line argument parsing.
    /// </para>
    /// <para>
    /// Command line parsing code from 
    /// <a href="http://www.gotdotnet.com/community/usersamples/details.aspx?sampleguid=62a0f27e-274e-4228-ba7f-bc0118ecc41e">
    /// Peter Halam</a>. 
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=false,Inherited=true)]
    public class CommandLineArgumentAttribute : Attribute
    {
        private readonly CommandLineArgumentFlags flags;
        private string shortName;
        private string longName;
        private string description = @"";
        private string valueLabel;
        private string[] synonyms = EmptyArray<string>.Instance;
        private Type resourceType;

        /// <summary>
        /// Allows control of command line parsing.
        /// </summary>
        /// <param name="flags">Specifies the error checking to be done on the argument.</param>
        public CommandLineArgumentAttribute(CommandLineArgumentFlags flags)
        {
            this.flags = flags;
        }
        
        /// <summary>
        /// The error checking to be done on the argument.
        /// </summary>
        public CommandLineArgumentFlags Flags
        {
            get { return flags; }
        }

        /// <summary>
        /// Returns true if the argument did not have an explicit short name specified.
        /// </summary>
        public bool IsDefaultShortName
        {
            get { return null == shortName; }
        }
        
        /// <summary>
        /// The short name of the argument.
        /// </summary>
        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }

        /// <summary>
        /// The localized short name of the argument.
        /// </summary>
        public string LocalizedShortName
        {
            get { return GetResourceLookup(shortName); }
        }

        /// <summary>
        /// Returns true if the argument did not have an explicit long name specified.
        /// </summary>
        public bool IsDefaultLongName
        {
            get { return null == longName; }
        }
        
        /// <summary>
        /// The long name of the argument.
        /// </summary>
        public string LongName
        {
            get { return longName; }
            set { longName = value; }
        }

        /// <summary>
        /// The localized long name of the argument.
        /// </summary>
        public string LocalizedLongName
        {
            get { return GetResourceLookup(longName); }
        }

        /// <summary>
        /// The description of the argument.
        /// </summary>
		public string Description
		{
			get { return description; }
            set { description = value; }
		}

        /// <summary>
        /// The localized description of the argument.
        /// </summary>
        public string LocalizedDescription
        {
            get { return GetResourceLookup(description); }
        }

        ///<summary>
        /// The description of the argument value.
        ///</summary>
        public string ValueLabel
        {
            get { return valueLabel; }
            set { valueLabel = value; }
        }

        ///<summary>
        /// The localized description of the argument value.
        ///</summary>
        public string LocalizedValueLabel
        {
            get { return GetResourceLookup(valueLabel); }
        }

        /// <summary>
        /// Gets or sets an array of additional synonyms that are silently accepted.
        /// </summary>
        public string[] Synonyms
        {
            get { return synonyms; }
            set { synonyms = value; } 
        }

        /// <summary>
        /// Gets an array of localized additional synonyms that are silently accepted.
        /// </summary>
        public string[] LocalizedSynonyms
        {
            get { return GenericCollectionUtils.ConvertAllToArray<string, string>(synonyms, GetResourceLookup); }
        }

        /// <summary>
        /// Gets or sets a resource to use for internationalization of strings
        /// </summary>
        public Type ResourceType
        {
            get { return resourceType; }
            set { resourceType = value; }
        }

        /// <summary>
        /// Gets or sets a resource to use for localization of strings
        /// </summary>
        private string GetResourceLookup(string resourceName)
        {
            if (resourceName == null)
            {
                return null;
            }

            if (resourceName.StartsWith("#"))
            {
                if (resourceType == null)
                    throw new InvalidOperationException(Resources.CommandLineArgumentAttribute_NoResourceException);

                PropertyInfo property = resourceType.GetProperty(resourceName.Substring(1), BindingFlags.NonPublic | BindingFlags.Static);
                if (property == null)
                {
                    throw new InvalidOperationException(Resources.CommandLineArgumentAttribute_ResourceNotFoundException);
                }
                if (property.PropertyType != typeof(string))
                {
                    throw new InvalidOperationException(Resources.CommandLineArgumentAttribute_ResourceNotAStringException);
                }
                return (string)property.GetValue(null, null);
            }
            else
            {
                return resourceName;
            }
        }
    }
}

