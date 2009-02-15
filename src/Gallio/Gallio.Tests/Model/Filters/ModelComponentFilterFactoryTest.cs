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
using Gallio.Model;
using Gallio.Model.Filters;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(ModelComponentFilterFactory<ITest>))]
    [Author("Julian Hidalgo")]
    public class ModelComponentFilterFactoryTest
    {
        [Test]
        [Row("Id", typeof(IdFilter<ITest>))]
        [Row("Name", typeof(NameFilter<ITest>))]
        [Row("Assembly", typeof(AssemblyFilter<ITest>))]
        [Row("Namespace", typeof(NamespaceFilter<ITest>))]
        [Row("Type", typeof(TypeFilter<ITest>))]
        [Row("ExactType", typeof(TypeFilter<ITest>))]
        [Row("Member", typeof(MemberFilter<ITest>))]
        [Row("SomeOtherKey", typeof(MetadataFilter<ITest>))]
        public void CreateFilter(string key, Type filterType)
        {
            Filter<string> filterValue = new EqualityFilter<string>("");
            Filter<ITest> filter = (new ModelComponentFilterFactory<ITest>()).CreateFilter(key, filterValue);
            Assert.AreEqual(filter.GetType(), filterType);
        }
    }
}
