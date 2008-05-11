// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Model;
using MbUnit.Framework;

namespace MbUnit.TestResources
{
    /// <summary>
    /// A variety of tests for which the framework will generate an annotation due to
    /// their being malformed or noteworthy.
    /// </summary>
    [TestFixture]
    public class AnnotatedTests
    {
        [Test, SetUp]
        public void InvalidCombinationOfPrimaryAttributes()
        {
        }

        [Test, Description(null)]
        public void InvalidAttributeParameter()
        {
        }

        [Test, Ignore("An ignored test should generate an annotation.")]
        public void Ignored()
        {
        }

        [Test, Annotation(AnnotationType.Error, "Bad mojo.", Details="Simulated annotation.")]
        public void ErrorAnnotation()
        {
        }

        [Test, Annotation(AnnotationType.Warning, "Bad mojo.", Details = "Simulated annotation.")]
        public void WarningAnnotation()
        {
        }

        [Test, Annotation(AnnotationType.Info, "Bad mojo.", Details = "Simulated annotation.")]
        public void InfoAnnotation()
        {
        }
    }
}
