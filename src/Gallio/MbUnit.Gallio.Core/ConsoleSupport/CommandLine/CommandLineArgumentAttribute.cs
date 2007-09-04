// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Diagnostics;

namespace MbUnit.Core.ConsoleSupport.CommandLine
{
    /// <summary>
    /// Allows control of command line parsing.
    /// Attach this attribute to instance fields of types used
    /// as the destination of command line argument parsing.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Command line parsing code from Peter Halam, 
    /// http://www.gotdotnet.com/community/usersamples/details.aspx?sampleguid=62a0f27e-274e-4228-ba7f-bc0118ecc41e
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field,AllowMultiple=false,Inherited=true)]
    public class CommandLineArgumentAttribute : Attribute
    {
        /// <summary>
        /// Allows control of command line parsing.
        /// </summary>
        /// <param name="type"> Specifies the error checking to be done on the argument. </param>
        public CommandLineArgumentAttribute(CommandLineArgumentType type)
        {
            this.type = type;
        }
        
        /// <summary>
        /// The error checking to be done on the argument.
        /// </summary>
        public CommandLineArgumentType Type
        {
            get { return type; }
        }
        /// <summary>
        /// Returns true if the argument did not have an explicit short name specified.
        /// </summary>
        public bool DefaultShortName    { get { return null == shortName; } }
        
        /// <summary>
        /// The short name of the argument.
        /// </summary>
        public string ShortName
        {
            get { return shortName; }
            set { shortName = value; }
        }

        /// <summary>
        /// Returns true if the argument did not have an explicit long name specified.
        /// </summary>
        public bool DefaultLongName     { get { return null == longName; } }
        
        /// <summary>
        /// The long name of the argument.
        /// </summary>
        public string LongName
        {
            get { Debug.Assert(!DefaultLongName); return longName; }
            set { longName = value; }
        }

        /// <summary>
        /// The description of the argument.
        /// </summary>
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description=value;
			}
		}

        ///<summary>
        /// The description of the argument value type.
        ///</summary>
        public string ArgumentValueType
        {
            get { return _argValueType; }
            set { _argValueType = value; }
        }

        
        private string shortName;
        private string longName;
		private string description="";
        private string _argValueType;
        private readonly CommandLineArgumentType type;
    }
}

