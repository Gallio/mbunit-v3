using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(BodyTag), ImplementsOperatorOverloads = false)]
    public class BodyTagTest : BaseTagTest<BodyTag>
    {
        public override EquivalenceClassCollection<BodyTag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<BodyTag>.FromDistinctInstances(
                new BodyTag(),
                new BodyTag() { Contents = { new TextTag("text") } },
                new BodyTag() { Contents = { new TextTag("text"), new TextTag("more") } }
                );
        }
    }
}
