// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Markup.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup.Tags
{
    public class SectionTagTest : BaseTagTest<SectionTag>
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<SectionTag>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses = equivalenceClasses
        };

        public override EquivalenceClassCollection<SectionTag> GetEquivalenceClasses()
        {
            return equivalenceClasses;
        }

        private static EquivalenceClassCollection<SectionTag> equivalenceClasses = new EquivalenceClassCollection<SectionTag>
        {
            { new SectionTag("section") },
            { new SectionTag("section") { Contents = { new TextTag("text") }}},
            { new SectionTag("section") { Contents = { new TextTag("text"), new TextTag("more") }}}
        };
    }
}
