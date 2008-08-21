using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(Tag), ImplementsOperatorOverloads = false)]
    public class TagTest : BaseTagTest<Tag>
    {
        public override EquivalenceClassCollection<Tag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<Tag>.FromDistinctInstances(
                new BodyTag(),
                new BodyTag() { Contents = { new TextTag("text") } },
                new BodyTag() { Contents = { new TextTag("text"), new TextTag("more") } }
                );
        }
    }
}
