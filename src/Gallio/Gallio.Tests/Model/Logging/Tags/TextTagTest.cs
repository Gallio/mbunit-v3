using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(TextTag), ImplementsOperatorOverloads = false)]
    public class TextTagTest : BaseTagTest<TextTag>
    {
        public override EquivalenceClassCollection<TextTag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<TextTag>.FromDistinctInstances(
                new TextTag(""),
                new TextTag("text"),
                new TextTag("other"),
                new TextTag("   \nsomething\nwith  embedded  newlines and significant whitespace to\nencode\n  ")
                );
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfTextIsNull()
        {
            new TextTag(null);
        }
    }
}