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
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Describes a test kind.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test kind is ignored by the test runner but it allows tests to be classified
    /// so that a user interface can provide appropriate decorations and other affordances
    /// for any test kinds that it recognizes.
    /// </para>
    /// <para>
    /// The test kind is associated with a test by setting the test's <see cref="MetadataKeys.TestKind"/>
    /// metadata to the id of the test kind component.
    /// </para>
    /// </remarks>
    [Traits(typeof(TestKindTraits))]
    public interface ITestKind
    {
    }
}
