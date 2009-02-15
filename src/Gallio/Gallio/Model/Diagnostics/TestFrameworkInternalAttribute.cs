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
using Gallio.Model.Diagnostics;

namespace Gallio.Model.Diagnostics
{
    /// <summary>
    /// <para>
    /// This attribute is used to mark methods and types that are internal to the implementation
    /// of the test framework.
    /// </para>
    /// <para>
    /// The <see cref="StackTraceFilter" /> uses this attribute to identify stack
    /// frames that may safely be filtered out when describing failures to users.  If the
    /// stack frame refers to a method with this attribute or declared by a type
    /// with this attribute then it is omitted.  The attribute also applies to nested types
    /// to help cover anonymous delegates as well.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Other attributes may also contribute to stack trace filtering.  Refer to
    /// <see cref="StackTraceFilter"/> for details.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class TestFrameworkInternalAttribute : Attribute
    {
    }
}