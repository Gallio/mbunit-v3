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
using System.Text;
using Gallio.Model.Tree;

namespace Gallio.Model.Commands
{
    /// <summary>
    /// Defines the test ordering strategy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This default strategy first compares test by explicit ordering (using <see cref="Test.Order"/>)
    /// then defines an implicit ordering by name (using <see cref="TestComponent.Name"/>).
    /// </para>
    /// <para>
    /// Why implicitly order tests by name instead of randomly?
    /// </para>
    /// <para>
    /// It is my belief that ordering tests by name significantly eases test result interpretation
    /// because the test results are produced in a meaningful and predictable order.  If the implicit
    /// order is known, then it's easier for a tester to guage test progress by inspecting the name
    /// of the currently executing test (in addition to using cues provided by progress monitors, of course).
    /// Related tests also tend to have similar names or share a common prefix so sorting by name
    /// can effectively cluster these tests together thereby providing feedback about related topics
    /// around the same time.
    /// </para>
    /// <para>
    /// A tester may of course take advantage of this known ordering to produce tests that are
    /// dependent upon one another without making that clear in the code by specifying explicit test
    /// dependencies or ordering.  When a test is renamed, the implicit ordering will change, possibly
    /// causing other tests to fail.  However, it is not Gallio's objective to police
    /// testers, who may well adopt any number of inadvisable practices to meet their ends.
    /// </para>
    /// <para>
    /// Nevertheless, any test framework is still free to produce a random ordering of independent tests.
    /// It suffices for the framework to initialize the <see cref="Test.Order" /> property of each of its
    /// testsw to a distinct random number. Problem solved.
    /// </para>
    /// <para>
    /// Perhaps someday we can also offer the user a global choice among alternative test ordering strategies.
    /// </para>
    /// </remarks>
    public sealed class DefaultTestOrderStrategy : IComparer<Test>
    {
        /// <inheritdoc />
        public int Compare(Test a, Test b)
        {
            int discriminator = a.Order.CompareTo(b.Order);
            if (discriminator == 0)
                discriminator = String.Compare(a.Name, b.Name, StringComparison.InvariantCultureIgnoreCase);

            return discriminator;
        }
    }
}