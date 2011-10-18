// Copyright 2011 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
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
			// HACK: this code gets executed both in and out of proc :(
			// it won't be necessary once all communication is via messaging
			return Process.GetCurrentProcess().ProcessName == "Gallio.Host";
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