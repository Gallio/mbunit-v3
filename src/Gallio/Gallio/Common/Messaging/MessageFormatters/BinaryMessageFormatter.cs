using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gallio.Common.Messaging.MessageFormatters
{
	///<summary>
	/// IMessageFormatter impl using the .net BinaryFormatter.
	///</summary>
	public class BinaryMessageFormatter : IMessageFormatter
	{
		private readonly BinaryFormatter binaryFormatter;

		///<summary>
		/// Default ctor.
		///</summary>
		public BinaryMessageFormatter()
		{
			binaryFormatter = new BinaryFormatter();
		}

		///<summary>
		/// Serialise a message.
		///</summary>
		///<param name="message">The message to serialise.</param>
		///<returns>A byte array containing the serialised data.</returns>
		public byte[] Serialise(Message message)
		{
			using (var memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, message);
				return memoryStream.ToArray();
			}
		}

		///<summary>
		/// Deserialise a message.
		///</summary>
		///<param name="messageData">The raw message data.</param>
		///<returns>The deserialised message.</returns>
		public Message Deserialise(byte[] messageData)
		{
			using (var memoryStream = new MemoryStream(messageData))
			{
				memoryStream.Position = 0; // rewind the stream
				var message = (Message)binaryFormatter.Deserialize(memoryStream);
				return message;
			}
		}
	}
}