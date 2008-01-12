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

namespace Gallio.Hosting.ConsoleSupport
{
	/// <summary>
	/// Used to control parsing of command line arguments.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Command line parsing code from Peter Halam, 
	/// http://www.gotdotnet.com/community/usersamples/details.aspx?sampleguid=62a0f27e-274e-4228-ba7f-bc0118ecc41e
	/// </para>
	/// </remarks>
	[Flags]    
	public enum CommandLineArgumentFlags
	{
		/// <summary>
		/// Indicates that this field is required. An error will be displayed
		/// if it is not present when parsing arguments.
		/// </summary>
		Required    = 0x01,

		/// <summary>
		/// Only valid in conjunction with Multiple.
		/// Duplicate values will result in an error.
		/// </summary>
		Unique      = 0x02,

		/// <summary>
		/// Inidicates that the argument may be specified more than once.
		/// Only valid if the argument is a collection
		/// </summary>
		Multiple    = 0x04,

		/// <summary>
		/// The default type for non-collection arguments.
		/// The argument is not required, but an error will be reported if it is specified more than once.
		/// </summary>
		AtMostOnce  = 0x00,
        
		/// <summary>
		/// For non-collection arguments, when the argument is specified more than
		/// once no error is reported and the value of the argument is the last
		/// value which occurs in the argument list.
		/// </summary>
		LastOccurrenceWins = Multiple,

		/// <summary>
		/// The default type for collection arguments.
		/// The argument is permitted to occur multiple times, but duplicate 
		/// values will cause an error to be reported.
		/// </summary>
		MultipleUnique  = Multiple | Unique,
	}
}
