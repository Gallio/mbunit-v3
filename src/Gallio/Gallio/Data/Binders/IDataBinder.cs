// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Data.Binders
{
    /// <summary>
    /// <para>
    /// A data binder specifies how to produce values that are suitable for data binding
    /// in some context.
    /// </para>
    /// <para>
    /// The typical <see cref="IDataBinder"/> lifecycle is as follows:
    /// <list type="bullet">
    /// <item>First, a <see cref="DataBindingContext" /> is created.</item>
    /// <item>Next the client calls <see cref="IDataBinder.Register"/> for each
    /// <see cref="IDataBinder" />, supplying the <see cref="DataBindingContext" />
    /// and a <see cref="IDataSourceResolver" /> for resolving data sources.</item>
    /// <item>Then the client calls <see cref="DataBindingContext.GetItems" /> and
    /// begins enumerating over the items.</item>
    /// <item>For each item, the client calls <see cref="IDataBindingAccessor.GetValue" />
    /// using the <see cref="IDataBindingAccessor" /> produced by the <see cref="IDataBinder" />s
    /// to obtain the bound values.</item>
    /// <item>When finished with an item, the client disposes it.</item>
    /// </list>
    /// </para>
    /// <para>
    /// The lifecycle may also be explained in terms of three phases from the perspective
    /// of a <see cref="IDataBinder" />.
    /// <list type="bullet">
    /// <item>
    /// <term>Prebinding</term>
    /// <description>The <see cref="IDataBinder.Register" /> method is called to register
    /// interest in particular <see cref="IDataSet"/>s with the <see cref="DataBindingContext" />.</description>
    /// </item>
    /// <item>
    /// <term>Binding</term>
    /// <description>The <see cref="IDataBindingAccessor.GetValue" /> method is called to obtain
    /// bound values from a <see cref="DataBindingItem" />.</description>
    /// </item>
    /// <item>
    /// <term>Unbinding</term>
    /// <description>The <see cref="DataBindingItem" />'s <see cref="IDisposable.Dispose" /> method
    /// is called to release resources used by the bound values.</description>
    /// </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Several data binders may be composed to generate complex values that
    /// are produced using contributions from other data binders.  For example,
    /// a simple data binder might select a single value from a data source and
    /// convert it to the right time.  A composite data binder may then use the
    /// values produced by several of these simple data binders to initialize
    /// the fields of a composite object.  Thus a composite data binder facilitates
    /// the production of relatively complex object graphs that are generated from
    /// primitive data binding units.
    /// </para>
    /// </remarks>
    public interface IDataBinder
    {
        /// <summary>
        /// Registers the <see cref="IDataBinder"/>'s requests to query particular
        /// <see cref="IDataSet"/>s with the <see cref="DataBindingContext"/>.
        /// </summary>
        /// <param name="context">The data binding context</param>
        /// <param name="resolver">The data source resolver</param>
        /// <returns>The data binding accessor to use for obtaining bound values
        /// from <see cref="DataBindingItem"/>s produced by the <see cref="DataBindingContext" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="resolver"/> is null</exception>
        IDataBindingAccessor Register(DataBindingContext context, IDataSourceResolver resolver);
    }
}
