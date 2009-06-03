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

namespace Gallio.Common.Diagnostics
{
    /// <summary>
    /// This attribute is used to mark methods that call into user code.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="StackTraceFilter" /> uses this attribute to determine at what
    /// point control flow initially transitioned from the test framework into the test.
    /// It can then filter out irrelevant frames that lie above the entry point.
    /// </para>
    /// <para>
    /// If two entry points are nested within one another in the call stack, then the
    /// effect is the same as if the outermost entry point did not appear.  This enables
    /// test frameworks to support recursion into themselves.
    /// </para>
    /// <para>
    /// Other attributes may also contribute to stack trace filtering.  Refer to
    /// <see cref="StackTraceFilter"/> for details.
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple=false, Inherited=true)]
    public sealed class UserCodeEntryPointAttribute : Attribute
    {
    }
}