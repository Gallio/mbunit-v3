using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    [VerifyEqualityContract(typeof(ContainerTag), ImplementsOperatorOverloads = false)]
    public class ContainerTagTest : BaseTagTest<ContainerTag>
    {
        public override EquivalenceClassCollection<ContainerTag> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<ContainerTag>.FromDistinctInstances(
                new BodyTag(),
                new BodyTag() { Contents = { new TextTag("text") } },
                new BodyTag() { Contents = { new TextTag("text"), new TextTag("more") } }
                );
        }

        [Test, ExpectedArgumentNullException]
        public void AcceptContentsThrowsIfVisitorIsNull()
        {
            new BodyTag().AcceptContents(null);
        }
    }
}
