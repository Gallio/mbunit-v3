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
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies that a class represents a test fixture.  This attribute is optional.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test fixture attribute is applied to a class that contains a suite
    /// of related test cases.  If an error occurs while initializing the fixture
    /// or if at least one of the test cases within the fixture fails,
    /// then the fixture itself will be deemed to have failed.  Otherwise the
    /// fixture will pass.
    /// Output from the fixture, such as text written to the console, is captured
    /// by the framework and will be included in the test report.
    /// </para>
    /// <para>
    /// A test fixture has a timeout of 10 minutes by default.  This may be changed
    /// using the <see cref="TimeoutAttribute" />.
    /// </para>
    /// <para>
    /// This attribute may be omitted whenever a test fixture class contains at least
    /// one test method or test parameter or when other MbUnit attributes are applied
    /// to the test fixture class.  This is almost always the case unless for some reason
    /// you have an empty fixture.
    /// </para>
    /// <para>
    /// The class must have a public default constructor.  The class may not be static.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class TestFixtureAttribute : TestTypePatternAttribute
    {
    }
}
