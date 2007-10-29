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
using System.Security.Permissions;

namespace Gallio.Utilities
{
	/// <summary>
	/// Long living object. (Extracted from NUnit source)
	/// </summary>
	/// <remarks>
	/// <para>
	/// All objects which are marshalled by reference
	/// and whose lifetime is manually controlled by
	/// the app, should derive from this class rather
	/// than MarshalByRefObject.
	/// </para>
	/// <para>
	/// This includes the remote test domain objects
	/// which are accessed by the client and those
	/// client objects which are called back by the
	/// remote test domain.
    /// </para>
	/// <para>
	/// Objects in this category that already inherit
	/// from some other class (e.g. from TextWriter)
	/// which in turn inherits from MarshalByRef object 
	/// should override InitializeLifetimeService to 
	/// return null to obtain the same effect.
	/// </para>
	/// <para>
	/// Original code from NUnit.
	/// Portions Copyright © 2003 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov, Charlie Poole
	/// </para>
	/// </remarks>
	public class LongLivingMarshalByRefObject : MarshalByRefObject
	{
        /// <inheritdoc />
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override Object InitializeLifetimeService()
		{
			return null;
		}
	}
}