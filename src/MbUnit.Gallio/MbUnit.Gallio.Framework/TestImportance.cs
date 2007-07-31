// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies the test's relative importance which can be used for classifying tests
    /// to be executed.
    /// </summary>
    /// <remarks>
    /// In MbUnit v2 this enum had the <see cref="FlagsAttribute" /> attribute and allowed
    /// multiple values to be specified together by OR-ing them.  This usage is no longer permitted because
    /// it does not work well with the new metadata architecture.  To indicate that a test
    /// belongs to multiple importance classes, decorate it with multiple occurrences of
    /// the <see cref="ImportanceAttribute" />.
    /// </remarks>
    public enum TestImportance
    {
        /// <summary>
        /// Critical importance.
        /// </summary>
        Critical,

        /// <summary>
        /// Severe importance.  Less important than <see cref="Critical" />.
        /// </summary>
        Severe,

        /// <summary>
        /// Serious importance.  Less important than <see cref="Severe" />.
        /// </summary>
        Serious,

        /// <summary>
        /// Default importance.
        /// </summary>
        Default,

        /// <summary>
        /// Not important.
        /// </summary>
        NoOneReallyCaresAbout
    }
}