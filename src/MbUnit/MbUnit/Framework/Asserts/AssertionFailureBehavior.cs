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

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Specifies the behavior that should take place when a <see cref="AssertionFailure" />
    /// is submitted to the <see cref="AssertionContext"/>.
    /// </para>
    /// </summary>
    public enum AssertionFailureBehavior
    {
        /// <summary>
        /// <para>
        /// When an assertion failure is reported, log it, then throw an
        /// <see cref="AssertionFailureException" /> to immediately abort the current computation.
        /// </para>
        /// <para>
        /// This is the default behavior.
        /// </para>
        /// </summary>
        LogAndThrow = 0,

        /// <summary>
        /// <para>
        /// When an assertion failure is reported, log the failure and allow the computation to
        /// continue.
        /// </para>
        /// </summary>
        Log,

        /// <summary>
        /// <para>
        /// When an assertion failure is reported, throw an <see cref="AssertionFailureException" />
        /// to immediately abort the current computation.
        /// </para>
        /// </summary>
        Throw,

        /// <summary>
        /// <para>
        /// When an assertion failure is reported, save it in a list to be processed 
        /// or possibly logged later and allow the current computation to continue.
        /// </para>
        /// </summary>
        Defer,

        /// <summary>
        /// <para>
        /// When an assertion failure is reported, ignore it.
        /// </para>
        /// </summary>
        Ignore
    }
}
