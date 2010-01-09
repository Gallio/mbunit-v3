// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using MbUnit.Framework;
using Gallio.Common.Reflection.Impl;
using Gallio.Common.Platform;

namespace Gallio.Tests.Common.Reflection.Impl
{
    [TestsOn(typeof(UnresolvedCodeElementFactory))]
    public class UnresolvedCodeElementFactoryTest
    {
        [Test]
        public void Instance_WhenRuntimeIsDotNet20_ReturnsDotNet20Instance()
        {
            if (DotNetFrameworkSupport.FrameworkVersion != DotNetFrameworkVersion.DotNet40
                && DotNetFrameworkSupport.FrameworkVersion != DotNetFrameworkVersion.DotNet35
                && DotNetFrameworkSupport.FrameworkVersion != DotNetFrameworkVersion.DotNet20)
                return;

            Assert.IsInstanceOfType<Gallio.Common.Reflection.Impl.DotNet20.UnresolvedCodeElementFactoryInternal>(UnresolvedCodeElementFactory.Instance);
        }
    }
}
