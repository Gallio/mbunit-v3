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

namespace Gallio.Core.ConsoleSupport
{
	/// <summary>
	/// Indicates that this argument is the default argument.
	/// '/' or '-' prefix only the argument value is specified.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Command line parsing code from Peter Halam, 
	/// http://www.gotdotnet.com/community/usersamples/details.aspx?sampleguid=62a0f27e-274e-4228-ba7f-bc0118ecc41e
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field)]
	public class DefaultCommandLineArgumentAttribute : CommandLineArgumentAttribute
	{
		/// <summary>
		/// Indicates that this argument is the default argument.
		/// </summary>
		/// <param name="flags"> Specifies the error checking to be done on the argument. </param>
		public DefaultCommandLineArgumentAttribute(CommandLineArgumentFlags flags)
			: base (flags)
		{
		}
	}
}
