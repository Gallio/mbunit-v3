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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Gallio.Common.Messaging.MessageFormatters;

namespace Gallio.Common.Messaging.MessageSinks.Tcp
{
	/// <summary>
	/// Wraps a <see cref="IMessageSink"/> so that messages can be sent remotely.
	/// </summary>
	[Serializable]
	public class AsyncTcpListenerMessageSink : IMessageSink, IDisposable
	{
		private readonly IMessageSink messageSink;
		private readonly IMessageFormatter messageFormatter;
		private bool listening = true;
		private readonly Socket socket;
		private readonly IPEndPoint endPoint;
		private const int defaultPort = 56351;

		///<summary>
		///</summary>
		public AsyncTcpListenerMessageSink(IMessageSink messageSink, IMessageFormatter messageFormatter, int? port)
		{
			this.messageSink = messageSink;
			this.messageFormatter = messageFormatter;

			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			endPoint = new IPEndPoint(IPAddress.Loopback, port ?? defaultPort);
			socket.BeginConnect(endPoint, Connected, socket);
		}

		private void Connected(IAsyncResult asyncResult)
		{
			try
			{
				socket.EndConnect(asyncResult);
				Listen();
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode == 10061) // WSAECONNREFUSED: server not ready yet
				{
					Thread.Sleep(100);
					socket.BeginConnect(endPoint, Connected, socket);
				}
			}
		}

		private void Listen()
		{
			if (!listening)
				return;

			var state = new State();
			BeginReceive(state, 0, state.Buffer.Length);
		}

		private void BeginReceive(State state, int offset, int length)
		{
			socket.BeginReceive(state.Buffer, offset, length, SocketFlags.None, OnReceive, state);
		}

		private void OnReceive(IAsyncResult asyncResult)
		{
			var state = asyncResult.AsyncState as State;

			if (state == null)
				return;

			var count = socket.EndReceive(asyncResult);
			state.BytesReceived += count;

			ReadBuffer(state);
		}

		private void ReadBuffer(State state)
		{
			if (state.MessageSize == -1)
			{
				ReadMessageSize(state);
			} 
			else
			{
				ReadMessageBody(state);
			}
		}

		private void ReadMessageSize(State state)
		{
			if (state.BytesReceived == 0)
			{
				listening = false;
				return;
			}

			if (state.BytesReceived != 4)
				throw new ProtocolViolationException("The host sent an invalid message size.");
				
			state.MessageSize = BitConverter.ToInt32(state.Buffer, 0);

			ValidateMessageSize(state);

			state.Buffer = new byte[state.MessageSize];
			state.BytesReceived = 0;

			BeginReceive(state, 0, state.Buffer.Length - state.BytesReceived);
		}

		private static void ValidateMessageSize(State state)
		{
			if (state.MessageSize < 0)
				throw new ProtocolViolationException("The host sent a negative message size.");

			if (state.MessageSize == 0)
				throw new ProtocolViolationException("The host sent a zero length message.");
		}

		private void ReadMessageBody(State state)
		{
			if (state.BytesReceived == state.MessageSize)
			{
				var message = messageFormatter.Deserialise(state.Buffer);
				Publish(message);
				Listen();
			} 
			else
			{
				if (state.BytesReceived == 0)
					throw new ProtocolViolationException("The host closed the connection before the entire message was received");

				BeginReceive(state, state.BytesReceived, state.Buffer.Length - state.BytesReceived);
			}
		}

		/// <summary>
		/// Closes the TCP client.
		/// </summary>
		public void Dispose()
		{
			listening = false;
		}

		/// <inheritdoc />
		public void Publish(Message message)
		{
			messageSink.Publish(message);
		}

		private class State
		{
			public byte[] Buffer { get; set; }
			public int MessageSize { get; set; }
			public int BytesReceived { get; set; }

			public State()
			{
				Buffer = new byte[4];
				MessageSize = -1;
				BytesReceived = 0;
			}
		}
	}
}
