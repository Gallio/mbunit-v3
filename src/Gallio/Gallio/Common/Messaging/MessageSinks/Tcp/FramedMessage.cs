using System;
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
			socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, OnWrite, new	State(socket));
		}

		private static void OnWrite(IAsyncResult asyncResult)
		{
			var state = (State)asyncResult.AsyncState;
			state.Socket.EndSend(asyncResult);
		}

		private class State
		{
			public Socket Socket { get; private set; }

			public State(Socket socket)
			{
				Socket = socket;
			}
		}
	}
}