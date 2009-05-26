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
using System.Runtime.CompilerServices;
using System.Text;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The Gallio.Framework.Pattern namespace provides the Pattern Test Framework which
    /// can serve as the foundation for building new test frameworks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Pattern Test Framework is based on the idea of extensible and composable units
    /// called patterns (<see cref="IPattern"/>).  Patterns are usually encoded as .Net attributes
    /// and applied to test assemblies, types and members to define tests.
    /// </para>
    /// <para>
    /// MbUnit is based on the Pattern Test Framework.
    /// </para>
    /// <para>
    /// If you are implementing a new attribute-based test framework then you may find it
    /// beneficial to extend the Pattern Test Framework.  Alternately you may implement
    /// <see cref="ITestFramework"/> directly.
    /// </para>
    /// </remarks>
    /// <seealso cref="IPattern"/>
    /// <seealso cref="PatternTestFramework"/>
    [CompilerGenerated]
    class NamespaceDoc
    {
    }
}
