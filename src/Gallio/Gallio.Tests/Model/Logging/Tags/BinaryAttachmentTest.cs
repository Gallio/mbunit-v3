using Gallio.Model.Logging;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(BinaryAttachment), ImplementsOperatorOverloads = false)]
    public class BinaryAttachmentTest : IEquivalenceClassProvider<BinaryAttachment>
    {
        public EquivalenceClassCollection<BinaryAttachment> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<BinaryAttachment>.FromDistinctInstances(
                new BinaryAttachment("abc", MimeTypes.PlainText, new byte[] { 1, 2, 3 }),
                new BinaryAttachment("def", MimeTypes.PlainText, new byte[] { 1, 2, 3 }),
                new BinaryAttachment("abc", MimeTypes.Xml, new byte[] { 1, 2, 3 }),
                new BinaryAttachment("abc", MimeTypes.PlainText, new byte[] { 1, 2 }));
        }

        [Test]
        public void NullAttachmentNamePicksAUniqueOne()
        {
            Assert.IsNotNull(new BinaryAttachment(null, MimeTypes.PlainText, new byte[0]));
        }
    }
}