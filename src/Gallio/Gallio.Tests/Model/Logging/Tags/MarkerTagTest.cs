using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(MarkerTag), ImplementsOperatorOverloads = false)]
    public class MarkerTagTest : BaseTagTest<MarkerTag>
    {
        public override EquivalenceClassCollection<MarkerTag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<MarkerTag>.FromDistinctInstances(
                new MarkerTag(Marker.Highlight),
                new MarkerTag(Marker.Highlight) { Contents = { new TextTag("text") } },
                new MarkerTag(Marker.Highlight) { Contents = { new TextTag("text"), new TextTag("more") } }
                );
        }
    }
}