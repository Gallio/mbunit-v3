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

namespace Gallio.Model
{
    /// <summary>
    /// A test parameter describes a formal parameter of a <see cref="ITest" />
    /// to which a value may be bound and used during test execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ITestComponent.Name" /> property of a test parameter should be
    /// unique among the set parameters belonging to its <see cref="Owner"/> to ensure
    /// that it can be differentiated from others.  However, this constraint is not enforced.
    /// </para>
    /// </remarks>
    public interface ITestParameter : ITestComponent
    {
        /// <summary>
        /// Gets or sets the test that owns this parameter, or null if this parameter
        /// does not yet have an owner.
        /// </summary>
        ITest Owner { get; set; }
    }
}
