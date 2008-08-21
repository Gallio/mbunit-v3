
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(EmbedTag), ImplementsOperatorOverloads = false)]
    public class EmbedTagTest : BaseTagTest<EmbedTag>
    {
        public override EquivalenceClassCollection<EmbedTag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<EmbedTag>.FromDistinctInstances(
                new EmbedTag("attachment1"),
                new EmbedTag("attachment2")
                );
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfAttachmentIsNull()
        {
            new EmbedTag(null);
        }

        protected override void PrepareLogWriterForWriteToTest(TestLogWriter writer)
        {
            writer.AttachPlainText("attachment1", "abc");
            writer.AttachPlainText("attachment2", "def");
        }
    }
}