namespace Gallio.Common.Messaging
{
	/// <summary>
	/// Serialise or deserialise a message for the wire.
	/// </summary>
	public interface IMessageFormatter
	{
		///<summary>
		/// Serialise a message.
		///</summary>
		///<param name="message">The message to serialise.</param>
		///<returns>A byte array containing the serialised data.</returns>
		byte[] Serialise(Message message);

		///<summary>
		/// Deserialise a message.
		///</summary>
		///<param name="messageData">The raw message data.</param>
		///<returns>The deserialised message.</returns>
		Message Deserialise(byte[] messageData);
	}
}