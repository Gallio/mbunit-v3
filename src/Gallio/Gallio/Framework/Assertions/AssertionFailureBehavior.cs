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

namespace Gallio.Framework.Assertions
{
    /// <summary>
    /// Specifies the behavior that should take place when a <see cref="AssertionFailure" />
    /// is submitted to the <see cref="AssertionContext"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// There are three orthogonal dimensions to the assertion failure behavior:
    /// <list type="bullet">
    /// <item><see cref="Log"/> / No-<see cref="Log" />: Log the failure when reported, or do not log it.</item>
    /// <item><see cref="Throw"/> / No-<see cref="Throw"/>: Throw an exception to abort computation, or allow it to continue</item>
    /// <item><see cref="Discard"/> / No-<see cref="Discard"/> (aka. <see cref="CaptureAndContinue" />): Discard the failure when finished reporting it, or capture it in a list for further processing</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Flags]
    public enum AssertionFailureBehavior
    {
        /// <summary>
        /// When an assertion failure is reported, capture it in a list, log it, and allow
        /// the computation to continue.
        /// </summary>
        /// <value>1</value>
        Log = 1,

        /// <summary>
        /// When an assertion failure is reported, capture it in a list, then throw an
        /// <see cref="AssertionFailureException" /> to immediately abort the current computation.
        /// </summary>
        /// <value>2</value>
        Throw = 2,

        /// <summary>
        /// When an assertion failure is reported, capture it in a list, log it, then throw an
        /// <see cref="AssertionFailureException" /> to immediately abort the current computation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the default behavior for assertion contexts.
        /// </para>
        /// </remarks>
        /// <value>3</value>
        LogAndThrow = Log | Throw,

        /// <summary>
        /// When an assertion failure is reported, discard it (instead of capturing it in a list)
        /// and allow the current computation to continue.
        /// </summary>
        /// <value>4</value>
        Discard = 4,

        /// <summary>
        /// When an assertion failure is reported, capture it in a list and allow the current
        /// computation to continue.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the opposite of <see cref="Discard"/>, and is an alias for the case where none
        /// of the other flags are specified.
        /// </para>
        /// </remarks>
        /// <value>0</value>
        CaptureAndContinue = 0
    }
}