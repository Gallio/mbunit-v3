using System;
using Gallio.Model.Logging;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(TextAttachment), ImplementsOperatorOverloads = false)]
    public class TextAttachmentTest : IEquivalenceClassProvider<TextAttachment>
    {
        public EquivalenceClassCollection<TextAttachment> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<TextAttachment>.FromDistinctInstances(
                new TextAttachment("abc", MimeTypes.PlainText, "text"),
                new TextAttachment("def", MimeTypes.PlainText, "text"),
                new TextAttachment("abc", MimeTypes.Xml, "text"),
                new TextAttachment("abc", MimeTypes.PlainText, "blah"));
        }

        [Test]
        public void NullAttachmentNamePicksAUniqueOne()
        {
            Assert.IsNotNull(new TextAttachment(null, MimeTypes.PlainText, "foo"));
        }
    }
}
