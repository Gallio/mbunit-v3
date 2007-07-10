using System;

namespace MbUnit.Framework.Xml 
{
	/// <summary>
	/// Summary description for FlowControlException.
	/// </summary>
	[Serializable]
	public class FlowControlException : Exception
	{
		Difference cause;
		public FlowControlException(Difference cause)
		{
			this.cause = cause;
		}

		public Difference Cause
		{
			get
			{
				return this.cause;
			}
		}

		public override string Message
		{
			get
			{
				return String.Format("Diff failed: {0}",cause.ToString());
			}
		}

	}
}
