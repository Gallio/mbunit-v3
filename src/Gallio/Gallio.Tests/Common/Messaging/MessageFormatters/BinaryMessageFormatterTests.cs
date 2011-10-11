using Gallio.Common.Messaging;
using Gallio.Common.Messaging.MessageFormatters;
using Gallio.Model.Messages.Exploration;
using MbUnit.Framework;
using NHamcrest.Core;

namespace Gallio.Tests.Common.Messaging
{
	public class BinaryMessageFormatterTests
	{
		[Test]
		public void There_and_back_again()
		{
			var formatter = new BinaryMessageFormatter();
			const string parentTestId = "parentTestId";
			var message = new TestDiscoveredMessage { ParentTestId = parentTestId };

			var bytes = formatter.Serialise(message);
			var deserialisedMessage = (TestDiscoveredMessage)formatter.Deserialise(bytes);

			Assert.That(deserialisedMessage.ParentTestId, Is.EqualTo(parentTestId));
		}
	}
}