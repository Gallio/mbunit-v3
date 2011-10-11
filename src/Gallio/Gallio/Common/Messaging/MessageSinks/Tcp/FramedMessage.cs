using System;
using System.IO;
using System.Net.Sockets;

namespace Gallio.Common.Messaging.MessageSinks.Tcp
{
	/// <summary>
	/// Wraps a Message, and adds the message length before when writing to a socket.
	/// </summary>
	internal class FramedMessage
	{
		private readonly byte[] messageData;

		public FramedMessage(byte[] messageData)
		{
			this.messageData = messageData;
		}

		public void WriteTo(Socket socket)
		{
			var messageDataLengthBytes = BitConverter.GetBytes(messageData.Length);
			var bytes = new byte[messageDataLengthBytes.Length + messageData.Length];
			Buffer.BlockCopy(messageDataLengthBytes, 0, bytes, 0, messageDataLengthBytes.Length);
			Buffer.BlockCopy(messageData, 0, bytes, messageDataLengthBytes.Length, messageData.Length);
			socket.Send(bytes);
		}

		private static void OnWrite(IAsyncResult asyncResult)
		{
			var state = asyncResult.AsyncState as State;

			if (state == null)
				return;

			state.Stream.EndWrite(asyncResult);
			state.Stream.Flush();
		}

		private class State
		{
			public Stream Stream { get; private set; }

			public State(Stream stream)
			{
				Stream = stream;
			}
		}
	}
}