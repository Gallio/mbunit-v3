// MbUnit Test Framework
// 
// Copyright (c) 2004 Jonathan de Halleux
//
// This software is provided 'as-is', without any express or implied warranty. 
// 
// In no event will the authors be held liable for any damages arising from 
// the use of this software.
// Permission is granted to anyone to use this software for any purpose, 
// including commercial applications, and to alter it and redistribute it 
// freely, subject to the following restrictions:
//
//		1. The origin of this software must not be misrepresented; 
//		you must not claim that you wrote the original software. 
//		If you use this software in a product, an acknowledgment in the product 
//		documentation would be appreciated but is not required.
//
//		2. Altered source versions must be plainly marked as such, and must 
//		not be misrepresented as being the original software.
//
//		3. This notice may not be removed or altered from any source 
//		distribution.
//		
//		MbUnit HomePage: http://www.mbunit.org
//		Author: Jonathan de Halleux

using System;

namespace MbUnit.Core.Utilities
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
		public override Object InitializeLifetimeService()
		{
			return null;
		}
	}
}