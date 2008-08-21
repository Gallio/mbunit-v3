using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(SectionTag), ImplementsOperatorOverloads = false)]
    public class SectionTagTest : BaseTagTest<SectionTag>
    {
        public override EquivalenceClassCollection<SectionTag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<SectionTag>.FromDistinctInstances(
                new SectionTag("section"),
                new SectionTag("section") { Contents = { new TextTag("text") } },
                new SectionTag("section") { Contents = { new TextTag("text"), new TextTag("more") } }
                );
        }
    }
}
