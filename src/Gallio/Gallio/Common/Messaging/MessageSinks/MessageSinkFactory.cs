using System;
using Gallio.Common.Messaging.MessageFormatters;
using Gallio.Common.Messaging.MessageSinks.Tcp;
using Gallio.Model.Isolation;

namespace Gallio.Common.Messaging.MessageSinks
{
	/// <summary>
	/// Message sink factory.
	/// </summary>
	public static class MessageSinkFactory
	{
		/// <summary>
		/// Gets the right message sink for the runner, based on the test isolation options.
		/// </summary>
		/// <param name="testIsolationOptions">The test isolation options.</param>
		/// <param name="messageSink">The message sink to wrap.</param>
		/// <returns>An appropriate message sink for the runner.</returns>
		public static IMessageSink GetRunnerMessageSink(TestIsolationOptions testIsolationOptions, IMessageSink messageSink)
		{
			return UseRemoting(testIsolationOptions) 
				? messageSink 
				: new AsyncTcpListenerMessageSink(messageSink, GetMessageFormatter(testIsolationOptions), GetPort(testIsolationOptions));
		}

		private static int? GetPort(TestIsolationOptions testIsolationOptions)
		{
			return null;
		}

		private static bool UseRemoting(TestIsolationOptions testIsolationOptions)
		{
			if (testIsolationOptions.Properties.ContainsKey("MessageSink"))
			{
				var messageSinkToUse = testIsolationOptions.Properties["MessageSink"];

				if (messageSinkToUse == "Remoting")
					return true;

				throw new ArgumentException(string.Format("{0} is not a valid value for MessageSink", messageSinkToUse));
			}
			return false;
		}

		private static IMessageFormatter GetMessageFormatter(TestIsolationOptions testIsolationOptions)
		{
			return new BinaryMessageFormatter();
		}

		/// <summary>
		/// Gets the right message sink for the host, based on the test isolation options.
		/// </summary>
		/// <param name="testIsolationOptions">The test isolation options.</param>
		/// <param name="messageSink">The message sink to wrap.</param>
		/// <returns>An appropriate message sink for the runner.</returns>
		public static IMessageSink GetHostMessageSink(TestIsolationOptions testIsolationOptions, IMessageSink messageSink)
		{
			return UseRemoting(testIsolationOptions) 
				? messageSink
				: new AsyncTcpServerMessageSink(GetMessageFormatter(testIsolationOptions), GetPort(testIsolationOptions));
		}
	}
}