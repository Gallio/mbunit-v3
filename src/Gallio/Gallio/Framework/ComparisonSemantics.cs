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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Extensibility;

namespace Gallio.Framework
{
    /// <summary>
    /// Describes the semantics of how objects should be compared.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class encapsulates a default set of rules for comparing objects.  These rules may be
    /// used as the foundation of a suite of standard assertion functions for comparing identity,
    /// equality and relations.
    /// </para>
    /// <para>
    /// The comparison engine has extension points available:
    /// <list type="bullet">Extend the object comparison by registring custom comparers through <see cref="CustomComparers"/>.</list>
    /// <list type="bullet">Extend the object eqaulity by registring custom equality comparers through <see cref="CustomEqualityComparers"/>.</list>
    /// </para>
    /// <para>
    /// Custom comparers defined through <see cref="CustomComparers"/> or <see cref="CustomEqualityComparers"/> 
    /// have always a higher priority than any built-in comparer, including inner type comparers such as 
    /// <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </para>
    /// </remarks>
    public static class ComparisonSemantics
    {
        private static IComparisonSemantics instance;

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static IComparisonSemantics Default
        {
            get
            {
                if (instance == null)
                {
                    instance = Runtime.RuntimeAccessor.IsInitialized
                        ? (IComparisonSemantics)Runtime.RuntimeAccessor.ServiceLocator.ResolveByComponentId("Gallio.ComparisonSemantics")
                        : new DefaultComparisonSemantics(new DefaultExtensionPoints());
                }

                return instance;
            }
        }
    }
}