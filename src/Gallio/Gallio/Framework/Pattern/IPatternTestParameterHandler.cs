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

using Gallio.Framework.Data;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern test parameter handler provides the logic that implements the various
    /// phases of the test parameter binding lifecycle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each method represents the behavior to be performed during a particular phase.
    /// Different actions are permitted during each phase.  Consult the
    /// documentation the appropriate method of this interface for restrictions.
    /// </para>
    /// <para>
    /// The phases generally run in the following order.  Some phases may be skipped
    /// due to exceptions or if there is no work to be done.
    /// <list type="bullet">
    /// <item>-- for each test instance --</item>
    /// <item><see cref="BindTestParameter" /></item>
    /// <item><see cref="UnbindTestParameter" /></item>
    /// <item>-- end --</item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IPatternTestParameterHandler
    {
        /// <summary>
        /// <para>
        /// Binds a value to a test parameter.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Associated a value with a slot on the test fixture or test method.</item>
        /// <item>Storing the object for later use during the test run.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null.</param>
        /// <param name="value">The value to bind to the parameter.</param>
        void BindTestParameter(PatternTestInstanceState testInstanceState, object value);

        /// <summary>
        /// <para>
        /// Unbinds a test parameter.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Releasing any resources used by the test parameter.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null.</param>
        /// <param name="value">The value that was bound to the test parameter.</param>
        void UnbindTestParameter(PatternTestInstanceState testInstanceState, object value);
    }
}
