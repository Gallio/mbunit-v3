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