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
using System.Net;
using System.Net.Sockets;
using Gallio.Common.Messaging.MessageFormatters;

namespace Gallio.Common.Messaging.MessageSinks.Tcp
{
	/// <summary>
	/// Async TCP based message sink.
	/// </summary>
	public class AsyncTcpServerMessageSink : IMessageSink, IDisposable
	{
		private readonly IMessageFormatter messageFormatter;
		private readonly Socket socket;
		private const int defaultPort = 56351;

		///<summary>
		/// Async TCP based message sink.
		///</summary>
		///<param name="messageFormatter">The formatter to use.</param>
		///<param name="port">The port to use.</param>
		public AsyncTcpServerMessageSink(IMessageFormatter messageFormatter, int? port)
		{
			this.messageFormatter = messageFormatter;

			var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			var endPoint = new IPEndPoint(IPAddress.Loopback, port ?? defaultPort);
			serverSocket.Bind(endPoint);
			serverSocket.Listen((int)SocketOptionName.MaxConnections);
			socket = serverSocket.Accept();
		}

		/// <inheritdoc />
		public void Publish(Message message)
		{
			if (message == null)
				throw new ArgumentNullException("message");

			if (!socket.Connected)
			{
				// TODO: queue messages perhaps?
				return;
			}

			message.Validate();

			var messageData = messageFormatter.Serialise(message);
			var framedMessage = new FramedMessage(messageData);
			framedMessage.WriteTo(socket);
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		public void Dispose()
		{
			socket.Shutdown(SocketShutdown.Both);
		}	
	}
}
